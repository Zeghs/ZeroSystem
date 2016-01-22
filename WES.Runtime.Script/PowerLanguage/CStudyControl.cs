using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Drawing;
using Zeghs.Services;
using Zeghs.Managers;

namespace PowerLanguage {
	/// <summary>
	///   基礎腳本控制類別(如果只有應用在圖表顯示則可以使用此類別)
	/// </summary>
	public class CStudyControl : IDisposable {
		private static readonly ILog logger = LogManager.GetLogger(typeof(CStudyControl));

		private struct DataStream {
			internal int index;
			internal InstrumentDataRequest data;
		}

		private bool __bBusy = false;      //忙碌旗標
		private bool __bDisposed = false;  //Dispose旗標
		private int __iTickCount = 1;
		private int __iWaitingCount = 0;
		private int __iMaxDataStream = 0;
		private int __iMaxBarsReference = 0;
		private TextContainer __cDrawTexts = null;
		private HashSet<string> __cDataSources = null;
		private List<Instrument> __cInstruments = null;
		private Dictionary<string, Queue<DataStream>> __cTemps = null;
		private object __oLock = new object();
		private object __oLockBars = new object();

		/// <summary>
		///   當即時報價資訊斷線時所觸發的事件
		/// </summary>
		public event EventHandler<QuoteDisconnectEvent> onDisconnected = null;

		/// <summary>
		///   當伺服器對時資訊送來所觸發的事件
		/// </summary>
		public event EventHandler<QuoteDateTimeEvent> onQuoteDateTime = null;

		/// <summary>
		///   當所有報價資訊都準備好時所觸發的事件
		/// </summary>
		public event EventHandler onReady = null;

		/// <summary>
		///   當有資料時會觸發這個更新的事件
		/// </summary>
		public event EventHandler onUpdate = null;

