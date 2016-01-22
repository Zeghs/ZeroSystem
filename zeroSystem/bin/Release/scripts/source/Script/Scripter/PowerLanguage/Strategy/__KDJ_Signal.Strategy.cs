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
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "小澤源", Copyright = "Copyright © 2014 小澤源. 保留一切權利。", Comment = "KDJ")]
	public sealed class __KDJ_Signal : SignalObject {
		private static TimeSpan __cStartTime = new TimeSpan(9, 15, 0);
		private static TimeSpan __cClosedTime = new TimeSpan(13, 0, 0);
		private static TimeSpan __cTradeClosed = new TimeSpan(13, 25, 0);

		private KD __cKD = null;
		private IOrderMarket BUY, BUY_C, SELL, SELL_C;

		private int 下單口數 = 1;        //下單口數
		private int 交易次數 = 5;        //交易次數
		private int __iTradeCount = 0;  //目前交易次數
		private DateTime __cPrevDate = DateTime.MinValue;

		public __KDJ_Signal(object _ctx)
			: base(_ctx) {
		}

		protected override void Create() {
			// 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
			BUY = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy, OrderExit.FromAll));
			SELL = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort, OrderExit.FromAll));
			BUY_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			SELL_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));

			__cKD = new KD(this);
		}

		protected override void CalcBar() {
			DateTime cDateTime = Bars.Time[0];
			TimeSpan cTime = cDateTime.TimeOfDay;
			if (cDateTime.Date > __cPrevDate.Date) {
				__iTradeCount = 0;
				__cPrevDate = cDateTime;
			}

			if (cTime > __cStartTime) {
				if (__cKD[0] < 0d) {
					if (cTime < __cClosedTime) {
						if (__iTradeCount < 交易次數) {
							if (CurrentPosition.Side != EMarketPositionSide.Flat) {
								++__iTradeCount;
							}
							BUY.Send("KDJ_BUY", 下單口數);
						}
					} else {
						SendClose("KDJ_TradingClosed");
					}
				} else if (__cKD[0] > 90d) {
					if (cTime < __cClosedTime) {
						if (__iTradeCount < 交易次數) {
							if (CurrentPosition.Side != EMarketPositionSide.Flat) {
								++__iTradeCount;
							}
							SELL.Send("KDJ_SELL", 下單口數);
						}
					} else {
						SendClose("KDJ_TradingClosed");
					}
				} else if (cTime >= __cTradeClosed) {  //如果都沒有符合條件就在 13:25:00 平倉
					SendClose("KDJ_TradingClosed");
				}
			}
		}

		protected override void Destroy() {
			TradeReport.Save("report.csv", Bars.Request.Symbol, this.Positions);
		}

		private void SendClose(string memo) {
			if (CurrentPosition.Side != EMarketPositionSide.Flat) {
				if (CurrentPosition.Side == EMarketPositionSide.Long) {
					BUY_C.Send(memo);
				} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
					SELL_C.Send(memo);
				}
			}
		}
	}
}