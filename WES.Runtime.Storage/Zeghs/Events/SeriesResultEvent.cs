using System;
using Zeghs.Data;

namespace Zeghs.Events {
	/// <summary>
	///   序列商品資料回報事件(使用非同步取得 SeriesSymbolDataRand 會使用此類別存放回報結果)
	/// </summary>
	public sealed class SeriesResultEvent : EventArgs {
		private object __oParameters = null;
		private SeriesSymbolDataRand __cSeriesRand = null;

		/// <summary>
		///   [取得] 序列商品資料
		/// </summary>
		public SeriesSymbolDataRand Data {
			get {
				return __cSeriesRand;
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

		internal SeriesResultEvent(SeriesSymbolDataRand seriesSymbolDataRand, object parameters) {
			__cSeriesRand = seriesSymbolDataRand;
			__oParameters = parameters;
		}
	}
}