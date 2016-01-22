using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Events;

namespace Zeghs.Data {
	internal sealed class SeriesStorage {
		private Dictionary<int, int> __cIndexs = null;
		private List<SeriesSymbolData> __cSeries = null;

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
		internal void Add(SeriesSymbolData series) {
			int iTotalSeconds = series.DataRequest.Resolution.TotalSeconds;
			lock (__cIndexs) {
				if (!__cIndexs.ContainsKey(iTotalSeconds)) {
					int iIndex = __cSeries.Count;
					__cSeries.Add(series);
					__cIndexs.Add(iTotalSeconds, iIndex);

					bool bBase = (iTotalSeconds == Resolution.MIN_BASE_TOTALSECONDS || iTotalSeconds == Resolution.MAX_BASE_TOTALSECONDS);
					if (!bBase) {
						series.onRequest += SeriesSymbolData_onRequest;
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
		///   建立指定的時間週期 SeriesSymbolData 列表
		/// </summary>
		/// <param name="dataRequest">InstrumentDataRequest 類別</param>
		/// <returns>返回值: SeriesSymbolData 類別</returns>
		internal SeriesSymbolData Create(InstrumentDataRequest dataRequest) {
			int iTotalSeconds = dataRequest.Resolution.TotalSeconds;
			SeriesSymbolData cBaseSeries = GetSeries(((iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) ? Resolution.MIN_BASE_TOTALSECONDS : Resolution.MAX_BASE_TOTALSECONDS));
			
			SeriesSymbolData cTargetSeries = cBaseSeries.CreateSeries(dataRequest);
			this.Add(cTargetSeries);

			int iCount = dataRequest.Range.Count;
			cTargetSeries.OnRequest(new DataRequestEvent(iCount, iCount, cTargetSeries.DataRequest.Resolution.Rate));
			return cTargetSeries;
		}

		/// <summary>
		///   取得指定的總秒數週期 SeriesSymbolData 列表
		/// </summary>
		/// <param name="totalSeconds">總秒數</param>
		internal SeriesSymbolData GetSeries(int totalSeconds) {
			int iIndex = 0;
			SeriesSymbolData cSeries = null;
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(totalSeconds, out iIndex)) {
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
		/// <param name="totalSeconds">總秒數</param>
		internal void Remove(int totalSeconds) {
			int iIndex = 0;
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(totalSeconds, out iIndex)) {
					SeriesSymbolData cTarget = __cSeries[iIndex];
					cTarget.Dispose();

					int iLast = __cSeries.Count - 1;
					if (iLast > 0 && iLast > iIndex) {
						SeriesSymbolData cLast = __cSeries[iLast];

						int iTotalSeconds = cLast.DataRequest.Resolution.TotalSeconds;
						__cIndexs[iTotalSeconds] = iIndex;
						__cSeries[iIndex] = cLast;
					}

					__cIndexs.Remove(totalSeconds);
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
					int iPosition = e.Position * e.Rate;
					int iRequestCount = iPosition - cBaseSeries.DataRequest.Range.Count;
					DataRequestEvent cRequestEvent = new DataRequestEvent(iPosition, iRequestCount, cBaseSeries.DataRequest.Resolution.Rate);
					
					cBaseSeries.OnRequest(cRequestEvent);  //回補歷史資訊

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

				int iAllocSize = e.Count - cTargetSeries.DataRequest.Range.Count + 1;
				cTargetSeries.AdjustSize(iAllocSize, true);
				cBaseSeries.Merge(cTargetSeries);  //合併資料
			}
		}
	}
}