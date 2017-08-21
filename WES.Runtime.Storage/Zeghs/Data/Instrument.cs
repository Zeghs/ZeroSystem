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

		private int __iCurrentBar = 0;  //目前 Bars 索引值(從 1 開始為第一根 Bars , 0=尚未開始執行 Next 方法時索引保持在此)
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
		///   [取得] Bars 資料總個數
		/// </summary>
		public int Count {
			get {
				return __cFullSymbolData.BarsCount;
			}
		}

		/// <summary>
		///   [取得] 目前 Bars 索引值(索引從 1 開始)
		/// </summary>
		public int CurrentBar {
			get {
				return (__iCurrentBar == 0) ? 1 : __iCurrentBar;
			}

			private set {
				__iCurrentBar = value;
				__cFullSymbolData.Current = (__iCurrentBar == 0) ? 1 : __iCurrentBar;
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

		/// <summary>
		///   [取得] 來源 SeriesSymbolData 資料
		/// </summary>
		internal SeriesSymbolData Source {
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

			string sDataSource = __cSource.DataRequest.DataFeed;
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
			if (cService != null) {
				__cQuoteStorage = cService.Storage;
			}
		}

		/// <summary>
		///   綁定清盤重置事件
		/// </summary>
		/// <param name="onReset">EventHandler 事件委派</param>
		public void BindResetEvent(EventHandler onReset) {
			__cSource.onReset += onReset;
		}

		/// <summary>
		///   清除綁定的清盤重置事件
		/// </summary>
		/// <param name="onReset">EventHandler 事件委派</param>
		public void ClearResetEvent(EventHandler onReset) {
			__cSource.onReset -= onReset;
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
			while (this.Next(time));
		}

		/// <summary>
		///   從目前位置移動至下一個 Bars
		/// </summary>
		/// <returns>返回值: true=移動至下一個 Bars 成功, false=已經是最後一個 Bars</returns>
		public bool Next() {
			return this.Next(DateTime.MinValue);
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onPositionChange = null;

					__cFullSymbolData.Dispose();
					SeriesManager.Manager.RemoveInstrument(this);
				}
			}
		}

		/// <summary>
		///   [取得] 目前當下 Bars 狀態 
		/// </summary>
		/// <param name="time">欲比較的時間</param>
		/// <returns>返回值: true=目前 Bars 狀態在傳入的時間區間內, false=目前 Bars 狀態在傳入的時間區間後面</returns>
		private bool GetBarsState(DateTime time) {
			if (time < __cFullSymbolData.Time.Value) {  //如果傳入的時間小於目前 Bars 的時間
				--this.CurrentBar;
				return false;
			} else {
				bool bInside = __cSource.UpdateTime < __cFullSymbolData.Time.Value;
				if (bInside) {
					__cBarsState = (__cFullSymbolData.High.Value == __cFullSymbolData.Low.Value) ? EBarState.Open : EBarState.Inside; 
				} else {
					__cBarsState = EBarState.Close;
				}
				return true;
			}
		}
		
		/// <summary>
		///   從目前位置移動至下一個 Bars
		/// </summary>
		/// <param name="time">參考時間值(如果有傳入此時間, 會比對此時間與目前 CurrentBar 所在的時間區間, 如果傳入 DateTime.MinValue 則傳入目前 CurrentBar 所在的時間區間)</param>
		/// <returns>返回值: true=移動至下一個 Bars 成功, false=已經是最後一個 Bars</returns>
		private bool Next(DateTime time) {
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

				bool bRet = GetBarsState((time == DateTime.MinValue) ? __cFullSymbolData.Time.Value : time);
				if (bRet) {
					if (onPositionChange != null) {
						onPositionChange(this, new SeriesPositionChangeEvent(__iCurrentBar, __cBarsState));
					}
				}
				return bNext && bRet;
			}
		}
	}
}  //320行