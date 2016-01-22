using System;

namespace PowerLanguage {
	/// <summary>
	///   商品的開高低收陣列資訊
	/// </summary>
	public interface ISeriesSymbolData {
		/// <summary>
		///   [取得] 收盤價陣列資訊
		/// </summary>
		ISeries<double> Close {
			get;
		}

		/// <summary>
		///   [取得] 最高價陣列資訊
		/// </summary>
		ISeries<double> High {
			get;
		}

		/// <summary>
		///   [取得] 最低價陣列資訊
		/// </summary>
		ISeries<double> Low {
			get;
		}

		/// <summary>
		///   [取得] 開盤價陣列資訊
		/// </summary>
		ISeries<double> Open {
			get;
		}

		/// <summary>
		///   [取得] 日期時間陣列資訊
		/// </summary>
		ISeries<DateTime> Time {
			get;
		}

		/// <summary>
		///   [取得] 成交量陣列資訊
		/// </summary>
		ISeries<double> Volume {
			get;
		}
	}
}