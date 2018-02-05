using System;
using PowerLanguage;
using Zeghs.Managers;
using Zeghs.Services;

namespace Zeghs.Data {
	/// <summary>
	///   隨機商品序列資料類別(提供給 Instrument 類別作來源資料使用)
	/// </summary>
	public sealed class SeriesSymbolDataRand : ISeriesSymbolDataRand, IDisposable {
		private bool __bDisposed = false;
		private int __iCurrentBar = 1;
		private int __iHistoryIndex = 0;
		private int __iBaseAdjustTotal = 0;
		private Series<double> __cLows = null;
		private Series<double> __cHighs = null;
		private Series<double> __cOpens = null;
		private Series<double> __cCloses = null;
		private Series<double> __cVolumes = null;
		private Series<DateTime> __cTimes = null;
		private SeriesIndexer __cIndexer = null;
		private SeriesSymbolData __cSource = null;
		private IQuoteStorage __cQuoteStorage = null;

		/// <summary>
		///   [取得] 收盤價陣列資訊
		/// </summary>
		public ISeries<double> Close {
			get {
				return __cCloses;
			}
		}

		/// <summary>
		///   [取得] 序列資料總個數(歷史序列資料+今日即時序列資料)
		/// </summary>
		public int Count {
			get {
				return __cIndexer.Count;
			}
		}

		/// <summary>
		///   [取得] 目前 Bars 索引值(索引從 1 開始)
		/// </summary>
		public int Current {
			get {
				return __iHistoryIndex + (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal) + __iCurrentBar;
			}

			internal set {
				__iCurrentBar = value;
				int iCurrent = __iHistoryIndex + (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal) + __iCurrentBar;

				__cOpens.Current = iCurrent;
				__cHighs.Current = iCurrent;
				__cLows.Current = iCurrent;
				__cCloses.Current = iCurrent;
				__cTimes.Current = iCurrent;
				__cVolumes.Current = iCurrent;
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

		/// <summary>
		///   [取得] Bars 資料總個數
		/// </summary>
		internal int BarsCount {
			get {
				return __cIndexer.RealtimeIndex - (__iHistoryIndex + (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal)) + 1;
			}
		}

		/// <summary>
		///   [取得] Bars 資料容器大小(包含預留的即時資料空間)
		/// </summary>
		internal int BarsSize {
			get {
				return __cCloses.Count - (__iHistoryIndex + (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal));
			}
		}

		/// <summary>
		///   [取得] 委託價量資訊
		/// </summary>
		internal IDOMData DOM {
			get {
				if (__cQuoteStorage != null) {
					string sSymbolId = __cSource.DataRequest.Symbol;
					IQuote cQuote = __cQuoteStorage.GetQuote(sSymbolId);
					if (cQuote != null) {
						return cQuote.DOM;
					}
				}
				return null;
			}
		}

		internal bool IsLastBars {
			get {
				int iIndex = this.Current - 1;
				return iIndex == __cIndexer.RealtimeIndex;
			}
		}

		/// <summary>
		///   [取得] 來源 SeriesSymbolData 資料(此來源資料為)
		/// </summary>
		internal SeriesSymbolData Source {
			get {
				return __cSource;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="source">SeriesSymbolData 商品資料類別</param>
		/// <param name="request">資料請求結構</param>
		internal SeriesSymbolDataRand(SeriesSymbolData source, InstrumentDataRequest request) {
			__cSource = source;
			__cSource.onRequestCompleted += SeriesSymbolData_onRequestCompleted;  //附掛請求歷史資料完成的事件通知

			__cIndexer = source.Indexer;
			__iBaseAdjustTotal = __cIndexer.AdjustTotalCount;
			source.Clone(out __cTimes, out __cOpens, out __cHighs, out __cLows, out __cCloses, out __cVolumes);

			__iHistoryIndex = __cCloses.Count - (request.Range.Count + source.RealtimeCount);
			this.Current = 1;  //預設值索引從 1 開始(內部會自動計算對應至 SeriesSymbolData 序列資料的正確索引位置)

			string sDataSource = source.DataRequest.DataFeed;
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService != null) {
				__cQuoteStorage = cService.Storage;
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
		///   設定最大 Bars count 參考個數
		/// </summary>
		/// <param name="maxbarsReferance">最大 bars count 參考個數</param>
		public void SetMaxbarsReferance(int maxbarsReferance) {
			if (maxbarsReferance > 0) {
				int iHistoryIndex = __cCloses.Count - (maxbarsReferance + __cSource.RealtimeCount);
				__iHistoryIndex = (iHistoryIndex < 0) ? __iHistoryIndex : iHistoryIndex;
			}
		}

		internal int TryOutside() {
			int iRet = 0, iIndex = this.Current - 1;
			if (iIndex > __cIndexer.RealtimeIndex) {  //如果目前索引已經超過即時資料索引
				iRet = this.BarsCount;  //傳回目前 Bars 總數量
			}
			return iRet;
		}
		
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					__cSource.onRequestCompleted -= SeriesSymbolData_onRequestCompleted;  //卸載請求歷史資料完成的事件通知

					__cTimes.Dispose();
					__cOpens.Dispose();
					__cHighs.Dispose();
					__cLows.Dispose();
					__cCloses.Dispose();
					__cVolumes.Dispose();
				}
			}
		}

		private void SeriesSymbolData_onRequestCompleted(object sender, EventArgs e) {
			//因為所有的 SeriesSymbolDataRand 都共用同一個 SeriesSymbolData 來源資訊, 所以當請求歷史資料片段完成後, 需要修正 Current 索引, 這樣才能正確對應至 SeriesSymbolData 內的序列資料位置
			this.Current = __iCurrentBar;  //如果沒有使用此事件通知, 則需要有新的即時資訊進來才會重新修正 Current , 這樣在 Chart 上會看到資料並沒有到最新的 Bars 上
		}
	}
}  //230行