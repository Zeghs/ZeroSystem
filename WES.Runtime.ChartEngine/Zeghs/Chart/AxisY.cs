using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Rules;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表 Y 座標軸類別
	/// </summary>
	public sealed class AxisY {
		private const int SCALE_SPACING_PIXEL = 15;
		private const int DEFAULT_AXISRECTANGLE_PADDING_WIDTH = 25;

		private bool __bRefresh = true;
		private int __iScaleCount = 0;
		private int __iMarginTop = 0;
		private int __iMarginBottom = 0;
		private double __dAxisMax = 0;
		private double __dAxisMin = 0;
		private double __dScaleUnit = 0;
		private double __dUnitHeight = 0;
		private AxisSetting __cSetting = null;
		private IPriceScale __cPriceScale = null;
		private Rectangle __cAxisRect = Rectangle.Empty;

		/// <summary>
		///   [取得/設定] 座標軸區域範圍
		/// </summary>
		public Rectangle AxisRectangle {
			get {
				CalculateAxisRectangleWidth();
				return __cAxisRect;
			}
			
			set {
				__cAxisRect = value;
			}
		}

		/// <summary>
		///   [取得/設定] 小數位數(預設: 0)
		/// </summary>
		public int Decimals {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 字型寬度與高度的像素
		/// </summary>
		public Size FontMetrics {
			get;
			set;
		}

		/// <summary>
		///   [取得] 下邊界高度
		/// </summary>
		public int MarginBottom {
			get {
				return __iMarginBottom;
			}
		}

		/// <summary>
		///   [取得] 上邊界高度
		/// </summary>
		public int MarginTop {
			get {
				return __iMarginTop;
			}
		}

		/// <summary>
		///   [取得] AxisY 最大刻度值
		/// </summary>
		public double Maximum {
			get {
				return __dAxisMax;
			}
		}

		/// <summary>
		///   [取得] AxisY 最小刻度值
		/// </summary>
		public double Minimum {
			get {
				return __dAxisMin;
			}
		}

		/// <summary>
		///   [取得/設定] 目前最新數值
		/// </summary>
		public double Value {
			get;
			set;
		}

		/// <summary>
		///   [取得] 是否需要更新
		/// </summary>
		public bool Refresh {
			get {
				return __bRefresh;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="setting">圖表座標軸設定檔</param>
		/// <param name="scale">價格縮放規則介面(可以由此規則取得價格座標與最小跳動點)</param>
		internal AxisY(AxisSetting setting, IPriceScale scale) {
			__cSetting = setting;
			__cPriceScale = scale;

			__dAxisMax = double.MinValue;
			__dAxisMin = double.MaxValue;
		}

		/// <summary>
		///   計算Y軸刻度值與 AxisRectangle 外框寬度(可在計算完後在取得 AxisRectangle)
		/// </summary>
		/// <returns>返回值: Y軸刻度值列表</returns>
		public List<double> CalculateAxisScale() {
			CalculateAxisScaleCount();  //計算座標軸上的刻度個數
			CalculateAxisUnit();  //計算每個刻度最小單位值

			__bRefresh = false;
			__iScaleCount = (__iScaleCount <= 0) ? 1 : __iScaleCount;

			List<double> cAxis = new List<double>(__iScaleCount);
			for (int i = 0; i <= __iScaleCount; i++) {
				cAxis.Add(Math.Round(__dAxisMax - __dScaleUnit * i, this.Decimals));
			}
			return cAxis;
		}

		/// <summary>
		///   從繪圖座標 y 點轉換為 AxisY 軸上的刻度值
		/// </summary>
		/// <param name="y">繪圖座標 y 點</param>
		/// <returns>返回值: AxisY 軸上的數值</returns>
		public double ConvertValueFromY(int y) {
			Rectangle cRect = this.AxisRectangle;
			return __dAxisMax - ((y - cRect.Y) / __dUnitHeight);
		}

		/// <summary>
		///   將 AxisY 軸上的刻度值轉換為Y軸上的高度點
		/// </summary>
		/// <param name="value">欲轉換的值</param>
		/// <returns>返回值: Y軸上的高度點</returns>
		public int ConvertValueToHeight(double value) {
			return (int) (__iMarginTop + (__dAxisMax - value) * __dUnitHeight);
		}

		/// <summary>
		///   重置座標軸(當每次重繪時皆須要重置座標軸)
		/// </summary>
		public void Reset() {
			EAxisScope cAxisScope = __cSetting.AxisScope;
			switch (cAxisScope) {
				case EAxisScope.CurrentScope:
				case EAxisScope.ChangeScope:
				case EAxisScope.PriceScaleScope:
					__dAxisMax = double.MinValue;
					__dAxisMin = double.MaxValue;
					break;
			}
		}

		/// <summary>
		///   設定座標軸的最大值與最小值
		/// </summary>
		/// <param name="max">最大值</param>
		/// <param name="min">最小值</param>
		public void SetAxisScaleValue(double max, double min) {
			if (max > __dAxisMax) {
				__dAxisMax = max;
				__bRefresh = true;
			}
			
			if (min < __dAxisMin) {
				__dAxisMin = min;
				__bRefresh = true;
			}
		}

		internal void SetAxisRectangle(Rectangle layer, int width) {
			__cAxisRect.X = layer.Width - width;
			__cAxisRect.Y = layer.Y;
			__cAxisRect.Height = layer.Height;
			__cAxisRect.Width = width;
			__bRefresh = true;
		}

		private void CalculateAxisScaleCount() {
			__iMarginTop = (int) (this.__cAxisRect.Height * __cSetting.MarginTop / 100);
			__iMarginBottom = (int) (this.__cAxisRect.Height * __cSetting.MarginBottom / 100);
			
			int iHeight = this.__cAxisRect.Height - (__iMarginTop + __iMarginBottom);
			__iScaleCount = iHeight / (this.FontMetrics.Height + SCALE_SPACING_PIXEL);
			__dUnitHeight = iHeight / (__dAxisMax - __dAxisMin);

			EAxisScaleMode cScaleMode = __cSetting.ScaleMode;
			switch (cScaleMode) {
				case EAxisScaleMode.ScaleCount:
					int iValue = (int) __cSetting.ScaleValue;
					__iScaleCount = (__iScaleCount < iValue) ? __iScaleCount : iValue;
					break;
			}
		}

		private void CalculateAxisUnit() {
			double dValue = 0;
			if (__cSetting.AxisScope == EAxisScope.PriceScaleScope) {
				if (__cPriceScale != null) {
					double[] dScales = __cPriceScale.GetPriceScale(__dAxisMax);
					dValue = dScales[0] * dScales[1];
				}
			} else {
				EAxisScaleMode cScaleMode = __cSetting.ScaleMode;
				switch (cScaleMode) {
					case EAxisScaleMode.ScaleGap:
						dValue = __cSetting.ScaleValue;
						break;
				}
			}

			__dScaleUnit = (__dAxisMax - __dAxisMin) / __iScaleCount;
			if (dValue > 0) {
				__dScaleUnit = Math.Ceiling(__dScaleUnit / dValue) * dValue;
			}
		}

		private void CalculateDecimals(double scale) {
			string sScale = scale.ToString();
			int iIndex = sScale.IndexOf('.');
			if (iIndex > -1) {
				++iIndex;
				this.Decimals = sScale.Length - iIndex;
			}
		}

		private void CalculateAxisRectangleWidth() {
			if (__bRefresh) {
				if (__cPriceScale != null) {
					double[] dScales = __cPriceScale.GetPriceScale(__dAxisMax);
					CalculateDecimals(dScales[0]);  //取得小數點位數
				}

				string sValue = Math.Round(__dAxisMax, this.Decimals).ToString();
				int iWidth = sValue.Length * this.FontMetrics.Width + DEFAULT_AXISRECTANGLE_PADDING_WIDTH;
				if (iWidth > __cAxisRect.Width) {
					__cAxisRect.X = __cAxisRect.Left + (__cAxisRect.Width - iWidth);
					__cAxisRect.Width = iWidth;
				}
			}
		}
	}
}