using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Products;
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
		private EBarState __cBarsState = EBarState.None;
		private SeriesSymbolDataRand __cFullSymbolData = null;

		/// <summary>
		///   [取得] Bars 最新的更新時間
		/// </summary>
		public DateTime BarUpdateTime {
			get {
				return __cFullSymbolData.Source.UpdateTime;
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
				return (__iCurrentBar < 1) ? 1 : __iCurrentBar;
			}

			private set {
				__iCurrentBar = value;
				__cFullSymbolData.Current = (__iCurrentBar < 1) ? 1 : __iCurrentBar;
			}
		}

		/// <summary>
		///   [取得] 委託價量資訊
		/// </summary>
		public IDOMData DOM {
			get {
				return __cFullSymbolData.DOM;
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
				InstrumentSettings cSettings =  __cFullSymbolData.Source.Settings;
				cSettings.SetExpirationFromTime((__cFullSymbolData.BarsCount > 0) ? __cFullSymbolData.Time.Value : __cFullSymbolData.Source.UpdateTime);
				cSettings.SetPriceScaleFromClosePrice((__cFullSymbolData.BarsCount > 0) ? __cFullSymbolData.Close.Value : __cFullSymbolData.Quotes.ReferPrice);
				
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
				return __cFullSymbolData.Source.LastBarTime;
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
		///   [取得] 即時報價資訊
		/// </summary>
		public IQuote Quotes {
			get {
				return __cFullSymbolData.Quotes;
			}
		}

		/// <summary>
		///   [取得] 資料請求資訊
		/// </summary>
		public InstrumentDataRequest Request {
			get {
				return __cFullSymbolData.Source.DataRequest;
			}
		}

		/// <summary>
		///   [取得] 開收盤的時間週期
		/// </summary>
		public List<SessionObject> Sessions {
			get {
				return __cFullSymbolData.Source.Settings.Sessions;
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
		///   建構子
		/// </summary>
		/// <param name="seriesSymbolDataRand">SeriesSymbolDataRand 類別</param>
		public Instrument(SeriesSymbolDataRand seriesSymbolDataRand) {
			__cFullSymbolData = seriesSymbolDataRand;
		}

		/// <summary>
		///   綁定清盤重置事件
		/// </summary>
		/// <param name="onReset">EventHandler 事件委派</param>
		public void BindResetEvent(EventHandler onReset) {
			__cFullSymbolData.Source.onReset += onReset;
		}

		/// <summary>
		///   清除綁定的清盤重置事件
		/// </summary>
		/// <param name="onReset">EventHandler 事件委派</param>
		public void ClearResetEvent(EventHandler onReset) {
			__cFullSymbolData.Source.onReset -= onReset;
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
			if (__cBarsState == EBarState.Inside) {  //檢查是否為 Inside (如果是 Inside 表示尚未 Close)
				bool bInside = __cFullSymbolData.Source.UpdateTime < __cFullSymbolData.Time.Value;  //判斷是否還是 Inside 如果是就直接離開
				if (bInside) {
					return false;  //如果是 Inside 表示最後一根 Bars 也尚未 Close 狀態, 不需要呼叫 Next 方法
				} else {
					__cBarsState = EBarState.Close;  //如果已經 Close 將狀態設定為 Close
					if (onPositionChange != null) {  //發送 Close 狀態的 onPositionChange 事件(如果不補發送, 最後的 Close 狀態會被忽略導致其他問題, 所以還是要補送最後 Close 狀態)
						onPositionChange(this, new SeriesPositionChangeEvent(__iCurrentBar, __cBarsState));
					}
				}
			}
			
			return this.Next(DateTime.MinValue);
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					onPositionChange = null;

					SeriesManager.Manager.RemoveSeries(__cFullSymbolData);
					__cFullSymbolData.Dispose();
				}
			}
		}

		/// <summary>
		///   [取得] 目前當下 Bars 狀態 
		/// </summary>
		/// <param name="time">欲比較的時間</param>
		/// <returns>返回值: true=目前 Bars 狀態在傳入的時間區間內, false=目前 Bars 狀態在傳入的時間區間後面</returns>
		private bool GetBarsState(DateTime time) {
			bool bRet = true;
			if (time < __cFullSymbolData.Time.Value) {  //如果傳入的時間小於目前 Bars 的時間
				--this.CurrentBar;
				bRet = false;
			}

			bool bInside = __cFullSymbolData.Source.UpdateTime < __cFullSymbolData.Time.Value;
			if (bInside) {
				__cBarsState = (__cFullSymbolData.High.Value == __cFullSymbolData.Low.Value) ? EBarState.Open : EBarState.Inside;
			} else {
				__cBarsState = EBarState.Close;
			}
			return bRet;
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
				if (onPositionChange != null) {
					onPositionChange(this, new SeriesPositionChangeEvent(__iCurrentBar, __cBarsState));
				}
				return bNext && bRet;
			}
		}
	}
}  //314行