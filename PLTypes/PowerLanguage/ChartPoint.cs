using System;

namespace PowerLanguage {
	/// <summary>
	///   圖表點座標結構
	/// </summary>
	public struct ChartPoint {
		private static ChartPoint __cEmpty = new ChartPoint(0);

		/// <summary>
		///   [取得] 空的 ChartPoint 結構
		/// </summary>
		public static ChartPoint Empty {
			get {
				return __cEmpty;
			}
		}

		/// <summary>
		///   [!=]運算子重載 
		/// </summary>
		/// <param name="left">左方運算元</param>
		/// <param name="right">右方運算元</param>
		/// <returns>返回值:true=不相等, false=相等</returns>
		public static bool operator !=(ChartPoint left, ChartPoint right) {
			return !left.Equals(right);
		}

		/// <summary>
		///   [==]運算子重載 
		/// </summary>
		/// <param name="left">左方運算元</param>
		/// <param name="right">右方運算元</param>
		/// <returns>返回值:true=相等, false=不相等</returns>
		public static bool operator ==(ChartPoint left, ChartPoint right) {
			return left.Equals(right);
		}

		private double __dPrice;
		private DateTime __cTime;
		private int? __iBarNumber;

		/// <summary>
		///   [取得/設定] Bar 編號
		/// </summary>
		public int? BarNumber {
			get {
				return __iBarNumber;
			}

			set {
				__iBarNumber = value;
			}
		}

		/// <summary>
		///   [取得/設定] 價格
		/// </summary>
		public double Price {
			get {
				return __dPrice;
			}

			set {
				__dPrice = value;
			}
		}

		/// <summary>
		///   [取得/設定] 時間
		/// </summary>
		public DateTime Time {
			get {
				return __cTime;
			}

			set {
				__cTime = value;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="price">價格</param>
		public ChartPoint(double price) {
			__iBarNumber = 0;
			__dPrice = price;
			__cTime = DateTime.MinValue;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="time">時間</param>
		/// <param name="price">價格</param>
		public ChartPoint(DateTime time, double price) {
			__iBarNumber = 0;
			__dPrice = price;
			__cTime = time;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="barNumber">Bar 編號</param>
		/// <param name="price">價格</param>
		public ChartPoint(int barNumber, double price) {
			__dPrice = price;
			__iBarNumber = barNumber;
			__cTime = DateTime.MinValue;
		}

		/// <summary>
		///   比較相等方法
		/// </summary>
		/// <param name="other">ChartPoint 結構</param>
		/// <returns>返回值: true=相等, false=不相等</returns>
		public bool Equals(ChartPoint other) {
			return this.__dPrice == other.__dPrice && (this.__iBarNumber == other.__iBarNumber || this.__cTime == other.__cTime);
		}

		/// <summary>
		///   判斷指定的 System.Object 和目前的 System.Object 是否相等。
		/// </summary>
		/// <param name="obj">System.Object，要與目前的 System.Object 比較。</param>
		/// <returns>如果指定的 System.Object 和目前的 System.Object 相等，則為 true，否則為 false。</returns>
		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			
			ChartPoint cPoint = (ChartPoint) obj;
			return this.__dPrice == cPoint.__dPrice && (this.__iBarNumber == cPoint.__iBarNumber || this.__cTime == cPoint.__cTime);
		}

		/// <summary>
		///   做為特定型別的雜湊函式。
		/// </summary>
		/// <returns>目前 System.Object 的雜湊程式碼。</returns>
		public override int GetHashCode() {
			return (__iBarNumber == 0) ? __cTime.GetHashCode() : (int) __iBarNumber;
		}

		/// <summary>
		///   傳回 System.String，表示目前的 System.Object。
		/// </summary>
		/// <returns>System.String，表示目前的 System.Object。</returns>
		public override string ToString() {
			return string.Format("[ChartPoint] price={0}, barNumber={1}, time={2}", __dPrice, __iBarNumber, __cTime.ToString("yyyy-MM-dd HH:mm:ss"));
		}
	}
}