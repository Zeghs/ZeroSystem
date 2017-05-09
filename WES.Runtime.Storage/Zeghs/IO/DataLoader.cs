using System;
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
		///   非同步讀取資料請求結構內的 Instrument 資料
		/// </summary>
		/// <param name="request">資料請求結構</param>
		/// <param name="result">當成功取得 Instrument 商品資料會使用此委派方法回傳資料</param>
		/// <param name="args">使用者自訂參數</param>
		public void BeginLoadData(InstrumentDataRequest request, LoadDataCallback result, object args = null) {
			SeriesManager.Manager.SetQuoteService(request.DataFeed);
			SeriesManager.Manager.AsyncGetSeries(request, (object sender, SeriesResultEvent e) => {
				Instrument cInstrument = new Instrument(e.Data, 0);  //建立 Instrument 商品資料
				Instrument cBars_0 = GetInstrument(0);  //取得目前第 0 個 Instrument 商品資料
				if (cBars_0 != null) {
					cInstrument.MoveBars(cBars_0.LastBarTime);
				}

				IQuote cQuote = null;
				InstrumentDataRequest cRequest = cInstrument.Request;
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(cRequest.DataFeed);
				if (cService != null) {
					cQuote = cService.Storage.GetQuote(cRequest.Symbol);
				}
				result(new DataLoaderResult(cInstrument, cQuote, args));
			}, false);
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
		public void LoadDataRange(List<InstrumentDataRequest> requests) {
			int iCount = 0;
			lock (__cInstruments) {
				iCount = __cInstruments.Count;
				__cInstruments.AddRange(new Instrument[requests.Count]);
			}

			foreach (InstrumentDataRequest request in requests) {
				SeriesManager.Manager.SetQuoteService(request.DataFeed);
				SeriesManager.Manager.AsyncGetSeries(request, SeriesManager_onSeriesResult, true, iCount++);
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

		private void CreateInstrument(SeriesSymbolData series, int dataIndex) {
			if (series != null) {
				if (logger.IsInfoEnabled) {
					InstrumentDataRequest cDataRequest = series.DataRequest;
					logger.InfoFormat("[DataLoader.CreateInstrument] {0}:{1}{2} Instrument create completed...", cDataRequest.Symbol, cDataRequest.Resolution.Size, cDataRequest.Resolution.Type);
				}

				bool bCompleted = false;
				Instrument cInstrument = new Instrument(series, (dataIndex == 0) ? __iMaxBarsReference : 0);
				
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
} //215行