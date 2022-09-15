using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Actions;
using Zeghs.Drawing;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Chart {
	/// <summary>
	///   ZChart 金融圖表類別
	/// </summary>
	public sealed class ZChart : IDisposable {
		private bool __bBusy = false;
		private bool __bFirst = true;
		private bool __bUpdated = false;
		private bool __bDisposed = false;
		private bool __bNewSeries = false;
		private AxisX __cAxisX = null;
		private Control __cContext = null;
		private Behavior __cBehavior = null;
		private List<Layer> __cLayers = null;
		private TradeContainer __cTrades = null;
		private Instrument __cInstrument = null;
		private ChartProperty __cProperty = null;
		private InputDeviceStatus __cStatus = null;
		private AbstractPaintEngine __cPainter = null;
		private Rectangle __cChartRect = Rectangle.Empty;

		/// <summary>
		///   [取得] AxisX 座標軸
		/// </summary>
		public AxisX AxisX {
			get {
				return __cAxisX;
			}
		}

		/// <summary>
		///   [取得] 圖表屬性與設定參數值
		/// </summary>
		public ChartProperty ChartProperty {
			get {
				return __cProperty;
			}
		}

		/// <summary>
		///   [取得] 圖表矩形大小
		/// </summary>
		public Rectangle ChartRectangle {
			get {
				return __cChartRect;
			}
		}

		/// <summary>
		///   [取得] 自訂繪圖工具集合(繪圖引擎自訂的圖表繪製工具)
		/// </summary>
		public Dictionary<string, IAction> CustomDrawTools {
			get {
				return __cBehavior.CustomActions;
			}
		}

		/// <summary>
		///   [取得] Layer 圖層陣列
		/// </summary>
		public List<Layer> Layers {
			get {
				return __cLayers;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="context">繪製圖表內容的目標元件</param>
		/// <param name="chartEngine">圖表引擎資訊(ZChart 要使用的圖表引擎模組資訊)</param>
		public ZChart(Control context, ChartEngineInformation chartEngine) {
			__cChartRect = context.ClientRectangle;
			__cPainter = PaintManager.Manager.CreatePainter(context.Handle, chartEngine);

			__cLayers = new List<Layer>(8);
			__cStatus = new InputDeviceStatus();

			__cBehavior = new Behavior(this, context, __cStatus);
			__cBehavior.SetCustomActions(__cPainter.GetActions());

			__cContext = context;
			__cContext.Paint += context_onPaint;
			__cContext.Resize += context_onResize;
			__cContext.KeyDown += Context_onKeyDown;
			__cContext.KeyUp += Context_onKeyUp;
			__cContext.MouseUp += context_onMouseUp;
			__cContext.MouseDown += context_onMouseDown;
			__cContext.MouseMove += context_onMouseMove;
			__cContext.MouseWheel += context_onMouseWheel;
		}

		/// <summary>
		///   加入 Series 資料來源
		/// </summary>
		/// <param name="series">資料來源</param>
		/// <param name="data_stream">資料串流編號(從 1 開始編號)</param>
		public void AddSeries(object series, int data_stream) {
			if (__cAxisX == null && series is IInstrument) {
				__cInstrument = series as Instrument;
				__cInstrument.BindResetEvent(onReset);

				__cAxisX = new AxisX(__cInstrument);
				__cAxisX.AxisRectangle = __cChartRect;
				__cAxisX.FontMetrics = __cPainter.GetFontMetrics("0", __cProperty.AxisFont);
			}

			Layer cLayer = null;
			ChartSetting cSetting = __cProperty.ChartSettings[data_stream - 1];
			int iCount = __cLayers.Count;
			if (cSetting.LayerIndex < iCount) {
				cLayer = __cLayers[cSetting.LayerIndex];
			} else {
				cLayer = CreateLayer();
				cLayer.LayerIndex = iCount;

				__cLayers.Add(cLayer);
			}

			__bNewSeries = true;
			this.Resize();

			AbstractPlot cPlot = __cPainter.GetPlot(this, series, cSetting);
			if (cPlot != null) {
				cPlot.DataStream = data_stream;
				cPlot.AdjustAxisScaleFromX(__cAxisX);
				cLayer.AddPlot(cPlot);
			}
		}

		/// <summary>
		///   加入 DrwText 文字繪製容器
		/// </summary>
		/// <param name="drwText">文字繪製容器</param>
		public void AddDrwText(ITextContainer drwText) {
			TextContainer cContainer = drwText as TextContainer;

			int iCount =__cLayers.Count;
			__cProperty.ChartSettings.Add(
				new ChartSetting() {
					Axis = new AxisSetting(),
					IsSubChart = true,
					LayerIndex = iCount - 1,
					ChartType = EChartType.TextObject
			});

			cContainer.SetChartProperty(__cProperty);
			AddSeries(cContainer, __cProperty.ChartSettings.Count);

			//建立完畢後就從 ChartSettings 列表移除(因為 DrwText ChartSetting 屬性是由 ZChart 自行建立, 不是由使用者建立所以不用保存在列表內)
			int iChartCount = __cProperty.ChartSettings.Count;
			__cProperty.ChartSettings.RemoveAt(iChartCount - 1);
		}

		/// <summary>
		///   加入 PlotShape 物件(使用者自訂繪圖物件)
		/// </summary>
		/// <param name="args">Plot屬性類別</param>
		/// <param name="plotObject">Plot資料物件</param>
		/// <param name="data_stream">資料串流編號(從 1 開始編號)</param>
		public void AddPlotShape(PlotAttributes args, object plotObject, int data_stream) {
			if (__cAxisX != null) {
				Layer cLayer = null;
				ChartSetting cSetting = __cProperty.ChartSettings[data_stream - 1];

				//判斷是否要建立新的 Layer 圖層
				int iCount = __cLayers.Count;
				if (args.LayerIndex == -1 || args.LayerIndex >= iCount) {
					cLayer = CreateLayer();
					cLayer.LayerIndex = iCount;

					__cLayers.Add(cLayer);
				} else {
					cLayer = __cLayers[args.LayerIndex];
				}

				__bNewSeries = true;
				this.Resize();

				AxisSetting cAxis = cSetting.Axis.Clone();
				cAxis.IsCreateInstance = (args.LayerIndex == -1) ? true : !args.UseMasterAxisY;

				ChartSetting cPlotSetting = new ChartSetting() {
					Axis = cAxis,
					ChartType = EChartType.CustomSharp,
					IsShowNewPrice = args.ShowLastPrice,
					IsSubChart = true,
					LayerIndex = cSetting.LayerIndex,
					LegendColor = args.PenStyles[0].Color,
					PenStyles = args.PenStyles,
					PlotShape = args.PlotSharp
				};

				AbstractPlot cPlot = __cPainter.GetPlot(this, plotObject, cPlotSetting);
				if (cPlot != null) {
					cPlot.DataStream = data_stream;
					cPlot.AdjustAxisScaleFromX(__cAxisX);
					cLayer.AddPlot(cPlot);
				}
			}
		}

		/// <summary>
		///   加入交易資料容器(如果有加入交易資料容器則可以在圖表上顯示使用者交易線圖與交易資訊)
		/// </summary>
		/// <param name="container">交易資料容器</param>
		public void AddTradeContainer(TradeContainer container) {
			if (__cProperty.ChartSettings.Count > 0) {
				ChartSetting cSetting = __cProperty.ChartSettings[0];
				if (cSetting.IsSubChart) {
					__cProperty.ChartSettings.Add(
						new ChartSetting() {
							Axis = new AxisSetting(),
							IsSubChart = true,
							LayerIndex = 0,
							ChartType = EChartType.TradeObject,
							LegendColor = __cProperty.ForeColor
						});

					__cTrades = container;
					AddSeries(__cTrades, __cProperty.ChartSettings.Count);

					//建立完畢後就從 ChartSettings 列表移除(因為 TradeContainer ChartSetting 屬性是由 ZChart 自行建立, 不是由使用者建立所以不用保存在列表內)
					int iChartCount = __cProperty.ChartSettings.Count;
					__cProperty.ChartSettings.RemoveAt(iChartCount - 1);
				}
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   更新圖表
		/// </summary>
		public void Refresh() {
			__cContext.Invalidate();
		}

		/// <summary>
		///   設定圖表屬性
		/// </summary>
		/// <param name="property">圖表屬性類別</param>
		public void SetChartProperty(ChartProperty property) {
			__cProperty = property;
			__cContext.BackColor = __cProperty.BackgroundColor;
		}

		/// <summary>
		///   設定使用者自訂的動作介面(繪製操作介面)
		/// </summary>
		/// <param name="name">IAction 介面動作名稱</param>
		/// <param name="pen">筆刷樣式</param>
		/// <returns>返回值: true=設定成功, false=設定失敗</returns>
		public bool SetCustomAction(string name, PowerLanguage.PenStyle pen = null) {
			bool bRet = __cBehavior.SetCustomAction(name, pen);
			int iCount = __cLayers.Count;
			for (int i = 0; i < iCount; i++) {
				__cLayers[i].Drawable = bRet;
			}
			
			this.Refresh();
			return bRet;
		}

		/// <summary>
		///   設定使用者自訂畫筆樣式
		/// </summary>
		/// <param name="pen">筆刷樣式</param>
		public void SetCustomPenStyle(PowerLanguage.PenStyle pen) {
			__cBehavior.SetCustomPenStyle(pen);
		}

		public void onTradeResponse(object sender, ResponseEvent e) {
			if (__cTrades != null) {
				if (e.ResponseType == Orders.ResponseType.Deal) {
					bool bSuccess = __cTrades.AddTrades(e.OpenTrades, e.CloseTrades);
					if (bSuccess) {
						this.Refresh();
					}
				}
			}
		}

		/// <summary>
		///   用來附掛於 SignalObject.onUpdate 事件上的委託方法(此方法可以在 SignalObject 更新時通知 Chart 需要更新畫面)
		/// </summary>
		/// <param name="sender">呼叫者物件(這裡通常是 SignalObject)</param>
		/// <param name="e">EventArgs 類別</param>
		public void onUpdate(object sender, EventArgs e) {
			int iCount = __cLayers.Count;
			for (int i = 0; i < iCount; i++) {
				__cLayers[i].OnUpdate(__cInstrument.CurrentBar);
			}

			if (__bFirst) {
				if (__cInstrument.IsLastBars) {
					__bFirst = false;
					__cBehavior.Enabled = true;
					this.Refresh();
				}
			} else {
				int iBarNumber = __cAxisX.BarNumber + __cAxisX.BarCount - 1;
				int iDiff = __cInstrument.CurrentBar - ((iBarNumber > __cAxisX.DataCount) ? __cAxisX.DataCount : iBarNumber);
				switch (iDiff) {
					case 1:  //有新的 Bars 出現(自動移動到新的 Bars 並更新)
						if (!__bBusy) {
							__cAxisX.BarNumber = __cInstrument.CurrentBar;
							this.Refresh();
						}
						break;
					case 0:
						if (!__cAxisX.Refresh) {
							if (!__bBusy) {
								__bBusy = true;

								Task.Factory.StartNew(() => {
									__cPainter.Clear(__cProperty.BackgroundColor, __cChartRect, false);
									for (int i = 0; i < iCount; i++) {
										if (!__cPainter.DrawLayer(__cLayers[i], __cProperty, true)) {
											this.AxisX.Refresh = true;

											__bBusy = false;
											this.Refresh();
											return;
										}
									}
									__bBusy = false;
								});
							}
						}
						break;
				}
			}
		}

		internal void Resize() {
			int iCount = __cLayers.Count;
			if (iCount > 0) {
				Rectangle cAxisRect = __cAxisX.AxisRectangle;
				int iAxisX_Y = __cChartRect.Height - cAxisRect.Height;

				int iY = 0, iAxisYWidth = 0;
				double dTotalHeight = iAxisX_Y;
				for (int i = 0; i < iCount; i++) {
					Layer cLayer = __cLayers[i];
					if (__bNewSeries) {
						cLayer.HeightScale = 1d / iCount;
					}

					int iHeight = (int) (dTotalHeight * cLayer.HeightScale);
					cLayer.LayerRectangle = new Rectangle(__cChartRect.X, iY, __cChartRect.Width, iHeight);
					iY += iHeight;
				}

				//重新設定 AxisX 的寬度與 AxisX.Y 的位置
				cAxisRect.Y = iAxisX_Y;
				cAxisRect.Width = __cChartRect.Width - iAxisYWidth;
				__cAxisX.AxisRectangle = cAxisRect;

				__bNewSeries = false;
			}
		}

		private Layer CreateLayer() {
			Layer cLayer = new Layer(__cProperty);
			cLayer.AxisX = __cAxisX;
			return cLayer;
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					__bBusy = true;
					if (__cInstrument != null) {
						__cInstrument.ClearResetEvent(onReset);
					}

					__cContext.Paint -= context_onPaint;
					__cContext.Resize -= context_onResize;
					__cContext.KeyUp -= Context_onKeyUp;
					__cContext.KeyDown -= Context_onKeyDown;
					__cContext.MouseUp -= context_onMouseUp;
					__cContext.MouseDown -= context_onMouseDown;
					__cContext.MouseMove -= context_onMouseMove;
					__cContext.MouseWheel -= context_onMouseWheel;

					int iCount = __cLayers.Count;
					for (int i = 0; i < iCount; i++) {
						__cLayers[i].Dispose();
					}

					__cPainter.Dispose();
				}
			}
		}

		private void Draw() {
			if (!__bBusy) {
				__bBusy = true;
				__bUpdated = true;

				int iCount = __cLayers.Count;
				if (iCount > 0) {
					for (int i = 0; i < iCount; i++) {
						__cLayers[i].CalculatePlot();
					}
					ResizeAxisY();

					__cPainter.Clear(__cProperty.BackgroundColor, __cChartRect);
					__cPainter.DrawAxisX(__cAxisX, __cProperty);

					for (int i = 0; i < iCount; i++) {
						__cPainter.DrawLayer(__cLayers[i], __cProperty, false);
					}
					__cBehavior.DrawObjects();  //繪製使用者所繪製的物件
				}
				__bBusy = false;
			}
		}

		private void ProcessBehavior() {
			if (!__bBusy) {
				__bBusy = true;
				__cBehavior.Action(__bUpdated);
				
				__bUpdated = false;
				__bBusy = false;
			}
		}

		private void ResizeAxisY() {
			int iCount = __cLayers.Count;
			if (iCount > 0) {
				int iAxisYWidth = 0;
				for (int i = 0; i < iCount; i++) {
					Layer cLayer = __cLayers[i];
					AxisY cAxisY = cLayer.AxisY;
					if (cAxisY != null) {
						int iWidthY = cAxisY.AxisRectangle.Width;
						if (iWidthY > iAxisYWidth) {
							iAxisYWidth = iWidthY;
						}
					}
				}

				for (int i = 0; i < iCount; i++) {
					Layer cLayer = __cLayers[i];
					cLayer.ResizeAxisY(iAxisYWidth);
				}

				//重新設定 AxisX 的寬度
				Rectangle cAxisRect = __cAxisX.AxisRectangle;
				cAxisRect.Width = __cChartRect.Width - iAxisYWidth;
				__cAxisX.AxisRectangle = cAxisRect;
			}
		}

		private void onReset(object sender, EventArgs e) {
			int iCount = __cLayers.Count;
			if (iCount > 0) {
				int iAdjustSize = __cAxisX.SeriesCount - __cInstrument.FullSymbolData.Count;
				for (int i = 0; i < iCount; i++) {
					__cLayers[i].OnReset(iAdjustSize);
				}
			}
		}

		private void context_onPaint(object sender, PaintEventArgs e) {
			if (__bFirst) {
				Graphics g = e.Graphics;
				g.Clear(__cProperty.BackgroundColor);
				
				using (SolidBrush cBrush = new SolidBrush(__cProperty.ForeColor)) {
					g.DrawString("WES.Runtime.ChartEngine, Copyright © Web Electric Services. All rights reserved", __cContext.Font, cBrush, 0, 0);
					g.DrawString("Loading data is in progress, Please wait...", __cContext.Font, cBrush, 0, 15);

					string sChartTitle = "WES.ChartEngine";
					using (Font cFont = new Font("Impact", 24, FontStyle.Bold)) {
						SizeF cSize = g.MeasureString(sChartTitle, cFont);
						Rectangle cRect = __cContext.ClientRectangle;
						g.DrawString(sChartTitle, cFont, cBrush, (cRect.Width - cSize.Width) / 2, (cRect.Height - cSize.Height) / 2);
					}
				}
			} else {
				if (__bBusy) {
					this.Refresh();
				} else {
					this.Draw();
				}
			}
		}

		private void context_onResize(object sender, EventArgs e) {
			__cChartRect = __cContext.ClientRectangle;
			
			if (!__bBusy) {
				this.Resize();
				this.Refresh();
			}
		}

		private void Context_onKeyDown(object sender, KeyEventArgs e) {
			__cStatus.SetKeyEventArgs(EInputDeviceEvent.KeyDown, e);
			this.ProcessBehavior();
		}

		private void Context_onKeyUp(object sender, KeyEventArgs e) {
			__cStatus.SetKeyEventArgs(EInputDeviceEvent.KeyUp, e);
			this.ProcessBehavior();
		}

		private void context_onMouseDown(object sender, MouseEventArgs e) {
			__cStatus.SetMouseEventArgs(EInputDeviceEvent.MouseDown, e);
			this.ProcessBehavior();
		}

		private void context_onMouseMove(object sender, MouseEventArgs e) {
			__cStatus.SetMouseEventArgs(EInputDeviceEvent.MouseMove, e);
			this.ProcessBehavior();
		}

		private void context_onMouseUp(object sender, MouseEventArgs e) {
			__cStatus.SetMouseEventArgs(EInputDeviceEvent.MouseUp, e);
			this.ProcessBehavior();
		}

		private void context_onMouseWheel(object sender, MouseEventArgs e) {
			__cStatus.SetMouseEventArgs(EInputDeviceEvent.MouseWheel, e);
			this.ProcessBehavior();
		}
	}
}