		/// <summary>
		///   [取得] IInstrument 資訊
		/// </summary>
		public IInstrument Bars {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 文字繪製容器
		/// </summary>
		public ITextContainer DrwText {
			get {
				return __cDrawTexts;
			}
		}

		/// <summary>
		///   [取得] 最大 IInstrument 資訊個數
		/// </summary>
		public int MaxDataStream {
			get {
				return __iMaxDataStream;
			}
		}

		internal List<Instrument> Instruments {
			get {
				return __cInstruments;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public CStudyControl() {
			__cDrawTexts = new TextContainer();
			__cDataSources = new HashSet<string>();
			__cInstruments = new List<Instrument>(16);
			__cTemps = new Dictionary<string, Queue<DataStream>>(16);
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   加入資料串流
		/// </summary>
		/// <param name="dataRequests">InstrumentDataRequest 列表</param>
		public void AddDataStreams(List<InstrumentDataRequest> dataRequests) {
			lock (__oLockBars) {
				int iCount = __cInstruments.Count;
				__cInstruments.AddRange(new Instrument[dataRequests.Count]);

				foreach (InstrumentDataRequest dataRequest in dataRequests) {
					AddDataStream(new DataStream() { index = iCount++, data = dataRequest });
				}
			}
		}

		/// <summary>
		///   移除資料串流
		/// </summary>
		/// <param name="data_stream">資料串流編號(0 為主要依據不能移除)</param>
		public void RemoveDataStream(int data_stream) {
			if (__cTemps.Count == 0 && data_stream > 0 && data_stream < __cInstruments.Count) {
				Instrument cInstrument = __cInstruments[data_stream];
				if (cInstrument != null) {
					cInstrument.Dispose();
				}

				lock (__oLockBars) {
					__cInstruments.RemoveAt(data_stream);
					--__iMaxDataStream;
				}
			}
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		internal virtual void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					onReady = null;
					onUpdate = null;
					onDisconnected = null;
					onQuoteDateTime = null;

					__cDrawTexts.Clear();
					DisposeResources();
					if (logger.IsInfoEnabled) logger.Info("[CStudyControl.Dispose] CStudyControl Disposed...");
				}
			}
		}

		internal void SetMaximumBarsReference(int barsCount) {
			__iMaxBarsReference = barsCount;
		}

		/// <summary>
		///   當有資料時會觸發這個更新的事件
		/// </summary>
		internal virtual void OnUpdate() {
			if (onUpdate != null) {
				onUpdate(this, EventArgs.Empty);
			}
		}

		/// <summary>
		///   啟動腳本
		/// </summary>
		internal virtual void Start() {
			Instrument cInstrument = __cInstruments[0];
			string sDataSource = cInstrument.Request.DataFeed;

			AsyncCalculate();  //啟動的時候先計算一次(因為使用者不一定會使用即時報價來源, 如果不先計算沒有報價源就不會啟動 CalcBar 方法)

			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService != null) {
				cService.onQuote += QuoteService_onQuote;
				cService.onQuoteDateTime += QuoteService_onQuoteDateTime;
			}
			if (logger.IsInfoEnabled) logger.Info("[CStudyControl.Start] Script running and calculation...");
		}

		/// <summary>
		///   由伺服器報價時間觸發的引動事件
		/// </summary>
		/// <param name="e">伺服器報價時間資訊</param>
		protected virtual void OnQuoteDateTime(QuoteDateTimeEvent e) {
			if (onQuoteDateTime != null) {
				onQuoteDateTime(this, e);
			}
		}

		private void AddDataStream(DataStream dataStream) {
			string sDataSource = dataStream.data.DataFeed;
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService == null) {
				CreateInstrument(dataStream);
				CheckReady();
			} else {
				bool bLogin = cService.IsLogin;
				lock (__cDataSources) {
					if (!__cDataSources.Contains(sDataSource)) {
						cService.onDisconnected += QuoteService_onDisconnected;
						if (!bLogin) {  //如果尚未登入完成
							cService.onLoginCompleted += QuoteService_onLoginCompleted;  //掛上登入完成事件
						}
						SeriesManager.Manager.SetQuoteService(cService);  //設定報價服務給股票資訊管理員(這樣 Bars 才會合併即時報價)

						__cDataSources.Add(sDataSource);
					}
				}

				if (bLogin) {
					string sSymbolId = dataStream.data.Symbol;
					IQuote cQuote = cService.Storage.GetQuote(sSymbolId);
					if (cQuote != null && cQuote.ComplementStatus != ComplementStatus.Complemented) {
						lock (__oLock) {
							if (__iWaitingCount == 0) {
								cService.onComplementCompleted += QuoteService_onComplementCompleted;
							}
							++__iWaitingCount;

							Queue<DataStream> cQueue = GetTemps(sSymbolId);
							cQueue.Enqueue(dataStream);

							if (cQuote.ComplementStatus == ComplementStatus.NotComplement) {
								cService.AddSubscribe(sSymbolId);
								cService.Complement(sSymbolId);
							}
						}
					} else {
						CreateInstrument(dataStream);
						CheckReady();
					}
				} else {  //如果還沒有登入完成
					bool bReAddData = false;
					lock (__oLock) {  //進行同步(因為登入完成的通知是非同步, 所以要同步一些步驟才不會出現資料沒有載入的問題)
						if (cService.IsLogin) {  //再檢查一次是否已經登入成功
							lock (__cTemps) {
								bReAddData = !__cTemps.ContainsKey(sDataSource);  //檢查是否還有資料(如果沒資料表示已經執行過 onLoginCompleted 事件)
							}
						} else {  //如果還沒有登入完成
							Queue<DataStream> cQueue = GetTemps(sDataSource);  //取得資料
							cQueue.Enqueue(dataStream);  //保存至 Queue
						}
					}

					if (bReAddData) {  //是否要重新加入資料
						AddDataStream(dataStream);  //重新加入一次資料
					}
				}
			}
		}

		private void AsyncCalculate() {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bBusy;
				if (!bBusy) {
					__bBusy = true;
				}
			}

			if (!bBusy) {
				Task.Factory.StartNew(() => {
					Instrument cBaseInstrument = __cInstruments[0];
					while (__iTickCount > 0) {
						Interlocked.Decrement(ref __iTickCount);

						do {
							DateTime cTime = cBaseInstrument.Time[0];
							lock (__oLockBars) {
								int iCount = __cInstruments.Count;
								Parallel.For(0, iCount, (i) => {
									Instrument cInstrument = __cInstruments[i];
									
									if (cInstrument != null) {
										cInstrument.MoveBars(cTime);
									}
								});
							}
							
							OnUpdate();
						} while (cBaseInstrument.Next());
					}

					lock (__oLock) {
						__bBusy = false;
					}
				});
			}
		}

