using System;
using PowerLanguage;
using Zeghs.Chart;
using Zeghs.Drawing.PlotSharps;

namespace Zeghs.Drawing.Plots {
	internal sealed class PlotEngine {
		internal static AbstractPlot Create(GdiEngine engine, ZChart chart, object source, ChartSetting chartSetting) {
			AbstractPlot cPlot = null;
			EChartType cChartType = chartSetting.ChartType;
			switch (cChartType) {
				case EChartType.Candlestick:
					cPlot = new PlotCandlestick(engine, source, chartSetting);
					break;
				case EChartType.HLC:
					cPlot = new PlotHlc(engine, source, chartSetting);
					break;
				case EChartType.OHLC:
					cPlot = new PlotOhlc(engine, source, chartSetting);
					break;
				case EChartType.ClosingLine:
					cPlot = new PlotClosingLine(engine, source, chartSetting);
					break;
				case EChartType.TextObject:
					cPlot = new PlotTextObject(engine, chart, source, chartSetting);
					break;
				case EChartType.TradeObject:
					cPlot = new PlotTradeObject(engine, source, chartSetting);
					break;
				case EChartType.CustomSharp:
					cPlot = ShapeEngine.Create(engine, chart, source, chartSetting);
					break;
			}
			return cPlot;
		}
	}
}