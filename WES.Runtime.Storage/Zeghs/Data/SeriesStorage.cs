using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Events;

namespace Zeghs.Data {
	internal sealed class SeriesStorage {
		private Dictionary<int, int> __cIndexs = null;
		private List<SeriesSymbolData> __cSeries = null;
		private object __oLock = new object();

		/// <summary>
		///   [取得] SeriesSymbolData 列表儲存個數
		/// </summary>
		internal int Count {
			get {
				return __cSeries.Count;
			}
		}

		/// <summary>
		///   [取得] SeriesSymbolData 列表
		/// </summary>
		internal SeriesSymbolData this[int index] {
			get {
				return __cSeries[index];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="capacity">擴充的容量大小</param>
		internal SeriesStorage(int capacity) {
			__cIndexs = new Dictionary<int, int>(capacity);
			__cSeries = new List<SeriesSymbolData>(capacity);
		}

		/// <summary>
		///   加入 SeriesSymbolData 列表
		/// </summary>
		/// <param name="series">SeriesSymbolData 列表</param>
		/// <param name="useIdentify">是否使用 Identify 當作 Hash 值(true=使用 Identify 作為 Hash 值, false=使用時間週期總秒數當作 Hash 值)</param>
		internal void Add(SeriesSymbolData series, bool useIdentify = false) {
			int iHash = (useIdentify) ? series.Id : series.DataRequest.Resolution.TotalSeconds;
			lock (__cIndexs) {
				if (!__cIndexs.ContainsKey(iHash)) {
					int iIndex = __cSeries.Count;
					__cSeries.Add(series);
					__cIndexs.Add(iHash, iIndex);

					if (!useIdentify) {
						series.Id = iHash;  //將 Id 改為 iHash(iHash=時間週期總秒數, 當存入 SeriesStorage 後都以 Id 當作 Hash 存取)

						bool bBase = (iHash == Resolution.MIN_BASE_TOTALSECONDS || iHash == Resolution.MAX_BASE_TOTALSECONDS);
						if (!bBase) {
							series.onRequest += SeriesSymbolData_onRequest;
						}
					}
				}
			}
		}

		/// <summary>
		///   清除 SeriesStorage
		/// </summary>
		internal void Clear() {
			lock (__cIndexs) {
				foreach (SeriesSymbolData cSeries in __cSeries) {
					cSeries.Dispose();
				}
				
				__cSeries.Clear();
				__cIndexs.Clear();
			}
		}

		/// <summary>
		///   取得指定的總秒數週期 SeriesSymbolData 列表
		/// </summary>
		/// <param name="hashKey">可以為時間週期總秒數或是 Identify</param>
		internal SeriesSymbolData GetSeries(int hashKey) {
			int iIndex = 0;
			SeriesSymbolData cSeries = null;
			
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(hashKey, out iIndex)) {
					cSeries = __cSeries[iIndex];
				}
			}
			return cSeries;
		}

		internal void MergeTick(ITick tick) {
			lock (__cIndexs) {
				int iCount = __cSeries.Count;
				
				Parallel.For(0, iCount, (i) => {
					SeriesSymbolData cSeries = __cSeries[i];
					if (cSeries.Initialized) {
						cSeries.Merge(tick);
					}
				});
			}
		}

		/// <summary>
		///   移除 SeriesSymbolData 列表
		/// </summary>
		/// <param name="seriesId">SeriesSymbolData id</param>
		internal void Remove(int seriesId) {
			int iIndex = 0;
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(seriesId, out iIndex)) {
					SeriesSymbolData cTarget = __cSeries[iIndex];
					cTarget.Dispose();

					int iLast = __cSeries.Count - 1;
					if (iLast > 0 && iLast > iIndex) {
						SeriesSymbolData cLast = __cSeries[iLast];

						int iLastSeriesId = cLast.Id;
						__cIndexs[iLastSeriesId] = iIndex;
						__cSeries[iIndex] = cLast;
					}

					__cIndexs.Remove(seriesId);
					__cSeries.RemoveAt(iLast);
				}
			}
		}

		/// <summary>
		///   處理清盤重置
		/// </summary>
		/// <param name="dataSource">即時報價來源名稱</param>
		internal void Reset(string dataSource) {
			lock (__cIndexs) {
				int iCount = __cSeries.Count;
				Parallel.For(0, iCount, (i) => {
					SeriesSymbolData cSeries = __cSeries[i];
					
					string sDataFeed = cSeries.DataRequest.DataFeed;
					if (sDataFeed.Equals(dataSource)) {
						cSeries.Reset();
					}
				});
			}
		}

		private void SeriesSymbolData_onRequest(object sender, DataRequestEvent e) {
			SeriesSymbolData cTargetSeries = sender as SeriesSymbolData;
			if (!cTargetSeries.DataRequest.Range.IsAlreadyRequestAllData) {
				int iTotalSeconds = cTargetSeries.DataRequest.Resolution.TotalSeconds;

				SeriesSymbolData cBaseSeries = GetSeries(((iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) ? Resolution.MIN_BASE_TOTALSECONDS : Resolution.MAX_BASE_TOTALSECONDS));
				if (cBaseSeries.DataRequest.Range.IsAlreadyRequestAllData) {
					e.IsAlreadyRequestAllData = true;
					e.Count = cBaseSeries.DataRequest.Range.Count / e.Rate;
				} else {
					DataRequestEvent cRequestEvent = null;
					lock (__oLock) {  //需要鎖定資源(因為有可能多個策略同時請求基礎序列類別資料, 如果不鎖定會重複請求資料)
						if (e.Totals == 0) {  //檢查是否資料總個數為0(0=使用 InstrumentDataRequest 請求歷史資料)
							cRequestEvent = e.Clone();  //直接複製
						} else {  //如果不為0(表示使用者取得資料時超過目前已下載歷史資料的區間, 經過基礎週期比率計算之後再向伺服器請求歷史資料)
							int iTotals = e.Totals * e.Rate;  //資料總個數 * 縮放比率 = 基礎週期需要請求的資料總個數
							int iRequestCount = iTotals - cBaseSeries.DataRequest.Range.Count;  //計算後的資料總個數 - 基礎週期目前已下載後的資料個數 = 欲請求的的個數
							cRequestEvent = new DataRequestEvent(iRequestCount, iTotals, cBaseSeries.DataRequest.Resolution.Rate);
						}
						cBaseSeries.OnRequest(cRequestEvent);  //回補歷史資訊
					}

					if (cRequestEvent.Result == 0) {
						e.Result = cRequestEvent.Result;
						e.Ranges = cRequestEvent.Ranges;
						e.Count = cRequestEvent.Count / e.Rate;
						e.IsAlreadyRequestAllData = cRequestEvent.IsAlreadyRequestAllData;
					}
				}

				if (e.IsAlreadyRequestAllData) {  //如果已經全部讀取完畢就取消事件
					cTargetSeries.RemoveRequest();
				}

				lock (__oLock) {  //須鎖定資源(將基礎序列資料合併至目標序列時需要鎖定, 避免合併的時候多執行緒導致合併資料產生問題)
					int iTargetCount = cTargetSeries.Indexer.Count;
					int iAllocSize = e.Count - cTargetSeries.DataRequest.Range.Count;
					if (iTargetCount == 0 || iAllocSize > 0) {
						if (iAllocSize > 0) {
							++iAllocSize;  //多預留一個空間(避免陣列空間不足)
							cTargetSeries.AdjustSize(iAllocSize, true);
						}
						cBaseSeries.Merge(cTargetSeries);  //合併資料
					}
				}
			}
		}
	}
}  //200行