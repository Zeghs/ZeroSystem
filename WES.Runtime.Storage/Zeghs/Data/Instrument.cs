using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Data {
	/// <summary>
	///    商品資訊類別(包含開高低收資訊與商品屬性及其他重要資訊)
	/// </summary>
	public sealed class Instrument : IInstrument, IDisposable {
		/// <summary>
		///   當 CurrentBar 位置改變時所觸發的事件
		/// </summary>
		public event EventHandler<SeriesPositionChangeEvent> onPositionChange = null;

		private int __iCurrentBar = 0;
		private bool __bDisposed = false;
		private SeriesSymbolData __cSource = null;
		private IQuoteStorage __cQuoteStorage = null;
		private SeriesSymbolDataRand __cFullSymbolData = null;
		private EBarState __cBarsState = EBarState.None;

		/// <summary>
		///   [取得] Bars 最新的更新時間
		/// </summary>
		public DateTime BarUpdateTime {
			get {
				return __cSource.UpdateTime;
			}
		}

		/// <summary>
		///   [取得] 收盤價陣列資訊
		/// </summary>
		public ISeries<double> Close {
			get {
				return __cFullSymbolData.Close;
			}
		}

		/// <summary>
		///   [取得] 目前 Bars 索引值(索引從 1 開始)
		/// </summary>
		public int CurrentBar {
			get {
				return __iCurrentBar;
			}

			private set {
				__iCurrentBar = value;
				__cFullSymbolData.Current = value;
			}
		}

		/// <summary>
		///   [取得] 委託價量資訊
		/// </summary>
		public IDOMData DOM {
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

		/// <summary>
		///   [取得] 商品的全部資訊
		/// </summary>
		public ISeriesSymbolDataRand FullSymbolData {
			get {
				return __cFullSymbolData;
			}
		}

		/// <summary>
		///   [取得] 最高價陣列資訊
		/// </summary>
		public ISeries<double> High {
			get {
				return __cFullSymbolData.High;
			}
		}

		/// <summary>
		///   [取得] 商品屬性
		/// </summary>
		public IInstrumentSettings Info {
			get {
				InstrumentSettings cSettings =  __cSource.Settings;
				cSettings.SetExpirationFromTime(__cFullSymbolData.Time[0]);
				cSettings.SetPriceScaleFromClosePrice(__cFullSymbolData.Close[0]);
				
				return cSettings;
			}
		}

		/// <summary>
		///   [取得] 是否為最後一根 Bars
		/// </summary>
		public bool IsLastBars {
			get {
				return __cFullSymbolData.IsLastBars;
			}
		}

		/// <summary>
		///   [取得] 最後一根 Bars 的時間
		/// </summary>
		public DateTime LastBarTime {
			get {
				return __cSource.LastBarTime;
			}
		}

		/// <summary>
		///   [取得] 最低價陣列資訊
		/// </summary>
		public ISeries<double> Low {
			get {
				return __cFullSymbolData.Low;
			}
		}

		/// <summary>
		///   [取得] 開盤價陣列資訊
		/// </summary>
		public ISeries<double> Open {
			get {
				return __cFullSymbolData.Open;
			}
		}

		/// <summary>
		///   [取得] 資料請求資訊
		/// </summary>
		public InstrumentDataRequest Request {
			get {
				return __cSource.DataRequest;
			}
		}

		/// <summary>
		///   [取得] 開收盤的時間週期
		/// </summary>
		public List<SessionObject> Sessions {
			get {
				return __cSource.Settings.Sessions;
			}
		}

		/// <summary>
		///   [取得] Bars 狀態
		/// </summary>
		public EBarState Status {
			get {
				return __cBarsState;
			}
		}

		/// <summary>
		///   [取得] 日期時間陣列資訊
		/// </summary>
		public ISeries<DateTime> Time {
			get {
				return __cFullSymbolData.Time;
			}
		}

		/// <summary>
		///   [取得] 成交量陣列資訊
		/// </summary>
		public ISeries<double> Volume {
			get {
				return __cFullSymbolData.Volume;
			}
		}

		internal SeriesSymbolData SeriesSymbolData {
			get {
				return __cSource;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="source">SeriesSymbolData 類別</param>
		/// <param name="maxBarsReferance">最大 Bars count 參考值(如果不需要很多歷史資訊計算則可以設定小一點, 設定數量不可以超過歷史資料載入總數量)</param>
		public Instrument(SeriesSymbolData source, int maxBarsReferance) {
			__cSource = source;
			__cFullSymbolData = new SeriesSymbolDataRand(source, maxBarsReferance);

			this.CurrentBar = 1;  //預設值
			string sDataSource = __cSource.DataRequest.DataFeed;
			
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
		///   移動 Bars 至指定的時間前
		/// </summary>
		/// <param name="time">指定的時間</param>
		public void MoveBars(DateTime time) {
			bool bLoop = false;
			while (bLoop = GetBarsState(time, bLoop)) {
				if (onPositionChange != null) {
					onPositionChange(this, new SeriesPositionChangeEvent(__iCurrentBar, __cBarsState));
				}

				if (!Next()) {
					break;
				}
			}
		}

		/// <summary>
		///   從目前位置移動至下一個 Bars
		/// </summary>
		public bool Next() {
			if (__cFullSymbolData.Count == 0) {  //如果沒有資料就不用再移動 Bars
				return false;
			} else {
				bool bNext = true;
				++this.CurrentBar;

				int iCount = __cFullSymbolData.TryOutside();
				if (iCount > 0) {
					this.CurrentBar = iCount;
					bNext = false;
				}
				return bNext;
			}
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onPositionChange = null;

					__cFullSymbolData.Dispose();
				}
			}
		}

		/// <summary>
		///   [取得] 目前當下 Bars 狀態 
		/// </summary>
		/// <param name="time">欲比較的時間</param>
		/// <param name="isLoop">循環旗標</param>
		/// <returns>返回值: EBarState 狀態</returns>
		private bool GetBarsState(DateTime time, bool isLoop) {
			bool bRet = false;
			if (time < __cFullSymbolData.Time[0]) {  //如果傳入的時間小於目前 Bars 的時間
				if (isLoop) {
					--this.CurrentBar;
					return bRet;
				}

				double dOpen = __cFullSymbolData.Open[0];  //取出開盤價格(如果高低收都等於開盤價格則表示此 Bars 是新建立剛剛開盤的資訊)
				if (dOpen == __cFullSymbolData.High[0] && dOpen == __cFullSymbolData.Low[0] && dOpen == __cFullSymbolData.Close[0]) {
					__cBarsState = EBarState.Open;
				} else {
					__cBarsState = EBarState.Inside;
				}
			} else {
				__cBarsState = (__cSource.UpdateTime < __cFullSymbolData.Time[0]) ? EBarState.Inside : EBarState.Close;  //判斷 Bars 更新時間是否已經超過目前 Bars 的區間(如果超過表示此 Bars 區間已經收盤)
				bRet = true;
			}
			return bRet;
		}
	}
}  //293行