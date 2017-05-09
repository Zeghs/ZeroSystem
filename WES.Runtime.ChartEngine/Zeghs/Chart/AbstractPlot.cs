using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Rules;

namespace Zeghs.Chart {
	/// <summary>
	///   抽象 Plot 繪製類別
	/// </summary>
	public abstract class AbstractPlot : IDisposable {
		protected static AxisY CreateAxisY(AbstractPaintEngine engine, AxisSetting axisSetting, IPriceScale scale, ChartProperty property, Rectangle axisRectangle) {
			AxisY cAxisY = new AxisY(axisSetting, scale);
			cAxisY.AxisRectangle = axisRectangle;
			cAxisY.FontMetrics = engine.GetFontMetrics("0", property.AxisFont);
			return cAxisY;
		}

		private bool __bDisposed = false;
		private List<int> __cScaleX = null;
		private ChartSetting __cSetting = null;
		private AbstractPaintEngine __cEngine = null;

		/// <summary>
		///   [取得] 資料串流編號(編號從 1 開始)
		/// </summary>
		public int DataStream {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 是否為副圖(true=副圖表, false=隱藏)
		/// </summary>
		public bool IsSubChart {
			get {
				return __cSetting.IsSubChart;
			}
		}

		/// <summary>
		///   [取得] Plot 索引值
		/// </summary>
		public int PlotIndex {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] AxisY 座標軸
		/// </summary>
		internal protected AxisY AxisY {
			get;
			set;
		}

		/// <summary>
		///   [取得] 圖表設定值
		/// </summary>
		internal protected ChartSetting ChartSetting {
			get {
				return __cSetting;
			}
		}

		/// <summary>
		///   [取得] 資料目前索引值
		/// </summary>
		protected abstract int Current {
			get;
		}

		/// <summary>
		///   [取得] 序列資料容器總個數
		/// </summary>
		protected abstract int SeriesCount {
			get;
		}

		/// <summary>
		///   [取得] 繪圖引擎抽象類別
		/// </summary>
		protected AbstractPaintEngine Painter {
			get {
				return __cEngine;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="chartSetting">圖表設定值</param>
		public AbstractPlot(AbstractPaintEngine engine, ChartSetting chartSetting) {
			__cEngine = engine;
			__cSetting = chartSetting;
		}

		/// <summary>
		///   釋放 AbstractPlot 所有資源
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   計算 Plot 圖表區間內的所需 AxisY 軸最高與最低刻度資訊
		/// </summary>
		/// <param name="axis">AxisX 軸類別(需要取得 X 軸的區間範圍)</param>
		public abstract void CalculatePlot(AxisX axis);

		/// <summary>
		///   繪製 Plot 圖表
		/// </summary>
		/// <param name="layer">Layer 圖層類別</param>
		/// <param name="property">Chart 屬性參數</param>
		/// <param name="onlyUpdateLastBar">僅更新最後一根Bar</param>
		/// <returns>返回值: true=繪製完成, false=繪製中止</returns>
		public abstract bool DrawPlot(Layer layer, ChartProperty property, bool onlyUpdateLastBar);

		/// <summary>
		///   繪製 Legend 圖例
		/// </summary>
		/// <param name="barNumber">Bar 編號值</param>
		/// <param name="property">Chart 屬性參數</param>
		internal protected abstract void DrawLegend(int barNumber, ChartProperty property);

		/// <summary>
		///   調整 AxisX 軸的刻度基準(如果 X 軸資料刻度與 Plot 資料刻度不符合，需要調整轉換)
		/// </summary>
		/// <param name="axis">圖表 X 座標軸類別</param>
		internal void AdjustAxisScaleFromX(AxisX axis) {
			if (this.SeriesCount > 0 && this.SeriesCount != axis.SeriesCount) {
				__cScaleX = new List<int>(new int[axis.DataCount]);
			}
		}

		/// <summary>
		///   擴充 AxisX 軸的刻度大小
		/// </summary>
		/// <param name="size">擴充大小</param>
		internal void AdjustAxisScaleSize(int size) {
			if (__cScaleX != null) {
				if (size > 0) {
					__cScaleX.AddRange(new int[size]);
				}
			}
		}

		/// <summary>
		///   設定 AxisX 軸的刻度(將原始刻度的 index 索引值與資料內正確的索引值保存至列表內)
		/// </summary>
		/// <param name="sourceIndex">原始資料索引值(起始為 1 所以需要減 1 才會對應至列表索引 0)</param>
		internal void SetAxisScale(int sourceIndex) {
			if (__cScaleX != null) {
				if (sourceIndex > __cScaleX.Count) {
					__cScaleX.Add(this.Current);
				} else {
					__cScaleX[sourceIndex - 1] = this.Current;
				}
			}
		}

		/// <summary>
		///   建立 AxisY 座標軸
		/// </summary>
		/// <param name="property">Chart 屬性參數</param>
		/// <param name="axisRectangle">AxisY 座標軸區域</param>
		internal protected virtual void CreateAxisY(ChartProperty property, Rectangle axisRectangle) {
			this.AxisY = CreateAxisY(__cEngine, this.ChartSetting.Axis, null, property, axisRectangle);
		}

		/// <summary>
		///   轉換 AxisX 軸的刻度的索引值
		/// </summary>
		/// <param name="sourceIndex">原始資料索引值</param>
		/// <returns>返回值: 轉換後的偏移索引值</returns>
		protected int ConvertAxisScaleIndexToOffset(int sourceIndex) {
			if (__cScaleX != null) {
				return this.Current - __cScaleX[sourceIndex - 1];
			}
			return this.Current - sourceIndex;
		}

		/// <summary>
		///   釋放 AbstractPlot 所有資源(繼承後可複寫方法)
		/// </summary>
		/// <param name="disposing">是否處理受託管的資源</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					if (__cScaleX != null) {
						__cScaleX.Clear();
					}
				}
			}
		}

		/// <summary>
		///   更新 AxisY 軸資訊
		/// </summary>
		/// <param name="property">圖表屬性類別</param>
		protected void RefreshAxisY(ChartProperty property) {
			if (this.PlotIndex == 0) {
				__cEngine.DrawAxisY(this.AxisY, property);
			} else {
				this.AxisY.CalculateAxisScale();
			}
		}
	}
}