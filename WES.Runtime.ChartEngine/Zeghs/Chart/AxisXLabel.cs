using System;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表 X 座標軸標籤資訊結構
	/// </summary>
	public struct AxisXLabel {
		private DateTime __cTime;
		private int __iBarNumber;

		/// <summary>
		///   [取得] 座標軸標籤刻度 Bar Number
		/// </summary>
		public int BarNumber {
			get {
				return __iBarNumber;
			}
		}

		/// <summary>
		///   [取得] 座標軸標籤刻度時間
		/// </summary>
		public DateTime Time {
			get {
				return __cTime;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="barNumber">座標軸標籤刻度 Bar Number</param>
		/// <param name="time">座標軸標籤刻度時間</param>
		public AxisXLabel(int barNumber, DateTime time) {
			__iBarNumber = barNumber;
			__cTime = time;
		}
	}
}