using PowerLanguage;
using Zeghs.Chart;
using Zeghs.Drawing.PlotShapes;

namespace Zeghs.Drawing.PlotSharps {
	internal sealed class ShapeEngine {
		internal static AbstractPlot Create(GdiEngine engine, ZChart chart, object source, ChartSetting chartSetting) {
			AbstractPlot cPlot = null;
			EPlotShapes cPlotShape = chartSetting.PlotShape;
			switch (cPlotShape) {
				case EPlotShapes.Line:
					cPlot = new PlotShapeLine(engine, source, chartSetting);
					break;
			}
			return cPlot;
		}
	}
}