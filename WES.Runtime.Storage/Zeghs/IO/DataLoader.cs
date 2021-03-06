﻿using System;
using System.Collections.Generic;
using log4net;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.IO {
	/// <summary>
	///   商品資訊 Instrument 資料讀取者類別
	/// </summary>
	public sealed class DataLoader : IDataLoader, IDisposable {
		private static readonly ILog logger = LogManager.GetLogger(typeof(DataLoader));

		/// <summary>
		///   當有 Instrument 新增時所觸發的更動事件
		/// </summary>
		public event EventHandler<InstrumentChangeEvent> onAddInstrument = null;

		/// <summary>
		///   當所有 Instrument 資料都讀取完畢後所觸發的事件
		/// </summary>
		public event EventHandler onLoadCompleted = null;

		/// <summary>
		///   當有 Instrument 移除時所觸發的更動事件
		/// </summary>
		public event EventHandler<InstrumentChangeEvent> onRemoveInstrument = null;

		private bool __bDisposed = false;  //Dispose旗標
		private int __iMaxBarsReference = 0;
		private int __iMaxInstrumentCount = 0;
		private List<Instrument> __cInstruments = null;

		/// <summary>
		///   [取得] Instrument 商品資訊列表
		/// </summary>
		public List<Instrument> Instruments {
			get {
				return __cInstruments;
			}
		}

		/// <summary>
		///   [取得] 最大 Instrument 資訊個數
		/// </summary>
		public int MaxInstrumentCount {
			get {
				return __iMaxInstrumentCount;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public DataLoader() {
			__cInstruments = new List<Instrument>(16);
		}

		/// <summary>
		///   加入 Instrument 資料
		/// </summary>
		/// <param name="instrument">Instrument 商品資訊類別</param>
		/// <returns>返回值: 資料存放索引值(從 0 開始)</returns>
		public int AddData(Instrument instrument) {
			int iDataIndex = 0;
			lock (__cInstruments) {
				__cInstruments.Add(instrument);
				iDataIndex = __iMaxInstrumentCount++;
			}

			if (onAddInstrument != null) {
				onAddInstrument(this, new InstrumentChangeEvent(instrument, iDataIndex));
			}
			return iDataIndex;
		}

		/// <summary>
		///   非同步讀取資料請求結構內的 Instrument 資料
		/// </summary>
		/// <param name="request">資料請求結構</param>
		/// <param name="result">當成功取得 Instrument 商品資料會使用此委派方法回傳資料</param>
		/// <param name="useCache">是否使用快取 [預設:false](true=序列資料結構建立後保存在快取內，下次需要使用直接從快取拿取, false=重新建立序列資料結構，建立的序列資料需要自行移除否則會占用記憶體空間)</param>
		/// <param name="args">使用者自訂參數</param>
		/// <param name="millisecondsTimeout">回補資料 Timeout 毫秒數 [預設:System.Threading.Timeout.Infinite (永遠等待直到回補完成)]</param>
		public void BeginLoadData(InstrumentDataRequest request, LoadDataCallback result, bool useCache = false, object args = null, int millisecondsTimeout = System.Threading.Timeout.Infinite) {
			SeriesManager.Manager.SetQuoteService(request.DataFeed);
			SeriesManager.Manager.AsyncGetSeries(request, (object sender, SeriesResultEvent e) => {
				SeriesSymbolDataRand cSeriesSymbolDataRand = e.Data;
				cSeriesSymbolDataRand.SetMaxbarsReferance(__iMaxBarsReference);
				Instrument cInstrument = new Instrument(cSeriesSymbolDataRand);  //建立 Instrument 商品資料
				
				Instrument cBars_0 = GetInstrument(0);  //取得目前第 0 個 Instrument 商品資料
				if (cBars_0 != null) {
					cInstrument.MoveBars(cBars_0.Time.Value);
				}

				result(new DataLoaderResult(cInstrument, cInstrument.Quotes, args));
			}, useCache, null, millisecondsTimeout);
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   取得 Instrument 商品資訊
		/// </summary>
		/// <param name="index">資料存放索引值(從 0 起始)</param>
		/// <returns>返回值: Instrument 商品資訊類別</returns>
		public Instrument GetInstrument(int index) {
			Instrument cInstrument = null;
			int iCount = __cInstruments.Count;
			if (iCount > 0 && index < iCount) {
				cInstrument = __cInstruments[index];
			}
			return cInstrument;
		}

		/// <summary>
		///   讀取資料請求結構列表內的 Instrument 資料
		/// </summary>
		/// <param name="requests">資料請求結構列表</param>
		/// <param name="useCache">是否使用快取(true=如果快取內有資料則從快取取得, false=不使用快取重新建立)</param>
		public void LoadDataRange(List<InstrumentDataRequest> requests, bool useCache = true) {
			int iCount = 0;
			lock (__cInstruments) {
				iCount = __cInstruments.Count;
				__cInstruments.AddRange(new Instrument[requests.Count]);
			}

			foreach (InstrumentDataRequest request in requests) {
				SeriesManager.Manager.SetQuoteService(request.DataFeed);
				SeriesManager.Manager.AsyncGetSeries(request, SeriesManager_onSeriesResult, useCache, iCount++);
			}
		}

		/// <summary>
		///   移除 Instrument 資料
		/// </summary>
		/// <param name="index">資料存放索引值(從 0 起始)</param>
		public void RemoveData(int index) {
			int iCount = __cInstruments.Count;
			if (iCount > 0 && index < iCount) {
				Instrument cInstrument = null;
				lock (__cInstruments) {
					cInstrument = __cInstruments[index];
					if (cInstrument != null) {
						cInstrument.Dispose();
					}

					__cInstruments.RemoveAt(index);
					--__iMaxInstrumentCount;
				}

				if (onRemoveInstrument != null) {
					onRemoveInstrument(this, new InstrumentChangeEvent(cInstrument, index));
				}
			}
		}

		/// <summary>
		///   設定最大 Bars 參考個數
		/// </summary>
		/// <param name="barsCount">Bars 參考個數</param>
		public void SetMaximumBarsReference(int barsCount) {
			__iMaxBarsReference = barsCount;
		}

		private void CreateInstrument(SeriesSymbolDataRand seriesSymbolDataRand, int dataIndex) {
			if (seriesSymbolDataRand != null) {
				if (logger.IsInfoEnabled) {
					InstrumentDataRequest cDataRequest = seriesSymbolDataRand.Source.DataRequest;
					logger.InfoFormat("[DataLoader.CreateInstrument] {0}:{1}{2} Instrument create completed...", cDataRequest.Symbol, cDataRequest.Resolution.Size, cDataRequest.Resolution.Type);
				}

				bool bCompleted = false;
				seriesSymbolDataRand.SetMaxbarsReferance(__iMaxBarsReference);
				Instrument cInstrument = new Instrument(seriesSymbolDataRand);
				
				lock (__cInstruments) {
					__cInstruments[dataIndex] = cInstrument;

					++__iMaxInstrumentCount;
					bCompleted = __iMaxInstrumentCount == __cInstruments.Count;
				}

				if (onAddInstrument != null) {
					onAddInstrument(this, new InstrumentChangeEvent(cInstrument, dataIndex));
				}

				if (bCompleted) {  //如果讀取資料都已經完成, 發出事件通知已經完成
					if (onLoadCompleted != null) {
						onLoadCompleted(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					onAddInstrument = null;
					onLoadCompleted = null;
					onRemoveInstrument = null;

					lock (__cInstruments) {
						foreach (Instrument cInstrument in __cInstruments) {
							if (cInstrument != null) {
								cInstrument.Dispose();
							}
						}
						__cInstruments.Clear();
					}
				}
			}
		}

		private void SeriesManager_onSeriesResult(object sender, SeriesResultEvent e) {
			CreateInstrument(e.Data, (int) e.Parameters);
		}
	}
} //235行