namespace Zeghs.Chart {
	/// <summary>
	///   圖表 X 座標軸繪製單位資訊結構
	/// </summary>
	public struct AxisXUnit {
		private int __iCenterPoint;
		private int __iLeftPoint;
		private int __iRightPoint;

		/// <summary>
		///   [取得] 繪製中間點
		/// </summary>
		public int CenterPoint {
			get {
				return __iCenterPoint;
			}
		}

		/// <summary>
		///   [取得] 繪製左端點
		/// </summary>
		public int LeftPoint {
			get {
				return __iLeftPoint;
			}
		}

		/// <summary>
		///   [取得] 繪製右端點
		/// </summary>
		public int RightPoint {
			get {
				return __iRightPoint;
			}
		}

		internal AxisXUnit(int left, int center, int right) {
			__iLeftPoint = left;
			__iCenterPoint = center;
			__iRightPoint = right;
		}
	}
}