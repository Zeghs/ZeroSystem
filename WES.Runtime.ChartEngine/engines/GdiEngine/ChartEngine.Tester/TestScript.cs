using System;
using System.Drawing;
using PowerLanguage;

namespace ChartEngine.Tester {
	public sealed class TestScript : SignalObject {
		private int iii = 0;
		private int current = 0;
		private IOrderMarket BUY, SELL;
		private IOrderMarket BUY_C, SELL_C;

		public TestScript(object _ctx)
			: base(_ctx) {
		}

		ITextObject cText = null;
		IPlotObject<double> cObjects = null;
		protected override void Create() {
			BUY = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
			SELL = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
			BUY_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			SELL_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));

			cObjects = AddPlot<double>(new PlotAttributes("ZEGHS", EPlotShapes.Line, Color.White));

			cText = this.DrwText.Create(new ChartPoint(1, 150), "1234", true);
			cText.Size = 13;
			cText.Color = Color.Yellow;
			cText.BGColor = Color.Red;
			cText.SetFont(System.Drawing.FontStyle.Bold, true);
		}

		protected override void CalcBar() {
			if ((Bars.CurrentBar % 5 == 0) && current < Bars.CurrentBar) {
				//*
				if (this.CurrentPosition.Side == EMarketPositionSide.Flat) {
					BUY.Send("BUY", 1);
					current = Bars.CurrentBar;
				} else {
					BUY_C.Send("SELL");
					current = Bars.CurrentBar;
				}
				//*/

				//*
				++iii;
				current = Bars.CurrentBar;
				ITextObject cObject = this.DrwText.Create(new ChartPoint(Bars.Low[0]), iii.ToString());
				cObject.HStyle = ETextStyleH.Center;

				cObject = this.DrwText.Create(new ChartPoint(BarsOfData(2).Low[0]), iii.ToString(), 2);
				cObject.HStyle = ETextStyleH.Center;
				//*/
				cObjects.Set(Bars.High.Value + 5, Color.Blue);
			}

			cText.Text = DateTime.Now.ToString();
			cObjects.Set(Bars.High.Value + 5);
		}
	}
}
