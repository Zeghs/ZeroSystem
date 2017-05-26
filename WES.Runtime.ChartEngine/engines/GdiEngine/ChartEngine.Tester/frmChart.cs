using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Chart;
using Zeghs.Scripts;
using Zeghs.Products;
using Zeghs.Managers;

namespace ChartEngine.Tester {
	public partial class frmChart : Form {
		private ZChart __cChart = null;

		public frmChart() {
			InitializeComponent();
			__cChart = new ZChart(chart, PaintManager.Manager.GetChartEngineInformations()[0]);
		}

		private void CreateChart() {
			ChartSetting cSetting1 = new ChartSetting() {
				Axis = new AxisSetting() {
					AxisScope = EAxisScope.PriceScaleScope,
					ScaleMode = EAxisScaleMode.None,
					ScaleValue = 0,
					MarginTop = 0,
					MarginBottom = 0
				},
				IsSubChart = true,
				IsShowNewPrice = true,
				LayerIndex = 0,
				LegendColor = Color.Yellow,
				ChartType = EChartType.Candlestick,
				PenStyles = new PowerLanguage.PenStyle[] {
					new PowerLanguage.PenStyle(Color.Red, 1),
					new PowerLanguage.PenStyle(Color.Green, 1),
					new PowerLanguage.PenStyle(Color.Gray, 1),
				 }
			};

			ChartSetting cSetting2 = new ChartSetting() {
				Axis = new AxisSetting() {
					AxisScope = EAxisScope.PriceScaleScope,
					ScaleMode = EAxisScaleMode.None,
					ScaleValue = 0,
					MarginTop = 0,
					MarginBottom = 0
				},
				IsSubChart = true,
				IsShowNewPrice = true,
				LayerIndex = 1,
				LegendColor = Color.Red,
				ChartType = EChartType.OHLC,
				PenStyles = new PowerLanguage.PenStyle[] {
					new PowerLanguage.PenStyle(Color.Red, 1),
					new PowerLanguage.PenStyle(Color.Green, 1),
					new PowerLanguage.PenStyle(Color.Gray, 1),
				 }
			};

			Font cFont = new System.Drawing.Font("新細明體", 8);
			ChartProperty cProperty = new ChartProperty() {
				AxisFont = cFont,
				AxisColor = Color.Yellow,
				BackgroundColor = Color.Black,
				DrawingSource = EDrawingSource.CurrentTech,
				DrawAideLineColor = Color.LightSkyBlue,
				ForeColor = Color.White,
				GridColor = Color.FromArgb(0x30, 0x30, 0x30),
				LegendFont = cFont,
				TitleFont = cFont,
				TextFont = cFont,
				IsShowGrid = true,
				TradeLineColor = Color.GreenYellow,
				TradeSymbolColor = Color.DodgerBlue,
				ChartSettings = new List<ChartSetting>() { 
					cSetting1, 
					cSetting2
				}
			};

			__cChart.SetChartProperty(cProperty);

			SignalObject cObject = new TestScript();
			cObject.onReady += SignalObject_onReady;
			cObject.ApplyProperty(new SignalProperty() {
				DefaultContracts = 1,
				InitialCapital = 100000,
				MaximumBarsReference = 0,
				OrderSource = "Netwings.OrderService;Netwings.SimulateOrderService"
			});

			cObject.AddDataStreams(new List<InstrumentDataRequest>() {
				new InstrumentDataRequest() {
					Exchange = "TWSE",
					DataFeed = "Mitake",
					Range = new DataRequest(DateTime.Today, 1000, DataRequestType.BarsBack, DateTime.Today),
					Symbol = "TXF0.tw",
					Resolution = new Resolution(EResolution.Minute, 1)
				},
				new InstrumentDataRequest() {
					Exchange = "TWSE",
					DataFeed = "Mitake",
					Range = new DataRequest(DateTime.Today, 1000, DataRequestType.BarsBack, DateTime.Today),
					Symbol = "TXF0.tw",
					Resolution = new Resolution(EResolution.Minute, 5)
				}
			});
		}

		private void frmChart_Load(object sender, EventArgs e) {
			CreateChart();
		}

		private void itemPointer_Click(object sender, EventArgs e) {
			__cChart.SetCustomAction("pointer");
		}

		private void itemCross_Click(object sender, EventArgs e) {
			__cChart.SetCustomAction("Cross");
		}

		private void itemPaint_Click(object sender, EventArgs e) {
			__cChart.SetCustomAction("Ray", new PenStyle(Color.Yellow, 1));
		}

		private void SignalObject_onReady(object sender, EventArgs e) {
			SignalObject cObject = sender as SignalObject;

			int iCount = cObject.MaxDataStream;
			for (int i = 1; i <= iCount; i++) {
				__cChart.AddSeries(cObject.BarsOfData(i), i);
			}

			__cChart.AddDrwText(cObject.DrwText);
			__cChart.AddTradeContainer(new TradeContainer());
			cObject.onUpdate += __cChart.onUpdate;
			cObject.onTradeResponse += __cChart.onTradeResponse;
		}
	}
}
