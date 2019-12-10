using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Data {
	/// <summary>
	///   商品資料類別(存放開高低收量資訊)
	/// </summary>
	internal sealed class SeriesSymbolData : ISeriesSymbolData, IDisposable {
		private static int __iLastSeriesId = 0;  //存放最後一個 Series Id(此 Id 會一直遞增, 每次使用 new 產生此類別時就會自動遞增)
		private static void MergeSeries(SeriesSymbolData target, DateTime period, double open, double high, double low, double close, double volume, bool isNewBars, bool isRealtime) {
			if (isNewBars) {
				target.AddSeries(period, open, high, low, close, volume, isRealtime);
			} else {
				target.SetSeries(open, high, low, close, volume, isRealtime);
			}
		}

		internal event EventHandler<DataRequestEvent> onRequest = null;
		internal event EventHandler onRequestCompleted = null;
		internal event EventHandler onReset = null;

		private double __dOVolume = 0;
		private int __iRealtimeCount = 0;
		private bool __bDisposed = false;
		private DateTime __cUpdateTime;
		private Series<double> __cLows = null;
		private Series<double> __cHighs = null;
		private Series<double> __cOpens = null;
		private Series<double> __cCloses = null;
		private Series<double> __cVolumes = null;
		private Series<DateTime> __cTimes = null;
		private Queue<DateTime> __cTimeQueue = null;
		private InstrumentSettings __cSettings = null;
		private InstrumentDataRequest __cDataRequest;
		private object __oLock = new object();
		private object __oLockRequest = new object();

		/// <summary>
		///   [取得] 收盤價陣列資訊
		/// </summary>
		public ISeries<double> Close {
			get {
				return __cCloses;
			}
		}

		/// <summary>
		///   [取得] 最高價陣列資訊
		/// </summary>
		public ISeries<double> High {
			get {
				return __cHighs;
			}
		}

		/// <summary>
		///   [取得] 最低價陣列資訊
		/// </summary>
		public ISeries<double> Low {
			get {
				return __cLows;
			}
		}

		/// <summary>
		///   [取得] 開盤價陣列資訊
		/// </summary>
		public ISeries<double> Open {
			get {
				return __cOpens;
			}
		}

		/// <summary>
		///   [取得] 日期時間陣列資訊
		/// </summary>
		public ISeries<DateTime> Time {
			get {
				return __cTimes;
			}
		}

		/// <summary>
		///   [取得] 成交量陣列資訊
		/// </summary>
		public ISeries<double> Volume {
			get {
				return __cVolumes;
			}
		}

		internal InstrumentDataRequest DataRequest {
			get {
				return __cDataRequest;
			}
		}

		internal int Id {
			get;
			set;
		}

		internal SeriesIndexer Indexer {
			get;
			private set;
		}

		internal bool Initialized {
			get;
			private set;
		}

		internal DateTime LastBarTime {
			get {
				return __cTimes[Indexer.RealtimeIndex];
			}
		}

		internal int RealtimeCount {
			get {
				return __iRealtimeCount;
			}
		}

		internal InstrumentSettings Settings {
			get {
				return __cSettings;
			}
		}

		internal DateTime UpdateTime {
			get {
				return __cUpdateTime;
			}
		}

		internal SeriesSymbolData(InstrumentDataRequest dataRequest, InstrumentSettings settings = null) {
			this.Id = 0x40000000 | System.Threading.Interlocked.Increment(ref __iLastSeriesId);
			this.Indexer = new SeriesIndexer();
			__cDataRequest = dataRequest;
			__cSettings = ((settings == null) ? new InstrumentSettings(ref __cDataRequest) : settings.Create(ref __cDataRequest));
			__cUpdateTime = DateTime.UtcNow.AddHours(__cSettings.TimeZone);
			
			SessionObject cSession = __cSettings.GetSessionFromToday();
			__iRealtimeCount = (cSession == null) ? 1 : (int) ((cSession.GetCloseTimeForDaylight() - cSession.GetStartTimeForDaylight()).TotalSeconds / dataRequest.Resolution.TotalSeconds + 1);

			Indexer.HistoryIndex = 0;
			Indexer.RealtimeIndex = -1;

			__cOpens = new Series<double>(__iRealtimeCount);
			__cOpens.onRequest += Series_onRequest;

			__cHighs = new Series<double>(__iRealtimeCount);
			__cHighs.onRequest += Series_onRequest;

			__cLows = new Series<double>(__iRealtimeCount);
			__cLows.onRequest += Series_onRequest;

			__cCloses = new Series<double>(__iRealtimeCount);
			__cCloses.onRequest += Series_onRequest;

			__cTimes = new Series<DateTime>(__iRealtimeCount);
			__cTimes.onRequest += Series_onRequest;

			__cVolumes = new Series<double>(__iRealtimeCount);
			__cVolumes.onRequest += Series_onRequest;

			__cDataRequest.Range.Count = 0;  //將資料筆數設定為0(因為一開始沒有請求任何資訊)
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void AddSeries(DateTime time, double open, double high, double low, double close, double volume, bool isRealtime = true) {
			lock (__oLock) {
				int iCount = __cTimes.Count;
				int iIndex = (isRealtime) ? Indexer.RealtimeIndex + 1 : Indexer.HistoryIndex - 1;
				if (iIndex < 0 || iIndex == iCount) {
					iIndex = AdjustSize(4, !isRealtime);
					iIndex = (isRealtime) ? ++iIndex : --iIndex;
				}

				__cTimes.SetData(iIndex, time);
				__cOpens.SetData(iIndex, open);
				__cHighs.SetData(iIndex, high);
				__cLows.SetData(iIndex, low);
				__cCloses.SetData(iIndex, close);
				__cVolumes.SetData(iIndex, volume);

				if (isRealtime) {
					++Indexer.RealtimeIndex;
				} else {
					--Indexer.HistoryIndex;
				}
			}
		}

		internal int AdjustSize(int count, bool isInsert = false) {
			int iIndex = 0;
			lock (__oLock) {
				__cTimes.AdjustSize(count, isInsert);
				__cOpens.AdjustSize(count, isInsert);
				__cHighs.AdjustSize(count, isInsert);
				__cLows.AdjustSize(count, isInsert);
				__cCloses.AdjustSize(count, isInsert);
				__cVolumes.AdjustSize(count, isInsert);

				if (isInsert) {  //如果是往前插入空間需要調整索引直
					Indexer.AdjustIndex(count);
				}
				iIndex = (isInsert) ? Indexer.HistoryIndex : Indexer.RealtimeIndex;
			}
			return iIndex;
		}

		internal void Clone(out Series<DateTime> time, out Series<double> open, out Series<double> high, out Series<double> low, out Series<double> close, out Series<double> volume) {
			time = __cTimes.Clone();
			open = __cOpens.Clone();
			high = __cHighs.Clone();
			low = __cLows.Clone();
			close = __cCloses.Clone();
			volume = __cVolumes.Clone();
		}

		internal SeriesSymbolData CreateSeries(InstrumentDataRequest dataRequest) {
			return new SeriesSymbolData(dataRequest, __cSettings);
		}

		internal void Merge(SeriesSymbolData target) {
			int iTargetCount = target.Indexer.Count;
			int iFirstIndex = target.Indexer.GetBaseIndex(__cDataRequest.Range.Count);

			DateTime cFrom = __cTimes[Indexer.HistoryIndex];
			DateTime cTo = __cTimes[iFirstIndex];
			List<DateTime> cPeriods = target.__cDataRequest.Resolution.CalculatePeriods(cFrom, cTo);

			for (int i = iFirstIndex; i >= Indexer.HistoryIndex; i--) {
				DateTime cBaseTime = __cTimes[i];
				bool bNewBars = Resolution.GetNearestPeriod(cPeriods, ref cBaseTime);
				MergeSeries(target, cBaseTime, __cOpens[i], __cHighs[i], __cLows[i], __cCloses[i], __cVolumes[i], bNewBars, false);

				if (bNewBars) {
					target.Indexer.SetBaseIndex(i);
				}
			}

			if (iTargetCount == 0) {
				target.__cDataRequest.Range.To = __cDataRequest.Range.To;
				Queue<DateTime> cQueue = target.CreateRealtimePeriods();
				for (int i = __cDataRequest.Range.Count; i <= Indexer.RealtimeIndex; i++) {
					DateTime cBaseTime = __cTimes[i];
					bool bCreate = Resolution.GetNearestPeriod(cQueue, ref cBaseTime);
					MergeSeries(target, cBaseTime, __cOpens[i], __cHighs[i], __cLows[i], __cCloses[i], __cVolumes[i], bCreate, true);
				}
			}

			target.__dOVolume = __dOVolume;        //更新最後參考總量(只要低於最後總量的 tick 都不會再被合併進去 Bars)
			target.__cUpdateTime = __cUpdateTime;  //更新目標資訊的最後更新時間(最後更新時間會牽涉到 Bars 的狀態 Close or Inside)
			target.Initialized = true;
		}

		internal void Merge(ITick tick) {
			double dVolume = tick.Volume;
			if (dVolume > __dOVolume) {
				double dPrice = tick.Price;
				double dSingle = dVolume - __dOVolume;  //重新計算準確的單量(即時系統送來的單量並不準確, 所以以總量為標準依據)
				if (dPrice > 0 && dSingle > 0) {
					DateTime cBaseTime = tick.Time;
					if (__cTimeQueue != null) {
						bool bNewBars = Resolution.GetNearestPeriod(__cTimeQueue, ref cBaseTime);
						MergeSeries(this, cBaseTime, dPrice, dPrice, dPrice, dPrice, dSingle, bNewBars, true);
					} else {
						MergeSeries(this, cBaseTime, dPrice, dPrice, dPrice, dPrice, dSingle, false, true);
					}
				}

				__dOVolume = dVolume;
				__cUpdateTime = tick.Time;
			}
		}

		internal void MergeTicks() {
			string sDataSource = __cDataRequest.DataFeed;
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService != null) {
				if (cService.TradeDate > LastBarTime.Date) {  //如果即時報價服務交易日期大於最後 Bars 交易日期, 就合併今日即時資訊
					this.Initialized = false;
					__cDataRequest.Range.To = cService.TradeDate;
					Indexer.RealtimeIndex = __cCloses.Count - __iRealtimeCount - 1;
					this.CreateRealtimePeriods();

					string sSymbolId = __cDataRequest.Symbol;
					IQuote cQuote = cService.Storage.GetQuote(sSymbolId);
					if (cQuote != null) {
						int iTickCount = cQuote.TickCount;
						for (int i = iTickCount - 1; i >= 0; i--) {
							Merge(cQuote.GetTick(i));
						}
					}
				} else {  //盤後分檔與日檔資料已經公布, 不用在合併今日即時資訊
					string sSymbolId = __cDataRequest.Symbol;
					IQuote cQuote = cService.Storage.GetQuote(sSymbolId);
					if (cQuote != null) {
						__dOVolume = cQuote.RealTick.Volume;  //取得今日即時資訊的總成交量(當 tick 來時會比對tick 總成交量是否比此變數大, 如果比較大就合併到 Bars 內, 所以如果今日已有分日檔資訊, 此欄位需填入今日即時資訊總成交量, 才不會被重複合併到最後一根 Bars)
					}
				}
			}
			this.Initialized = true;
		}

		internal void OnRequest(DataRequestEvent e) {
			if (onRequest != null) {
				onRequest(this, e);

				if (e.Result == 0) {
					__cDataRequest.Range.Count = e.Count;
					__cDataRequest.Range.From = e.Ranges[0];
					__cDataRequest.Range.IsAlreadyRequestAllData = e.IsAlreadyRequestAllData;

					if (!this.Initialized) {  //尚未初始化之前才需要更新(初始化之後已經併入即時報價資訊, 更新時間會是目前最新的報價時間)
						__cUpdateTime = __cTimes[this.Indexer.RealtimeIndex];  //更新最後的更新時間(如果沒有即時報價資訊, 這個就是最後一根 Bars 的時間)
					}

					if (onRequestCompleted != null) {
						onRequestCompleted(this, EventArgs.Empty);
					}
				} else {
					RemoveRequest();  //請求失敗就移除請求事件(表示可能伺服器有問題或沒有歷史報價資訊無法執行請求服務)
				}
			}
		}

		internal void RemoveRequest() {
			onRequest = null;
			onRequestCompleted = null;

			__cTimes.RemoveRequest();
			__cOpens.RemoveRequest();
			__cHighs.RemoveRequest();
			__cLows.RemoveRequest();
			__cCloses.RemoveRequest();
			__cVolumes.RemoveRequest();
		}

		internal void Reset() {
			string sDataSource = __cDataRequest.DataFeed;
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService != null) {
				__dOVolume = 0;
				__cDataRequest.Range.To = cService.TradeDate;
				__cDataRequest.Range.Count = this.Indexer.Count;

				int iCount = __iRealtimeCount - (__cCloses.Count - Indexer.RealtimeIndex - 1);  //計算即時報價資訊所需要的 Bars 個數量(調整好等候合併即時報價)
				if (iCount > 0) {
					this.AdjustSize(iCount);  //調整即時資訊所需要的空間
				}
				CreateRealtimePeriods();  //建立即時資訊所需要的報價周期

				if (onReset != null) {
					onReset(this, EventArgs.Empty);  //發送清盤重置事件
				}
			}
		}

		private Queue<DateTime> CreateRealtimePeriods() {
			DateTime cToday = DateTime.UtcNow.AddHours(__cSettings.TimeZone);
			__cTimeQueue = __cDataRequest.Resolution.CalculateRealtimePeriods(this.LastBarTime, cToday, __cSettings.Expiration);
			return __cTimeQueue;
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onReset = null;
					onRequest = null;
					onRequestCompleted = null;
					
					__cTimes.Dispose();
					__cOpens.Dispose();
					__cHighs.Dispose();
					__cLows.Dispose();
					__cCloses.Dispose();
					__cVolumes.Dispose();
				}
			}
		}

		private void SetSeries(double open, double high, double low, double close, double volume, bool isRealtime = true) {
			lock (__oLock) {
				int iIndex = (isRealtime) ? Indexer.RealtimeIndex : Indexer.HistoryIndex;
				if (isRealtime) {
					__cCloses.SetData(iIndex, close);
				} else {
					__cOpens.SetData(iIndex, open);
				}

				if (high > __cHighs[iIndex]) {
					__cHighs.SetData(iIndex, high);
				}

				if (low < __cLows[iIndex]) {
					__cLows.SetData(iIndex, low);
				}
				__cVolumes.SetData(iIndex, __cVolumes[iIndex] + volume);
			}
		}

		private void Series_onRequest(object sender, SeriesRequestEvent e) {  //此方法由 Open, Close, Time... 這些序列資料類別作綁定, 當使用者需要往前請求歷史資料時會觸發這個事件方法
			int iRequestCount = ~e.Position + 1;  //e.Position為負值(往前100根 Bar = -100), 使用補數運算變成 100 作為請求 Bars 資料個數
			int iTotals = iRequestCount + __cDataRequest.Range.Count;  //資料總個數(欲請求個數 + 目前已下載完後的資料個數)

			lock (__oLockRequest) {  //鎖定資源(請求序列資料時讓請求執行緒同步處理)
				OnRequest(new DataRequestEvent(iRequestCount, iTotals, __cDataRequest.Resolution.Rate));  //呼叫請求方法
			}
		}
	}
}  //432行