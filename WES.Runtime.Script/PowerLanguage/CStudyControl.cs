using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Zeghs.IO;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Services;
using Zeghs.Managers;

namespace PowerLanguage {
	/// <summary>
	///   基礎腳本控制類別(如果只有應用在圖表顯示則可以使用此類別)
	/// </summary>
	public class CStudyControl : IDisposable {
		private static readonly ILog logger = LogManager.GetLogger(typeof(CStudyControl));

		private bool __bBusy = false;      //忙碌旗標
		private bool __bDisposed = false;  //Dispose旗標
		private int __iTickCount = 1;
		private DataLoader __cDataLoader = null;
		private AbstractQuoteService __cQuoteService = null;
		private object __oLock = new object();

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
		///   [取得] 最大 IInstrument 資訊個數
		/// </summary>
		public int MaxDataStream {
			get {
				return __cDataLoader.MaxInstrumentCount;
			}
		}

		/// <summary>
		///   [取得] 商品資訊資料讀取者
		/// </summary>
		protected IDataLoader DataLoader {
			get {
				return __cDataLoader;
			}
		}

		internal List<Instrument> Instruments {
			get {
				return __cDataLoader.Instruments;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public CStudyControl() {
			__cDataLoader = new DataLoader();
			__cDataLoader.onLoadCompleted += DataLoader_onLoadCompleted;
		}

		/// <summary>
		///   加入資料串流
		/// </summary>
		/// <param name="instrument">IInstrument 商品資訊介面</param>
		/// <returns>回傳值: 資料串流編號</returns>
		public int AddDataStream(IInstrument instrument) {
			return __cDataLoader.AddData(instrument as Instrument) + 1;
		}

		/// <summary>
		///   加入資料串流
		/// </summary>
		/// <param name="dataRequests">InstrumentDataRequest 列表</param>
		public void AddDataStreams(List<InstrumentDataRequest> dataRequests) {
			__cDataLoader.LoadDataRange(dataRequests);
		}

		/// <summary>
		///   連結即時報價資訊源
		/// </summary>
		public void ConnectQuoteServer() {
			if (this.Bars != null) {
				string sDataSource = this.Bars.Request.DataFeed;
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
				if (cService != null && cService != __cQuoteService) {
					__cQuoteService = cService;
					__cQuoteService.AddSubscribe(this.Bars.Request.Symbol);

					__cQuoteService.onQuote += QuoteService_onQuote;
					__cQuoteService.onQuoteDateTime += QuoteService_onQuoteDateTime;
				}
			}
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   移除資料串流
		/// </summary>
		/// <param name="data_stream">資料串流編號(1 為主要依據不能移除)</param>
		public void RemoveDataStream(int data_stream) {
			if (data_stream > 1 && data_stream <= __cDataLoader.MaxInstrumentCount) {
				__cDataLoader.RemoveData(data_stream - 1);
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

					DisposeResources();
					if (logger.IsInfoEnabled) logger.Info("[CStudyControl.Dispose] CStudyControl Disposed...");
				}
			}
		}

		internal void SetMaximumBarsReference(int barsCount) {
			__cDataLoader.SetMaximumBarsReference(barsCount);
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
			RunMoveBarsAndUpdate(this.Bars.Time.Value);  //先觸發一次 CalcBar 方法(因為 Bars.CurrentBar 從 1 開始, 如果直接執行 AsyncCalculate 方法, 會直接從第 2 根 Bars 開始進入 CalcBar 方法, 少算了第 1 根 Bars)
			AsyncCalculate();  //啟動的時候先計算一次(因為使用者不一定會使用即時報價來源, 如果不先計算沒有報價源就不會啟動 CalcBar 方法)
			
			this.ConnectQuoteServer();  //連結即時報價資訊源
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
					Instrument cBaseInstrument = __cDataLoader.GetInstrument(0);  //取得第一個 Instrument(之後的 Instrument 資料都會跟隨這個同步移動 Bars)
					if (cBaseInstrument != null) {
						while (__iTickCount > 0) {
							Interlocked.Decrement(ref __iTickCount);  //每次遞減一次(當 TickCount 遞減完畢則表示已經計算完所有的 Tick)

							bool bNext = true;
							while (bNext) {
								bNext = cBaseInstrument.Next();  //往下一根 Bars 移動
								RunMoveBarsAndUpdate(cBaseInstrument.Time.Value);  //讀取移動後的 Bars 時間
							}
						}
					}

					lock (__oLock) {
						__bBusy = false;
					}
				});
			}
		}

		private void DisposeResources() {
			if (__cQuoteService != null) {
				__cQuoteService.onQuote -= QuoteService_onQuote;
				__cQuoteService.onDisconnected -= QuoteService_onDisconnected;
				__cQuoteService.onQuoteDateTime -= QuoteService_onQuoteDateTime;
				__cQuoteService = null;
			}
			__cDataLoader.Dispose();  //釋放資料讀取者資源
		}

		private void RunMoveBarsAndUpdate(DateTime time) {
			int iCount = __cDataLoader.MaxInstrumentCount;  //取得目前資料讀取者的所有 Instrument 個數
			Parallel.For(1, iCount, (i) => {
				Instrument cInstrument = __cDataLoader.GetInstrument(i);

				if (cInstrument != null) {
					cInstrument.MoveBars(time);  //開始同步 Instrument 的時間週期(會將 Instrument 的 Bars 移動到與參數 time 時間最接近的那一根 Bars 上)
				}
			});
			OnUpdate();  //觸發更新事件(如果是繼承 CStudyAbstract 的物件則會觸發 CalcBar 方法)
		}

		private void DataLoader_onLoadCompleted(object sender, EventArgs e) {
			if (this.Bars == null) {
				this.Bars = __cDataLoader.GetInstrument(0);

				if (logger.IsInfoEnabled) logger.Info("[CStudyControl.onLoadCompleted] Script initialized and ready...");
				if (onReady != null) {
					onReady(this, EventArgs.Empty);
				}

				this.Start();

				__cDataLoader.onLoadCompleted -= DataLoader_onLoadCompleted;
			}
		}

		private void QuoteService_onDisconnected(object sender, QuoteDisconnectEvent e) {
			if (onDisconnected != null) {
				onDisconnected(sender, e);
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
}  //280行