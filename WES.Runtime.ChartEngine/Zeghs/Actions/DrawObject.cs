using System;
using System.Drawing;
using System.Threading;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Actions {
	/// <summary>
	///   使用者繪製物件資訊類別
	/// </summary>
	public sealed class DrawObject : IDrawObject {
		private static int __iObjectID = 0;

		private int __iID = 0;
		private int __iLayerIndex = 0;
		private bool __bExist = true;
		private bool __bLocked = false;
		private string __sName = null;
		private PenStyle[] __cPens = null;
		private ChartPoint[] __cPoints = null;

		/// <summary>
		///   [取得] 繪圖物件是否存在
		/// </summary>
		public bool Exist {
			get {
				return __bExist;
			}
		}

		/// <summary>
		///   [取得] 繪圖物件 ID
		/// </summary>
		public int ID {
			get {
				return __iID;
			}
		}

		/// <summary>
		///   Layer 圖層的索引值
		/// </summary>
		public int LayerIndex {
			get {
				return __iLayerIndex;
			}
		}

		/// <summary>
		///   [設定] 鎖定繪圖物件以防止用戶修改
		/// </summary>
		public bool Locked {
			set {
				__bLocked = value;
			}
		}

		/// <summary>
		///   [取得] 繪製物件的繪製樣式名稱(例:Line, Ray...)
		/// </summary>
		public string Name {
			get {
				return __sName;
			}
		}

		/// <summary>
		///   [取得] 繪製筆樣式
		/// </summary>
		public PenStyle[] Pens {
			get {
				return __cPens;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		internal DrawObject(string name, int layerIndex, ChartPoint[] points, PenStyle[] pens) {
			__iID = Interlocked.Increment(ref __iObjectID);

			__sName = name;
			__iLayerIndex = layerIndex;
			__cPoints = points;
			__cPens = pens;
		}

		/// <summary>
		///   轉換 PowerLanguage.ChartPoint 為 System.Drawing.Point 座標
		/// </summary>
		/// <param name="layer">Layer 圖層</param>
		/// <returns>返回值: System.Drawing.Point 陣列</returns>
		public Point[] ConvertPoints(Layer layer) {
			AxisX cAxisX = layer.AxisX;
			AxisY cAxisY = layer.AxisY;
			int iTop = cAxisY.AxisRectangle.Top;

			int iLength = __cPoints.Length;
			Point[] cPoints = new Point[iLength];
			for (int i = 0; i < iLength; i++) {
				ChartPoint cCPoint = __cPoints[i];
				cPoints[i] = new Point(cAxisX.ConvertBarNumberToWidth(cCPoint.BarNumber.Value).CenterPoint, iTop + cAxisY.ConvertValueToHeight(cCPoint.Price));
			}
			return cPoints;
		}

		/// <summary>
		///   繪圖物件是否被刪除
		/// </summary>
		/// <returns>回傳值: true=刪除, false=尚未刪除</returns>
		public bool Delete() {
			if (__bExist) {
				__bExist = false;
			}
			return !__bExist;
		}

		/// <summary>
		///   指出目前的物件是否等於另一個具有相同型別的物件。
		/// </summary>
		/// <param name="other">要與這個物件相互比較的物件。</param>
		/// <returns>如果目前的物件等於 other 參數，則為 true，否則為 false。</returns>
		public bool Equals(IDrawObject other) {
			if (other == null) {
				return false;
			}
			return this.ID == other.ID;
		}

		/// <summary>
		///   指出目前的物件是否等於另一個具有相同型別的物件。
		/// </summary>
		/// <param name="other">要與這個物件相互比較的物件。</param>
		/// <returns>如果目前的物件等於 other 參數，則為 true，否則為 false。</returns>
		public bool Equals(ITextObject other) {
			if (other == null) {
				return false;
			}
			return this.ID == other.ID;
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
			
			DrawObject cDrawObject = obj as DrawObject;
			if (cDrawObject == null) {
				return false;
			}
			return this.ID == cDrawObject.ID;
		}

		/// <summary>
		///   做為特定型別的雜湊函式。
		/// </summary>
		/// <returns>目前 System.Object 的雜湊程式碼。</returns>
		public override int GetHashCode() {
			return __iID;
		}
	}
}