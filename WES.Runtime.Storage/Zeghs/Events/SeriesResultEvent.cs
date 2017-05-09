using System;
using Zeghs.Data;

namespace Zeghs.Events {
	/// <summary>
	///   序列商品資料回報事件(使用非同步取得 SeriesSymbolData 會使用此類別存放回報結果)
	/// </summary>
	public sealed class SeriesResultEvent : EventArgs {
		private object __oParameters = null;
		private SeriesSymbolData __cSeries = null;

		/// <summary>
		///   [取得] 序列商品資料
		/// </summary>
		public SeriesSymbolData Data {
			get {
				return __cSeries;
			}
		}

		/// <summary>
		///   [取得] 使用者自訂參數
		/// </summary>
		public object Parameters {
			get {
				return __oParameters;
			}
		}

		internal SeriesResultEvent(SeriesSymbolData series, object parameters) {
			__cSeries = series;
			__oParameters = parameters;
		}
	}
}