using System;
using System.Drawing;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Chart;
using Zeghs.Rules;
using Zeghs.Products;

namespace Zeghs.Drawing.Plots {
	internal sealed class PlotHlc : AbstractPlot {
		private Gdi __cGDI = null;
		private Instrument __cBars = null;

		protected override int Current {
			get {
				return __cBars.CurrentBar;
			}
		}

		protected override int SeriesCount {
			get {
				return (__cBars.Close as SeriesAbstract<double>).Count;
			}
		}

		internal PlotHlc(GdiEngine engine, object source, ChartSetting chartSetting)
			: base(engine, chartSetting) {

			__cGDI = engine.GDI;
			__cBars = source as Instrument;
		}

		public override void CalculatePlot(AxisX axis) {
			this.AxisY.Reset();

			int iBarNumber = axis.BarNumber;
			int iCount = axis.BarCount;
			for (int i = 0; i < iCount; i++) {
				int iIndex = iBarNumber + i;
				if (iIndex > axis.DataCount) {
					break;
				} else {
					int iOffset = this.ConvertAxisScaleIndexToOffset(iBarNumber + i);
					this.AxisY.SetAxisScaleValue(__cBars.High[iOffset], __cBars.Low[iOffset]);
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
				AxisXUnit cUnit = cAxisX.ConvertBarNumberToWidth(iIndex);
				if (!DrawHlc(iOffset, cRectY.Y, cUnit, property.BackgroundColor)) {
					return false;
				}
			} else {
				this.RefreshAxisY(property);

				int iCount = cAxisX.BarCount;
				int iStartIndex = cAxisX.BarNumber;
				for (int i = 0; i < iCount; i++) {
					iIndex = iStartIndex + i;
					if (iIndex > cAxisX.DataCount) {
						--iIndex;
						break;
					} else {
						int iOffset = this.ConvertAxisScaleIndexToOffset(iIndex);
						AxisXUnit cUnit = cAxisX.ConvertBarNumberToWidth(iIndex);
						DrawHlc(iOffset, cRectY.Y, cUnit, Color.Empty);
					}
				}

				if (this.PlotIndex == 0) {  //Plot 索引值為 0 才印出標題(其他的索引就不需要再印出標題)
					DrawTitle(cRectY, property);
				}
			}

			if (!layer.Drawable) {
				layer.LegendIndex = iIndex;

				if (this.ChartSetting.IsShowNewPrice) {    //是否要顯示新價格線
					this.AxisY.Value = __cBars.Close[0];  //指定最新收盤價格
					this.Painter.DrawAxisValueLabel(this.AxisY, this.ChartSetting.LegendColor, property);
				}
			}
			this.DrawLegend(layer.LegendIndex, property);

			__cGDI.SelectBackground(iOldBKColor);
			return true;
		}

		protected override void CreateAxisY(ChartProperty property, Rectangle axisRectangle) {
			InstrumentSettings cSettings = __cBars.Info as InstrumentSettings;
			this.AxisY = AbstractPlot.CreateAxisY(this.Painter, this.ChartSetting.Axis, cSettings.Property.PriceScaleRule as IPriceScale, property, axisRectangle);
		}

		protected override void DrawLegend(int barNumber, ChartProperty property) {
			Rectangle cRectY = this.AxisY.AxisRectangle;
			IntPtr cOldFont = __cGDI.SelectFont(property.LegendFont);
			int iDecimals = this.AxisY.Decimals;
			int iTextY = ((this.PlotIndex + 1) * __cGDI.MeasureString("0").Height) + 5;
			int iOffset = this.ConvertAxisScaleIndexToOffset(barNumber);
			string sLegend = string.Format("{0} (O:{1}, H:{2}, L:{3}, C:{4}, V:{5})", __cBars.Time[iOffset].ToString("yyyy/MM/dd HH:mm"), Math.Round(__cBars.Open[iOffset], iDecimals), Math.Round(__cBars.High[iOffset], iDecimals), Math.Round(__cBars.Low[iOffset], iDecimals), Math.Round(__cBars.Close[iOffset], iDecimals), __cBars.Volume[iOffset]);

			__cGDI.BeginRopDraw();
			__cGDI.DrawRopString(sLegend, this.ChartSetting.LegendColor, 5, cRectY.Top + iTextY);
			__cGDI.EndRopDraw();
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
		}

		private bool CheckBarsValue(int offset, out double[] values) {
			values = new double[] { __cBars.Open[offset], __cBars.High[offset], __cBars.Low[offset], __cBars.Close[offset] };
			for (int i = 0; i < 4; i++) {
				double dValue = values[i];
				if (dValue < this.AxisY.Minimum || dValue > this.AxisY.Maximum) {
					return false;
				}
			}
			return true;
		}

		private bool DrawHlc(int offset, int top, AxisXUnit unit, Color eraseColor) {
			double[] dValues = null;
			bool bCheck = CheckBarsValue(offset, out dValues);
			if (bCheck) {
				int iOpen = top + this.AxisY.ConvertValueToHeight(dValues[0]);
				int iHigh = top + this.AxisY.ConvertValueToHeight(dValues[1]);
				int iLow = top + this.AxisY.ConvertValueToHeight(dValues[2]);
				int iClose = top + this.AxisY.ConvertValueToHeight(dValues[3]);

				if (eraseColor != Color.Empty) {
					__cGDI.FillRectangle(eraseColor, unit.LeftPoint + 1, iHigh, (unit.RightPoint - unit.LeftPoint) - 1, iLow - iHigh + 1);
				}

				bool bOpenH = iOpen < iClose;
				IntPtr cOldPtr = __cGDI.SelectPen((bOpenH) ? this.ChartSetting.PenStyles[1] : this.ChartSetting.PenStyles[0]);

				__cGDI.DrawLine(unit.CenterPoint, iHigh, unit.CenterPoint, iLow);
				__cGDI.DrawLine(unit.CenterPoint, iClose, unit.RightPoint, iClose);
				__cGDI.RemoveObject(__cGDI.SelectPen(cOldPtr));
			}
			return bCheck;
		}

		private void DrawTitle(Rectangle rectangle, ChartProperty property) {
			IQuote cQuote = __cBars.Quotes;
			IInstrumentSettings cSettings = __cBars.Info;
			InstrumentDataRequest cRequest = __cBars.Request;
			Resolution cResolution = cRequest.Resolution;
			string sTitle = string.Format("{0} {1} ({2}{3}) #{4} {5}", cQuote.SymbolId, cQuote.SymbolName, cResolution.Size, cResolution.Type.ToString(), cSettings.Category.ToString(), cRequest.Exchange);

			IntPtr cOldFont = __cGDI.SelectFont(property.TitleFont);
			__cGDI.DrawString(sTitle, property.ForeColor, 5, rectangle.Top + 2);
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
		}
	}
}