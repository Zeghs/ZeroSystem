using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Chart;
using Zeghs.Actions;
using Zeghs.Drawing.Plots;
using Zeghs.Drawing.Actions;

namespace Zeghs.Drawing {
	/// <summary>
	///   GDI Plus 繪圖引擎模組
	/// </summary>
	public sealed class GdiEngine : AbstractPaintEngine {
		private bool __bDisposed = false;
		private Gdi __cGDI = null;

		internal Gdi GDI {
			get {
				return __cGDI;
			}
		}

		public GdiEngine(IntPtr handle) {
			__cGDI = new Gdi(handle);
		}

		/// <summary>
		///    清除畫面
		/// </summary>
		/// <param name="color">欲清除的背景顏色</param>
		/// <param name="rectangle">欲清除的範圍區間</param>
		/// <param name="isClearAll">是否要清除全部的繪製物件</param>
		public override void Clear(Color color, Rectangle rectangle, bool isClearAll = true) {
			if (isClearAll) {
				__cGDI.FillRectangle(color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
				__cGDI.ClearRops(false);
			} else {
				__cGDI.ClearRops(true);
			}
		}

		/// <summary>
		///   取得字型長度與寬度
		/// </summary>
		/// <param name="text">計算 Size 的文字字串</param>
		/// <param name="font">字型類別</param>
		/// <returns>回傳值: Size 結構</returns>
		public override Size GetFontMetrics(string text, Font font) {
			IntPtr cOldFont = __cGDI.SelectFont(font);
			Size cSize = __cGDI.MeasureString(text);
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
			return cSize;
		}

		/// <summary>
		///   繪製 AxisY 軸的數值標籤(價格線或數值線標籤)
		/// </summary>
		/// <param name="axis">AxisY 座標軸</param>
		/// <param name="lineColor">標籤線色彩</param>
		/// <param name="property">Chart 屬性參數</param>
		public override void DrawAxisValueLabel(AxisY axis, Color lineColor, ChartProperty property) {
			double dNewValue = axis.Value;
			Rectangle cRect = axis.AxisRectangle;
			int iFontHeight = axis.FontMetrics.Height;
			int iHeight = axis.ConvertValueToHeight(dNewValue);
			if (iHeight > 0 && iHeight + iFontHeight < cRect.Height) {
				cRect.Y = cRect.Top + iHeight;
				cRect.Height = iFontHeight;

				int iDecimals = axis.Decimals;
				IntPtr cOldFont = __cGDI.SelectFont(property.AxisFont);
				IntPtr cPen = Gdi.CreatePen(new PowerLanguage.PenStyle(lineColor, 1));

				__cGDI.BeginRopDraw();
				__cGDI.DrawRopString(Math.Round(dNewValue, iDecimals).ToString(), property.BackgroundColor, property.ForeColor, 5, 0, cRect);
				__cGDI.DrawRopLine(cPen, 0, cRect.Y, cRect.X, cRect.Y);
				__cGDI.EndRopDraw();

				__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
			}
		}

		/// <summary>
		///    繪製 AxisX 座標軸
		/// </summary>
		/// <param name="axis">AxisX 座標軸</param>
		/// <param name="property">Chart 屬性參數</param>
		public override void DrawAxisX(AxisX axis, ChartProperty property) {
			uint uPattern = 0xaaaaaaaa;  //格線樣式(Gdi.cs 內的 SelectPen 方法有定義格式, 格式皆由 Plugin 設計者自由規劃)
			int iOldBKColor = __cGDI.SelectBackground(property.BackgroundColor);
			IntPtr cOldPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.GridColor, 1, (int) uPattern));
			IntPtr cGridPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.ForeColor, 1));
			IntPtr cOldFont = __cGDI.SelectFont(property.AxisFont);

			List<AxisXLabel> cAxises = axis.CalculateAxisScale();
			Rectangle cRect = axis.AxisRectangle;
			__cGDI.FillRectangle(property.BackgroundColor, cRect.Left, cRect.Top, cRect.Width, cRect.Height);

			int iCount = cAxises.Count;
			for (int i = 0; i < iCount; i++) {
				AxisXLabel cLabel = cAxises[i];
				AxisXUnit cUnit = axis.ConvertBarNumberToWidth(cLabel.BarNumber);
				int iAbsWidth = cRect.Left + cUnit.CenterPoint;
				int iCharWidth = iAbsWidth + axis.FontMetrics.Width * 11;  //MM/dd HH:mm (11 chars) 
				if (iCharWidth < cRect.Width) {
					__cGDI.DrawLine(iAbsWidth, cRect.Top, iAbsWidth, cRect.Top + 5);
					__cGDI.DrawString(cLabel.Time.ToString((i == 0) ? "yyyy/MM/dd" : "MM/dd HH:mm"), property.ForeColor, iAbsWidth, cRect.Top + 5);

					if (property.IsShowGrid) {
						//繪製格線(因為 AxisX 軸在最下方, 格線往上延伸即可)
						IntPtr cAxisPen = __cGDI.SelectPen(cGridPen);
						__cGDI.DrawLine(iAbsWidth, 0, iAbsWidth, cRect.Top);
						__cGDI.SelectPen(cAxisPen);
					}
				}
			}

			__cGDI.RemoveObject(cGridPen);
			__cGDI.RemoveObject(__cGDI.SelectPen(cOldPen));
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
			__cGDI.SelectBackground(iOldBKColor);
		}

		/// <summary>
		///    繪製 AxisY 座標軸
		/// </summary>
		/// <param name="axis">AxisY 座標軸</param>
		/// <param name="property">Chart 屬性參數</param>
		public override void DrawAxisY(AxisY axis, ChartProperty property) {
			uint uPattern = 0xaaaaaaaa;  //格線樣式(Gdi.cs 內的 SelectPen 方法有定義格式, 格式皆由 Plugin 設計者自由規劃)
			int iOldBKColor = __cGDI.SelectBackground(property.BackgroundColor);
			IntPtr cOldPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.GridColor, 1, (int) uPattern));
			IntPtr cGridPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.AxisColor, 1));
			IntPtr cOldFont = __cGDI.SelectFont(property.AxisFont);

			int iFontHeight = axis.FontMetrics.Height;
			List<double> cAxises = axis.CalculateAxisScale();
			Rectangle cRect = axis.AxisRectangle;
			
			__cGDI.FillRectangle(property.BackgroundColor, cRect.Left, cRect.Top, cRect.Width, cRect.Height);
			__cGDI.DrawRectangle(cRect.Left, cRect.Top, cRect.Width, cRect.Height);

			int iCount = cAxises.Count;
			for (int i = 0; i < iCount; i++) {
				double dValue = cAxises[i];
				int iHeight = axis.ConvertValueToHeight(dValue);
				if (iHeight + iFontHeight + 2 < cRect.Height) {
					int iAbsHeight = cRect.Top + iHeight;
					__cGDI.DrawLine(cRect.Left, iAbsHeight, cRect.Left + 5, iAbsHeight);
					__cGDI.DrawString(dValue.ToString(), property.ForeColor, cRect.Left + 5, iAbsHeight + 2);

					if (property.IsShowGrid) {
						//繪製格線(因為 AxisY 軸在最右方, 格線往左延伸即可)
						IntPtr cAxisPen = __cGDI.SelectPen(cGridPen);
						__cGDI.DrawLine(0, iAbsHeight, cRect.Left, iAbsHeight);
						__cGDI.SelectPen(cAxisPen);
					}
				}
			}

			__cGDI.RemoveObject(cGridPen);
			__cGDI.RemoveObject(__cGDI.SelectPen(cOldPen));
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
			__cGDI.SelectBackground(iOldBKColor);
		}

		/// <summary>
		///   繪製 Layer 圖層
		/// </summary>
		/// <param name="layer">Layer 圖層</param>
		/// <param name="property">Chart 屬性參數</param>
		/// <param name="onlyUpdateLastBar">是否僅更新最後一根Bar(Plot可以使用此旗標來判斷是否只繪製最後一根Bar, 也可以忽略此旗標全部的Bar都更新)</param>
		/// <returns>返回值:true=繪製 Layer 完成, false=繪製 Layer 未完成</returns>
		public override bool DrawLayer(Layer layer, ChartProperty property, bool onlyUpdateLastBar) {
			List<AbstractPlot> cPlots = layer.Plots;
			int iCount = cPlots.Count;
			if (iCount > 0) {
				AxisY cAxisY = layer.AxisY;
				bool bRefresh = cAxisY.Refresh;
				if (onlyUpdateLastBar || bRefresh) {
					for (int i = 0; i < iCount; i++) {
						AbstractPlot cPlot = cPlots[i];
						if (cPlot.IsSubChart) {
							if (!cPlot.DrawPlot(layer, property, onlyUpdateLastBar)) {
								return false;
							}
						}
					}

					if (bRefresh) {
						//繪製 Layer 邊框
						Rectangle cRect = layer.LayerRectangleWithoutAxisY;
						IntPtr cOldPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.ForeColor, 1));
						__cGDI.DrawRectangle(0, cRect.Y, cRect.Width, cRect.Height);
						__cGDI.RemoveObject(__cGDI.SelectPen(cOldPen));
					}
				}
			}
			return true;
		}

		/// <summary>
		///   取得 Plot 物件
		/// </summary>
		/// <param name="chart">ZChart 圖表類別</param>
		/// <param name="source">資料來源</param>
		/// <param name="chartType">圖表類型</param>
		/// <returns>回傳值: AbstractPlot 抽象類別</returns>
		public override AbstractPlot GetPlot(ZChart chart, object source, ChartSetting chartSetting) {
			return PlotEngine.Create(this, chart, source, chartSetting);
		}

		/// <summary>
		///   取得使用者自訂的操作/動作列表
		/// </summary>
		/// <returns>返回值: 使用者操作/動作列表</returns>
		protected override List<IAction> GetActions() {
			List<IAction> cActions = new List<IAction>(4);
			cActions.Add(new Cross(this));  //十字線請放在第一個位置
			cActions.Add(new Line(this));   //繪圖功能放在後面
			cActions.Add(new Ray(this));
			return cActions;
		}
		
		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					__cGDI.Dispose();
				}
			}
		}
	}
}