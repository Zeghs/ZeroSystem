using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Actions;

namespace Zeghs.Chart {
	public abstract class AbstractPaintEngine : IDisposable {
		/// <summary>
		///   釋放報價服務的所有資源
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   繪製 Legend 標籤
		/// </summary>
		/// <param name="layer">Layer 圖層</param>
		/// <param name="property">Chart 屬性參數</param>
		public void DrawLegend(Layer layer, ChartProperty property) {
			List<AbstractPlot> cPlots = layer.Plots;
			int iCount = cPlots.Count;
			for (int i = 0; i < iCount; i++) {
				cPlots[i].DrawLegend(layer.LegendIndex, property);
			}
		}

		/// <summary>
		///    清除畫面
		/// </summary>
		/// <param name="color">欲清除的背景顏色</param>
		/// <param name="rectangle">欲清除的範圍區間</param>
		/// <param name="isClearAll">是否要清除全部的繪製物件</param>
		public abstract void Clear(Color color, Rectangle rectangle, bool isClearAll = true);

		/// <summary>
		///   繪製 AxisY 軸的數值標籤(價格線或數值線標籤)
		/// </summary>
		/// <param name="axis">AxisY 座標軸</param>
		/// <param name="lineColor">標籤線色彩</param>
		/// <param name="property">Chart 屬性參數</param>
		public abstract void DrawAxisValueLabel(AxisY axis, Color lineColor, ChartProperty property);

		/// <summary>
		///    繪製 AxisX 座標軸
		/// </summary>
		/// <param name="axis">AxisX 座標軸</param>
		/// <param name="property">Chart 屬性參數</param>
		public abstract void DrawAxisX(AxisX axis, ChartProperty property);

		/// <summary>
		///    繪製 AxisY 座標軸
		/// </summary>
		/// <param name="axis">AxisY 座標軸</param>
		/// <param name="property">Chart 屬性參數</param>
		public abstract void DrawAxisY(AxisY axis, ChartProperty property);

		/// <summary>
		///   繪製 Layer 圖層
		/// </summary>
		/// <param name="layer">Layer 圖層</param>
		/// <param name="property">Chart 屬性參數</param>
		/// <param name="onlyUpdateLastBar">僅更新最後一根Bar</param>
		/// <returns>返回值:true=繪製 Layer 完成, false=繪製 Layer 未完成</returns>
		public abstract bool DrawLayer(Layer layer, ChartProperty property, bool onlyUpdateLastBar);

		/// <summary>
		///   取得字型長度與寬度
		/// </summary>
		/// <param name="text">計算 Size 的文字字串</param>
		/// <param name="font">字型類別</param>
		/// <returns>回傳值: Size 結構</returns>
		public abstract Size GetFontMetrics(string text, Font font);

		/// <summary>
		///   取得 Plot 物件
		/// </summary>
		/// <param name="chart">ZChart 圖表類別</param>
		/// <param name="source">資料來源</param>
		/// <param name="chartSetting">圖表設定值</param>
		/// <returns>回傳值: AbstractPlot 抽象類別</returns>
		public abstract AbstractPlot GetPlot(ZChart chart, object source, ChartSetting chartSetting);

		/// <summary>
		///   取得使用者自訂的操作/動作列表
		/// </summary>
		/// <returns>返回值: 使用者操作/動作列表</returns>
		internal protected abstract List<IAction> GetActions();

		/// <summary>
		///   釋放 AbstractPaintEngine 所有資源(繼承後可複寫方法)
		/// </summary>
		/// <param name="disposing">是否處理受託管的資源</param>
		protected abstract void Dispose(bool disposing);
	}
}