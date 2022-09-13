using System;
using System.Drawing;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Drawing.PlotShapes {
	internal sealed class PlotShapeLine : AbstractPlot {
		private Gdi __cGDI = null;
		private PlotObject<double> __cObjects = null;
		private double __dPrev = double.NaN, __dLast = 0;

		protected override int Current {
			get {
				return __cObjects.Value.Current;
			}
		}

		protected override int SeriesCount {
			get {
				return (__cObjects.Value as SeriesAbstract<double>).Count;
			}
		}

		internal PlotShapeLine(GdiEngine engine, object source, ChartSetting chartSetting)
			: base(engine, chartSetting) {
			
			__cGDI = engine.GDI;
			__cObjects = source as PlotObject<double>;
		}

		public override void CalculatePlot(AxisX axis) {
			this.AxisY.Reset();

			int iBarNumber = axis.BarNumber;
			VariableSeries<double> cValues = __cObjects.Value;

			int iCount = axis.BarCount;
			for (int i = 0; i < iCount; i++) {
				int iIndex = iBarNumber + i;
				if (iIndex > axis.DataCount) {
					break;
				} else {
					int iOffset = this.ConvertAxisScaleIndexToOffset(iBarNumber + i);
					this.AxisY.SetAxisScaleValue(cValues[iOffset], cValues[iOffset]);
				}
			}
		}

		public override bool DrawPlot(Layer layer, ChartProperty property, bool onlyUpdateLastBar) {
			AxisX cAxisX = layer.AxisX;
			Rectangle cRectY = this.AxisY.AxisRectangle;

			int iIndex = 0;
			int iOldBKColor = __cGDI.SelectBackground(property.BackgroundColor);

			if (onlyUpdateLastBar) {
				iIndex = cAxisX.BarNumber + cAxisX.BarCount - 1;
				iIndex = (iIndex > cAxisX.DataCount) ? cAxisX.DataCount : iIndex;
				int iOffset = this.ConvertAxisScaleIndexToOffset(iIndex);
				int iX = cAxisX.ConvertBarNumberToWidth(iIndex).CenterPoint;
				int iPrevX = cAxisX.ConvertBarNumberToWidth(iIndex - 1).CenterPoint;

				PowerLanguage.PenStyle cLinePen = GetPenStyle(iOffset);
				if (!DrawPlotShapeLine(iOffset, cRectY.Y, iX, iPrevX, new PowerLanguage.PenStyle(property.BackgroundColor, cLinePen.Width, cLinePen.Pattern))) {
					return false;
				}
			} else {
				this.RefreshAxisY(property);

				__dPrev = double.NaN;
				int iCount = cAxisX.BarCount;
				int iStartIndex = cAxisX.BarNumber;
				for (int i = 0; i < iCount; i++) {
					iIndex = iStartIndex + i;
					if (iIndex > cAxisX.DataCount) {
						--iIndex;
						break;
					} else {
						if (__dPrev != double.NaN) {
							int iOffset = this.ConvertAxisScaleIndexToOffset(iIndex);
							int iX = cAxisX.ConvertBarNumberToWidth(iIndex).CenterPoint;
							int iPrevX = cAxisX.ConvertBarNumberToWidth(iIndex - 1).CenterPoint;
							DrawPlotShapeLine(iOffset, cRectY.Y, iX, iPrevX, null);
						}
					}
				}
			}

			if (!layer.Drawable) {
				layer.LegendIndex = iIndex;

				if (this.ChartSetting.IsShowNewPrice) {    //是否要顯示新價格線
					this.AxisY.Value = __cObjects.Value[0];  //指定最新價格
					this.Painter.DrawAxisValueLabel(this.AxisY, this.ChartSetting.LegendColor, property);
				}
			}
			this.DrawLegend(layer.LegendIndex, property);

			__cGDI.SelectBackground(iOldBKColor);
			return true;
		}

		protected override void DrawLegend(int barNumber, ChartProperty property) {
			Rectangle cRectY = this.AxisY.AxisRectangle;
			IntPtr cOldFont = __cGDI.SelectFont(property.LegendFont);
			int iDecimals = this.AxisY.Decimals;
			int iTextY = ((this.PlotIndex + 1) * __cGDI.MeasureString("0").Height) + 5;
			int iOffset = this.ConvertAxisScaleIndexToOffset(barNumber);
			string sLegend = string.Format("{0} (V:{1})", __cObjects.Name, Math.Round(__cObjects.Value[iOffset], iDecimals));

			__cGDI.BeginRopDraw();
			__cGDI.DrawRopString(sLegend, this.ChartSetting.LegendColor, 5, cRectY.Top + iTextY);
			__cGDI.EndRopDraw();
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
		}

		private bool CheckObjectValue(int offset, out double value) {
			value = __cObjects.Value[offset];
			if (value < this.AxisY.Minimum || value > this.AxisY.Maximum) {
				return false;
			}

			__dPrev = __cObjects.Value[offset + 1];
			return true;
		}

		private bool DrawPlotShapeLine(int offset, int top, int x, int prevX, PowerLanguage.PenStyle erasePen) {
			double dValue = 0;
			bool bCheck = CheckObjectValue(offset, out dValue);
			if (bCheck) {
				int iPrevClose = top + this.AxisY.ConvertValueToHeight(__dPrev);
				int iCurrentClose = top + this.AxisY.ConvertValueToHeight(dValue);

				if (erasePen != null) {
					int iLastClose = top + this.AxisY.ConvertValueToHeight(__dLast);
					IntPtr cOldPen = __cGDI.SelectPen(erasePen);
					__cGDI.DrawLine(prevX, iPrevClose, x, iLastClose);
					__cGDI.RemoveObject(__cGDI.SelectPen(cOldPen));
				}
				__dLast = dValue;

				IntPtr cOldStyle = __cGDI.SelectPen(GetPenStyle(offset));
				__cGDI.DrawLine(prevX, iPrevClose, x, iCurrentClose);
				__cGDI.RemoveObject(__cGDI.SelectPen(cOldStyle));
			}
			return bCheck;
		}

		private PowerLanguage.PenStyle GetPenStyle(int offset) {
			PowerLanguage.PenStyle cPenStyle = null;
			VariableSeries<PowerLanguage.PenStyle[]> cPenStyles = __cObjects.PenStyles;
			if (cPenStyles[offset] == null) {
				cPenStyle = this.ChartSetting.PenStyles[0];
			} else {
				cPenStyle = cPenStyles[offset][0];
			}
			return cPenStyle;
		}
	}
}