		private void CheckReady() {
			bool bReady = false;
			lock (__oLockBars) {
				if (__iMaxDataStream == __cInstruments.Count) {
					if (this.Bars == null) {
						this.Bars = __cInstruments[0];
						bReady = true;
					}
				}
			}

			if (bReady) {
				if (logger.IsInfoEnabled) logger.Info("[CStudyControl.CheckReady] Script initialized and ready...");
				if (onReady != null) {
					onReady(this, EventArgs.Empty);
				}
				
				this.Start();
			}
		}

		private void CreateInstrument(DataStream dataStream) {
			SeriesSymbolData cSeries = SeriesManager.Manager.GetSeries(dataStream.data);
			if (cSeries != null) {
				lock (__oLockBars) {
					int iIndex = dataStream.index;
					Instrument cInstrument = new Instrument(cSeries, (iIndex == 0) ? __iMaxBarsReference : 0);
					__cInstruments[iIndex] = cInstrument;
					
					++__iMaxDataStream;
				}
				if (logger.IsInfoEnabled) logger.InfoFormat("[CStudyControl.CreateInstrument] {0}:{1}{2} Instrument create completed...", dataStream.data.Symbol, dataStream.data.Resolution.Size, dataStream.data.Resolution.Type);
			}
		}

		private void DisposeResources() {
			lock (__cDataSources) {
				foreach (string sDataSource in __cDataSources) {
					AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
					if (cService != null) {
						cService.onQuote -= QuoteService_onQuote;
						cService.onDisconnected -= QuoteService_onDisconnected;
						cService.onQuoteDateTime -= QuoteService_onQuoteDateTime;
						cService.onLoginCompleted -= QuoteService_onLoginCompleted;
						cService.onComplementCompleted -= QuoteService_onComplementCompleted;
					}
				}
				__cDataSources.Clear();
			}

			lock (__oLockBars) {
				if (__cInstruments != null) {
					foreach (Instrument cInstrument in __cInstruments) {
						if (cInstrument != null) {
							cInstrument.Dispose();
						}
					}
					__cInstruments.Clear();
				}
			}
		}

		private Queue<DataStream> GetTemps(string key, bool isRemove = false) {
			Queue<DataStream> cQueue = null;
			lock (__cTemps) {
				if (__cTemps.TryGetValue(key, out cQueue)) {
					if (isRemove) {
						__cTemps.Remove(key);
					}
				} else {
					if (!isRemove) {
						cQueue = new Queue<DataStream>(16);
						__cTemps.Add(key, cQueue);
					}
				}
			}
			return cQueue;
		}

		private void QuoteService_onComplementCompleted(object sender, QuoteComplementCompletedEvent e) {
			AbstractQuoteService cService = sender as AbstractQuoteService;

			string sSymbolId = e.SymbolId;
			Queue<DataStream> cQueue = null;
			lock (__oLock) {
				cQueue = GetTemps(sSymbolId, true);
				if (cQueue != null) {
					__iWaitingCount -= cQueue.Count;
					if (__iWaitingCount == 0) {
						cService.onComplementCompleted -= QuoteService_onComplementCompleted;
					}
				}
			}
			if (logger.IsInfoEnabled) logger.InfoFormat("[CStudyControl.onComplementCompleted] {0}:{1} complement completed...", e.DataSource, sSymbolId);

			if (cQueue != null) {
				while (cQueue.Count > 0) {
					CreateInstrument(cQueue.Dequeue());
				}
				
				CheckReady();
			}
		}

		private void QuoteService_onDisconnected(object sender, QuoteDisconnectEvent e) {
			if (onDisconnected != null) {
				onDisconnected(sender, e);
			}
		}

		private void QuoteService_onLoginCompleted(object sender, EventArgs e) {
			AbstractQuoteService cService = sender as AbstractQuoteService;
			cService.onLoginCompleted -= QuoteService_onLoginCompleted;

			Queue<DataStream> cQueue = null;
			lock (__oLock) {
				cQueue = GetTemps(cService.DataSource, true);
			}

			if (cQueue != null) {
				while (cQueue.Count > 0) {
					AddDataStream(cQueue.Dequeue());
				}
			}
		}

		private void QuoteService_onQuote(object sender, QuoteEvent e) {
			string sSymbolId = this.Bars.Request.Symbol;
			if (sSymbolId.Equals(e.Quote.SymbolId)) {
				Interlocked.Increment(ref __iTickCount);
				
				if (!__bBusy) {
					AsyncCalculate();
				}
			}
		}

		private void QuoteService_onQuoteDateTime(object sender, QuoteDateTimeEvent e) {
			OnQuoteDateTime(e);
		}
	}
}  //443行