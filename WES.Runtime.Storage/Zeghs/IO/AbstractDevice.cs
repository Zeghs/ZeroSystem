using System;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;

namespace Zeghs.IO {
	/// <summary>
	///   抽象裝置基底類別(給 DataAdapter 使用的抽象裝置基底類別)
	/// </summary>
	public abstract class AbstractDevice {
		private long __iPosition = -1;
		private SeriesSymbolData __cSeries = null;

		/// <summary>
		///   [取得] 歷史資料請求結構
		/// </summary>
		protected InstrumentDataRequest DataRequest {
			get {
				return __cSeries.DataRequest;
			}
		}

		/// <summary>
		///   [取得/設定] 目前已讀取的歷史資料位置
		/// </summary>
		protected internal long Position {
			get {
				return __iPosition;
			}

			protected set {
				__iPosition = value;
			}
		}

		/// <summary>
		///   加入商品歷史資料
		/// </summary>
		/// <param name="time">時間</param>
		/// <param name="open">開盤價</param>
		/// <param name="high">最高價</param>
		/// <param name="low">最低價</param>
		/// <param name="close">收盤價</param>
		/// <param name="volume">成交量</param>
		protected void AddSeries(DateTime time, double open, double high, double low, double close, double volume) {
			__cSeries.AddSeries(time, open, high, low, close, volume, false);
		}

		/// <summary>
		///   調整商品資料容量
		/// </summary>
		/// <param name="count">欲調整資料個數</param>
		protected void AdjustSize(int count) {
			__cSeries.AdjustSize(count, true);
		}

		/// <summary>
		///   請求商品歷史資料
		/// </summary>
		/// <param name="e">資料請求事件</param>
		protected internal abstract void Request(DataRequestEvent e);

		internal void SetSeries(SeriesSymbolData series) {
			__cSeries = series;
		}
	}
}