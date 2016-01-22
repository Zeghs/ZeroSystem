using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using PowerLanguage.Function;
using Zeghs.Utils;
using Zeghs.Orders;
using Zeghs.Scripts;

namespace PowerLanguage.Strategy {
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "小澤源", Copyright = "Copyright © 2014 小澤源. 保留一切權利。", Comment = "KDJ_MACD")]
	public sealed class __KDJ_MACD_Signal : SignalObject {
		private static TimeSpan __cStartTime = new TimeSpan(9, 15, 0);
		private static TimeSpan __cClosedTime = new TimeSpan(13, 0, 0);
		private static TimeSpan __cTradeClosed = new TimeSpan(13, 25, 0);

		private KD __cKD = null;
		private MACD __cMACD = null;
		private IOrderMarket BUY, BUY_C, SELL, SELL_C;

		private int 下單口數 = 1;
		private bool __bTrade = false;  //是否可以交易
		private double __dOSDTotals = 0;
		private DateTime __cPrevDate = DateTime.MinValue;

		public __KDJ_MACD_Signal(object _ctx)
			: base(_ctx) {
		}

		protected override void Create() {
			// 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
			BUY = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy, OrderExit.FromAll));
			SELL = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort, OrderExit.FromAll));
			BUY_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			SELL_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));

			__cKD = new KD(this);
			__cMACD = new MACD(this);
		}

		protected override void CalcBar() {
			DateTime cDateTime = Bars.Time[0];
			TimeSpan cTime = cDateTime.TimeOfDay;
			if (cDateTime.Date > __cPrevDate.Date) {
				__bTrade = true;
				__cPrevDate = cDateTime;
			}

			if (__bTrade) {
				if (CurrentPosition.Side == EMarketPositionSide.Flat) {
					SendOrder();
				} else if (cTime >= __cTradeClosed) {
					__bTrade = false;
					SendTradeClosing();
				} else {
					SendClose();
				}
			}
		}

		protected override void Destroy() {
			TradeReport.Save("report.csv", Bars.Request.Symbol, this.Positions);
		}

		private void SendOrder() {
			double dOSD = __cMACD.OSD[1];
			if (__dOSDTotals > 0d && dOSD > 0d) {
				__dOSDTotals += dOSD;
			} else if (__dOSDTotals < 0d && dOSD < 0d) {
				__dOSDTotals += dOSD;
			} else {
				__dOSDTotals = dOSD;
			}

			double dKDJ = __cKD[1];
			if (dKDJ < 10d && __dOSDTotals < -50d) {
				BUY.Send(string.Format("Buy[J={0} & OSD={1}]", dKDJ.ToString("N2"), __dOSDTotals.ToString("N2")), 下單口數);
			} else if (dKDJ > 90d && __dOSDTotals > 50d) {
				SELL.Send(string.Format("SELL[J={0} & OSD={1}]", dKDJ.ToString("N2"), __dOSDTotals.ToString("N2")), 下單口數);
			}
		}

		private void SendClose() {
			double dKDJ = __cKD[0];
			if (CurrentPosition.Side == EMarketPositionSide.Long && dKDJ > 90) {
				BUY_C.Send(string.Format("Close[J={0}]", dKDJ.ToString("N2")));
			} else if (CurrentPosition.Side == EMarketPositionSide.Short && dKDJ < 10) {
				SELL_C.Send(string.Format("Close[J={0}]", dKDJ.ToString("N2")));
			}
		}

		private void SendTradeClosing() {
			if (CurrentPosition.Side == EMarketPositionSide.Long) {
				BUY_C.Send("TradeClosing");
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				SELL_C.Send("TradeClosing");
			}
		}
	}
}