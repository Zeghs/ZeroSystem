using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Rules;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表 X 座標軸類別
	/// </summary>
	public sealed class AxisX {
		private const int DEFAULT_AXIS_MIN_BARCOUNT = 10;
		private const int DEFAULT_AXISLABEL_PADDING_WIDTH = 15;
		private const int DEFAULT_AXISRECTANGLE_PADDING_HEIGHT = 5;

		private bool __bRefresh = true;
		private int __iBarCount = 0;
		private int __iBarNumber = 0;
		private double __dScaleUnit = 0;
		private Instrument __cBars = null;
		private Size __cFontMetrics = Size.Empty;
		private Rectangle __cAxisRect = Rectangle.Empty;

		/// <summary>
		///   [取得/設定] 座標軸區域範圍
		/// </summary>
		public Rectangle AxisRectangle {
			get {
				return __cAxisRect;
			}

			set {
				__cAxisRect = value;
			}
		}

		/// <summary>
		///   [取得/設定] 欲顯示的 Bar 資料個數
		/// </summary>
		public int BarCount {
			get {
				return __iBarCount;
			}
			
			set {
				int iWidth = __cAxisRect.Width / 2;
				int iCount = (value <= 0) ? DEFAULT_AXIS_MIN_BARCOUNT : ((value < iWidth)) ? value : iWidth;
				if (iCount != __iBarCount) {
					__iBarCount = iCount;
					__bRefresh = true;
				}
			}
		}

		/// <summary>
		///   [取得] 起始 BarNumber 
		/// </summary>
		public int BarNumber {
			get {
				return __iBarNumber;
			}
			
			internal set {
				int iEndNumber = value + __iBarCount;
				int iNumber = (value < 1) ? 1 : (iEndNumber < __cBars.Count) ? value : (__cBars.Count > __iBarCount) ? __cBars.Count - __iBarCount + 1 : 1;
				if (__iBarNumber != iNumber) {
					__iBarNumber = iNumber;
					__bRefresh = true;
				}
			}
		}

		/// <summary>
		///   [取得] AxisX 軸的資料總個數
		/// </summary>
		public int DataCount {
			get {
				return __cBars.Count;
			}
		}

		/// <summary>
		///   [取得/設定] 字型寬度與高度的像素
		/// </summary>
		public Size FontMetrics {
			get {
				return __cFontMetrics;
			}

			set {
				__cFontMetrics = value;
				__cAxisRect.Height = __cFontMetrics.Height + DEFAULT_AXISRECTANGLE_PADDING_HEIGHT;
			}
		}

		/// <summary>
		///   [取得/設定] 是否需要更新
		/// </summary>
		internal bool Refresh {
			get {
				return __bRefresh;
			}

			set {
				__bRefresh = value;
			}
		}

		/// <summary>
		///   [取得] 序列資料容器 Count 數
		/// </summary>
		internal int SeriesCount {
			get {
				return (__cBars.Close as SeriesAbstract<double>).Count;
			}
		}

		internal AxisX(Instrument bars) {
			__cBars = bars;

			__iBarCount = 100;
			this.BarNumber = __cBars.Count;
		}
		
		public List<AxisXLabel> CalculateAxisScale() {
			__bRefresh = false;
			__dScaleUnit = __cAxisRect.Width / (double) __iBarCount;
			int iLabelWidth = this.FontMetrics.Width * 11 + DEFAULT_AXISLABEL_PADDING_WIDTH;  //EX: 2016/10/26
			int iScaleCount = (int) (iLabelWidth / __dScaleUnit) + 1;

			int iCount = 0;
			DateTime cPrevTime = DateTime.MinValue;
			List<AxisXLabel> cAxis = new List<AxisXLabel>();
			while (iCount < __iBarCount) {
				int iBarNumber = __iBarNumber + iCount;
				DateTime cTime = ConvertBarNumberToTime(iBarNumber);
				if (cTime > cPrevTime) {
					cAxis.Add(new AxisXLabel(iBarNumber, cTime));
					cPrevTime = cTime;
				} else {
					break;
				}
				
				iCount += iScaleCount;
			}
			return cAxis;
		}

		/// <summary>
		///   從繪圖座標 x 點來轉換 BarNumber
		/// </summary>
		/// <param name="x">繪圖座標 x 點</param>
		/// <returns>返回值: BarNumber 編號</returns>
		public int ConvertBarNumberFromX(int x) {
			return __iBarNumber + (int) (x / __dScaleUnit + 0.5);
		}

		/// <summary>
		///   轉換 BarNumber 編號至 DateTime
		/// </summary>
		/// <param name="barNumber">BarNumber 編號</param>
		/// <returns>返回值: DateTime 資訊</returns>
		public DateTime ConvertBarNumberToTime(int barNumber) {
			return __cBars.Time[__cBars.CurrentBar - barNumber];
		}

		/// <summary>
		///   轉換 BarNumber 編號為繪圖座標寬度
		/// </summary>
		/// <param name="barNumber">BarNumber 編號</param>
		/// <returns>返回值: AxisXUnit 結構</returns>
		public AxisXUnit ConvertBarNumberToWidth(int barNumber) {
			int iUnit_2 = (int) (__dScaleUnit / 2);
			int iCenter = (int) ((barNumber - __iBarNumber) * __dScaleUnit);
			return new AxisXUnit(iCenter - iUnit_2, iCenter, iCenter + iUnit_2);
		}
	}
}