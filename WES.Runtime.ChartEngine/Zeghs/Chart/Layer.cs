using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表 Layer 圖層類別
	/// </summary>
	public sealed class Layer : IDisposable {
		private int __iMaxPlotCount = 0;
		private bool __bDisposed = false;
		private ChartProperty __cProperty = null;
		private List<AbstractPlot> __cPlots = null;
		private Rectangle __cLayerRect = Rectangle.Empty;

		/// <summary>
		///   [取得] AxisX 座標軸
		/// </summary>
		public AxisX AxisX {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] AxisY 座標軸
		/// </summary>
		public AxisY AxisY {
			get {
				if (__cPlots.Count > 0) {
					return __cPlots[0].AxisY;
				}
				return null;
			}
		}

		/// <summary>
		///   [取得/設定] 是否在繪圖模式
		/// </summary>
		public bool Drawable {
			get;
			set;
		}

		/// <summary>
		///   [取得] Layer 圖層索引值
		/// </summary>
		public int LayerIndex {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] Layer 區域範圍(包含 AxisY 軸的全部區塊)
		/// </summary>
		public Rectangle LayerRectangle {
			get {
				return __cLayerRect;
			}

			set {
				__cLayerRect = value;
			}
		}

		/// <summary>
		///   [取得] Layer 區域範圍(不包含 AxisY 軸的區塊)
		/// </summary>
		public Rectangle LayerRectangleWithoutAxisY {
			get {
				Rectangle cRect = __cLayerRect;
				cRect.Width = this.AxisY.AxisRectangle.X - 1;
				return cRect;
			}
		}

		/// <summary>
		///   [取得/設定] 目前 Legend 資料內容索引值
		/// </summary>
		public int LegendIndex {
			get;
			set;
		}

		/// <summary>
		///   [取得] Plot 繪製類別列表
		/// </summary>
		public List<AbstractPlot> Plots {
			get {
				return __cPlots;
			}
		}

		/// <summary>
		///   [取得/設定] Layer 高度比率(1=100%, 0.5=50%)
		/// </summary>
		internal double HeightScale {
			get;
			set;
		}

		internal Layer(ChartProperty property) {
			__cProperty = property;
			__cPlots = new List<AbstractPlot>(4);
			
			this.LegendIndex = 1;  //預設為第一根 Bar 索引值
		}

		/// <summary>
		///   釋放 Layer 所有資源
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   是否在此 Layer 範圍內
		/// </summary>
		/// <param name="x">滑鼠 X 座標</param>
		/// <param name="y">滑鼠 Y 座標</param>
		/// <returns>返回值: true=在範圍內, false=不再範圍內</returns>
		public bool IsLayerScope(int x, int y) {
			int iRight = __cLayerRect.X + __cLayerRect.Width;
			int iBottom = __cLayerRect.Y + __cLayerRect.Height;
			return x >= __cLayerRect.X && x <= iRight && y >= __cLayerRect.Y && y <= iBottom;
		}

		internal void AddPlot(AbstractPlot plot, AxisY axisY = null) {
			if (axisY == null) {
				if (__cPlots.Count == 0 || plot.ChartSetting.Axis.IsCreateInstance) {
					Rectangle cAxisRect = this.LayerRectangle;
					cAxisRect.Width = 0;  //將寬度設定為0(讓 AxisY 自動計算寬度)

					plot.CreateAxisY(__cProperty, cAxisRect);
				} else {
					plot.AxisY = __cPlots[0].AxisY;
				}
			} else {
				plot.AxisY = axisY;
			}

			EChartType cChartType = plot.ChartSetting.ChartType;
			switch (cChartType) {
				case EChartType.TextObject:
				case EChartType.TradeObject:
					plot.PlotIndex = -1;
					break;
				default:
					plot.PlotIndex = __iMaxPlotCount++;
					break;
			}
			__cPlots.Add(plot);
		}

		internal void CalculatePlot() {
			int iCount = __cPlots.Count;
			if (iCount > 0) {
				AxisY cAxisY = this.AxisY;
				cAxisY.Reset();

				for (int i = 0; i < iCount; i++) {
					if (__cPlots[i].AxisY != cAxisY) {
						__cPlots[i].AxisY.Reset();
					}

					__cPlots[i].CalculatePlot(this.AxisX);
				}
			}
		}

		internal void OnReset(int size) {
			int iCount = __cPlots.Count;
			for (int i = 0; i < iCount; i++) {
				__cPlots[i].AdjustAxisScaleSize(size);
			}
		}

		internal void OnUpdate(int sourceIndex) {
			int iCount = __cPlots.Count;
			for (int i = 0; i < iCount; i++) {
				__cPlots[i].SetAxisScale(sourceIndex);
			}
		}

		internal void ResizeAxisY(int width) {
			if (__cPlots.Count > 0) {
				AxisY cAxisY_0 = this.AxisY;
				cAxisY_0.SetAxisRectangle(__cLayerRect, width);

				int iCount = __cPlots.Count;
				for (int i = 1; i < iCount; i++) {
					AxisY cAxisY = __cPlots[i].AxisY;
					if (cAxisY != cAxisY_0) {
						cAxisY.SetAxisRectangle(__cLayerRect, width);
					}
				}
			}
		}
		
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					int iCount = __cPlots.Count;
					for (int i = 0; i < iCount; i++) {
						__cPlots[i].Dispose();
					}
					__cPlots.Clear();
				}
			}
		}
	}
}