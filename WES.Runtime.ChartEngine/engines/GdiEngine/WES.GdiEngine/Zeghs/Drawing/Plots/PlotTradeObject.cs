using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Drawing.Plots {
	internal sealed class PlotTradeObject : AbstractPlot {
		private static uint __uPattern = 0xaaaaaaaa;
		private static Font __cLegendFont = new Font("Courier New", 7);
		private static Font __cSymbolFont = new Font("Sans Serif", 9);

		private Gdi __cGDI = null;
		private ITrade __cPrevTrade = null;
		private TradeContainer __cTrades = null;

		protected override int Current {
			get {
				return 0;
			}
		}

		protected override int SeriesCount {
			get {
				return 0;
			}
		}

		internal PlotTradeObject(GdiEngine engine, object source, ChartSetting chartSetting)
			: base(engine, chartSetting) {
			
			__cGDI = engine.GDI;
			__cTrades = source as TradeContainer;
		}

		public override void CalculatePlot(AxisX axis) {
		}

		public override bool DrawPlot(Layer layer, ChartProperty property, bool onlyUpdateLastBar) {
			AxisX cAxisX = layer.AxisX;
			Rectangle cRectY = this.AxisY.AxisRectangle;
			Rectangle4 cLayerRect = new Rectangle4(layer.LayerRectangleWithoutAxisY);

			__cGDI.SaveDC();
			__cGDI.ClipRectangle(layer.LayerRectangleWithoutAxisY);

			int iOldMode = __cGDI.SelectTransparent();
			IntPtr iOldFont = __cGDI.SelectFont(__cSymbolFont);
			IntPtr iOldPen = __cGDI.SelectPen(new PowerLanguage.PenStyle(property.TradeLineColor, 1, (int) __uPattern));
			IntPtr iNameFont = __cLegendFont.ToHfont();

			if (onlyUpdateLastBar) {
				if (__cPrevTrade != null) {
					DrawTradeInfo(__cPrevTrade, cAxisX, property, iNameFont, cLayerRect, cRectY.Top);
				}
			} else {
				this.RefreshAxisY(property);

				int iStartIndex = cAxisX.BarNumber;
				int iEndIndex = iStartIndex + cAxisX.BarCount - 1;
				HashSet<ITrade> cTrades = __cTrades.GetTradeObjects(iStartIndex, iEndIndex);
				foreach (ITrade cTrade in cTrades) {
					DrawTradeInfo(cTrade, cAxisX, property, iNameFont, cLayerRect, cRectY.Top);
					__cPrevTrade = cTrade;
				}
			}

			__cGDI.RemoveObject(iNameFont);
			__cGDI.RemoveObject(__cGDI.SelectPen(iOldPen));
			__cGDI.RemoveObject(__cGDI.SelectFont(iOldFont));

			if (layer.Drawable) {
				this.DrawLegend(layer.LegendIndex, property);
			}
			__cGDI.ClearTransparent(iOldMode);
			__cGDI.RestoreDC();
			return true;
		}

		protected override void DrawLegend(int barNumber, ChartProperty property) {
			int iDecimals = this.AxisY.Decimals;
			Rectangle cRectY = this.AxisY.AxisRectangle;
			IntPtr cOldFont = __cGDI.SelectFont(__cLegendFont);
			Size cFontMetrics = __cGDI.MeasureString("0");
			
			__cGDI.BeginRopDraw();

			List<ITrade> cTrades = __cTrades.GetTradeObject(barNumber);
			int iRow = 0, iCount = cTrades.Count;
			for (int i = 0; i < iCount; i++) {
				ITrade cTrade = cTrades[i];
				ITradeOrder cEntry = cTrade.EntryOrder;
				string sEntryOrder = string.Format("#{0} {1,-10} {2} at {3} {4}", cEntry.Ticket, cEntry.Action, cEntry.Contracts, Math.Round(cEntry.Price, iDecimals), cEntry.Time.ToString("yyyy-MM-dd HH:mm:ss"));
				__cGDI.DrawRopString(sEntryOrder, this.ChartSetting.LegendColor, cRectY.X - (sEntryOrder.Length * cFontMetrics.Width) - 4, cRectY.Top + ((iRow++ * cFontMetrics.Height) + 2));

				ITradeOrder cExit = cTrade.ExitOrder;
				if (cExit != null) {
					string sExitOrder = string.Format("#{0} {1,-10} {2} at {3} {4}", cExit.Ticket, cExit.Action, cExit.Contracts, Math.Round(cExit.Price, iDecimals), cExit.Time.ToString("yyyy-MM-dd HH:mm:ss"));
					__cGDI.DrawRopString(sExitOrder, this.ChartSetting.LegendColor, cRectY.X - (sExitOrder.Length * cFontMetrics.Width) - 4, cRectY.Top + ((iRow++ * cFontMetrics.Height) + 2));
				}
			}
			__cGDI.EndRopDraw();
			__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
		}

		private void DrawTradeInfo(ITrade trade, AxisX axisX, ChartProperty property, IntPtr font, Rectangle4 layerRect, int top) {
			ITradeOrder cEntry = trade.EntryOrder;
			Point cPoint1 = new Point(axisX.ConvertBarNumberToWidth(cEntry.BarNumber).CenterPoint, top + this.AxisY.ConvertValueToHeight(cEntry.Price));
			__cGDI.DrawString("▸", property.TradeSymbolColor, cPoint1.X - 7, cPoint1.Y - 7);
			DrawTradeName(cEntry.Name, font, property.ForeColor, cPoint1, true);

			ITradeOrder cExit = trade.ExitOrder;
			if (cExit != null) {
				Point cPoint2 = new Point(axisX.ConvertBarNumberToWidth(cExit.BarNumber).CenterPoint, top + this.AxisY.ConvertValueToHeight(cExit.Price));
				__cGDI.DrawString("◂", property.TradeSymbolColor, cPoint2.X + 1, cPoint2.Y - 7);
				DrawTradeName(cExit.Name, font, property.ForeColor, cPoint2, false);

				if (Boundary.BoundFix(ref cPoint1, ref cPoint2, layerRect)) {
					__cGDI.DrawLine(cPoint1.X, cPoint1.Y, cPoint2.X, cPoint2.Y);
				}
			}
		}

		private void DrawTradeName(string name, IntPtr font, Color color, Point point, bool isEntry) {
			IntPtr iSymbolFont = __cGDI.SelectFont(font);
			Size cSize = __cGDI.MeasureString(name);
			__cGDI.DrawString(name, color, point.X - cSize.Width / 2, point.Y + ((isEntry) ? -cSize.Height : 2));
			__cGDI.SelectFont(iSymbolFont);
		}
	}
}