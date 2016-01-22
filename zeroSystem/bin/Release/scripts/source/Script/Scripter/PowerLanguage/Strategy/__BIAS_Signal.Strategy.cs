//#define HISTORY
#define STOP_PROFIT

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
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.1.0.0", Company = "小澤源", Copyright = "Copyright © 2014 小澤源. 保留一切權利。", Comment = "BIAS")]
	public sealed class __BIAS_Signal : SignalObject {
		private class MonthReport {
			internal double Count = 0;
			internal double MaxLossCount = 0;
			internal double MaxRunUp = 0;
			internal double MaxDrawDown = 0;
			internal double LossCount = 0;
			internal double LossTotals = 0;
			internal double WinCount = 0;
			internal double WinTotals = 0;


			internal void SetMaxLossCount(double count) {
				if (count > MaxLossCount) {
					MaxLossCount = count;
				}
			}

			internal void SetMaxRunUp(double totals) {
				if (totals > MaxRunUp) {
					MaxRunUp = totals;
				}
			}

			internal void SetMaxDrawDown(double totals) {
				if (totals < MaxDrawDown) {
					MaxDrawDown = totals;
				}
			}
		}

		private static void Save(string filename, string symbolId, ISeries<IMarketPosition> positions) {
			if (File.Exists(filename)) {
				File.Delete(filename);
			}

			PositionSeries cPositions = positions as PositionSeries;
			StringBuilder cBuilder = new StringBuilder(1024 * 1024);
			cBuilder.Append("NO.").Append(',').Append("SymbolID").Append(',').Append("Category").Append(',').Append("Action").Append(',').Append("Volume").Append(',').Append("Price").Append(',').Append("Profit").Append(',').Append("Fee").Append(',').Append("Tax").Append(',').Append("Trading time").Append(',').Append("Description").Append(',').Append("MaxRunUp").Append(',').AppendLine("MaxDrawDown");

			Dictionary<string, MonthReport> cMonthRpts = new Dictionary<string, MonthReport>();

			double dWin = 0, dLoss = 0, dWinT = 0, dLossT = 0, dLossCount = 0, dMaxLossCount = 0;
			int iCount = cPositions.Count;
			for (int i = 0; i < iCount; i++) {
				IMarketPosition cPosition = cPositions[i];
				if (cPosition.Value > 0) {
					List<ITrade> cTrades = cPosition.ClosedTrades;
					foreach (ITrade cTrade in cTrades) {
						ITradeOrder cOpenO = cTrade.EntryOrder;
						ITradeOrder cCloseO = cTrade.ExitOrder;

						MonthReport cReport = null;
						string sMonth = cCloseO.Time.ToString("yyyy/MM");
						if (!cMonthRpts.TryGetValue(sMonth, out cReport)) {
							cReport = new MonthReport();
							cMonthRpts.Add(sMonth, cReport);
						}

						++cReport.Count;

						double dProfit = cTrade.Profit;
						if (dProfit < 0) {
							++dLoss;
							dLossT += dProfit;
							++dLossCount;

							++cReport.LossCount;
							cReport.LossTotals += dProfit;
							cReport.SetMaxLossCount(dLossCount);
							cReport.SetMaxRunUp(dProfit);
							cReport.SetMaxDrawDown(dProfit);

							if (dLossCount > dMaxLossCount) {
								dMaxLossCount = dLossCount;
							}
						} else {
							++dWin;
							dWinT += dProfit;
							dLossCount = 0;

							++cReport.WinCount;
							cReport.WinTotals += dProfit;
							cReport.SetMaxRunUp(dProfit);
							cReport.SetMaxDrawDown(dProfit);
						}

						cBuilder.Append(cOpenO.Ticket).Append(',').Append(symbolId).Append(',').Append(cOpenO.Category).Append(',').Append(cOpenO.Action).Append(',').Append(cOpenO.Contracts).Append(',').Append(cOpenO.Price).Append(',').Append(string.Empty).Append(',').Append(cOpenO.Fee).Append(',').Append(cOpenO.Tax).Append(',').Append(cOpenO.Time.ToString("yyyy/MM/dd HH:mm:ss")).Append(',').Append(cOpenO.Name).Append(',').Append(string.Empty).Append(',').AppendLine(string.Empty);
						cBuilder.Append(cCloseO.Ticket).Append(',').Append(symbolId).Append(',').Append(cCloseO.Category).Append(',').Append(cCloseO.Action).Append(',').Append(cCloseO.Contracts).Append(',').Append(cCloseO.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cCloseO.Fee).Append(',').Append(cCloseO.Tax).Append(',').Append(cCloseO.Time.ToString("yyyy/MM/dd HH:mm:ss")).Append(',').Append(cCloseO.Name).Append(',').Append(cPosition.MaxRunUp.ToString()).Append(',').AppendLine(cPosition.MaxDrawDown.ToString());
					}
				}
			}

			cBuilder.AppendLine();
			cBuilder.Append(',').Append("Max Loss:").Append(',').AppendLine(dMaxLossCount.ToString());
			cBuilder.Append(',').Append("Win totals:").Append(',').Append(dWinT.ToString()).Append(',').Append(',').Append("Loss totals:").Append(',').AppendLine(dLossT.ToString());
			cBuilder.Append(',').Append("Win (%):").Append(',').Append((dWin / iCount * 100).ToString("F2")).Append(',').Append(',').Append("Loss (%):").Append(',').AppendLine((dLoss / iCount * 100).ToString("F2"));

			cBuilder.AppendLine();
			cBuilder.Append(',').Append("Month").Append(',').Append("Win totals").Append(',').Append("Win (%)").Append(',').Append("Loss totals").Append(',').Append("Loss (%)").Append(',').Append("Max Loss").Append(',').Append("MaxRunUp").Append(',').AppendLine("MaxDrawDown");
			foreach (string sMonth in cMonthRpts.Keys) {
				MonthReport cReport = cMonthRpts[sMonth];
				cBuilder.Append(',').Append(sMonth).Append(',').Append(cReport.WinTotals).Append(',').Append((cReport.WinCount / cReport.Count * 100).ToString("F2")).Append(',').Append(cReport.LossTotals).Append(',').Append((cReport.LossCount / cReport.Count * 100).ToString("F2")).Append(',').Append(cReport.MaxLossCount.ToString()).Append(',').Append(cReport.MaxRunUp).Append(',').AppendLine(cReport.MaxDrawDown.ToString());
			}

			File.WriteAllText(filename, cBuilder.ToString(), Encoding.UTF8);
		}
		
		private static int[] __iTable = new int[] { -180, -176, -173, -169, -162, -160, -157, -143, -139, -132, -129, -124, -122, -113, -107, -105, -91, -89, -86, -82, -76, -71, -64, -63, -60, -51, -40, -35, -33, -21, -20, -11, -6, -2, -1, 0, 1, 3, 7, 16, 18, 19, 23, 27, 30, 33, 34, 37, 41, 44, 46, 50, 51, 52, 58, 60, 68, 74, 77, 88, 89, 93, 102, 115, 116, 119, 121, 129, 131, 134, 136, 140, 141, 149, 150, 153, 164, 166, 168, 174, 177 };
		private static TimeSpan __cPrevOpen = new TimeSpan(8, 40, 0);
		private static TimeSpan __cClosedTime1 = new TimeSpan(13, 40, 0);
		private static TimeSpan __cClosedTime2 = new TimeSpan(13, 25, 0);

		private BIAS __cBIAS = null;
		private DateTime __cPrevDate;
		//private IOrderMarket BUY, BUY_C, SELL, SELL_C;
		private IOrderMarket BUY, SELL;
		private IOrderMarket BUY_C, SELL_C;
#if HISTORY
		private IOrderPriced BUY_LC, SELL_LC;
#endif

		private int 下單口數 = 2;        //下單口數
		private int 加碼次數 = 0;        //有跳多空加碼次數就會變兩次(因為要連續下單兩天都加碼)
		private double __dOpen = 0;
		private double 停損點數 = 34;    //停損點數
		private double 跳多空點數 = 60;  //開盤漲跌點數(超過此點數就加碼)
		private double 基礎停利點 = 60;  //基礎停利點
		private double 轉折停利率 = 40;  //轉折超過20%就停利
		private double __dPrevProfit = 0;  //基礎停利點
		private bool __bTrade = false;

		private MACD __cMACD = null;

		public __BIAS_Signal(object _ctx)
			: base(_ctx) {
		}

		protected override void Create() {
			// 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
			BUY = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
			SELL = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
			BUY_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			SELL_C = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
#if HISTORY
			BUY_LC = OrderCreator.Limit(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			SELL_LC = OrderCreator.Limit(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
#endif
			__cBIAS = new BIAS(this, 2);
			__cBIAS.Length = 5;

			__cMACD = new MACD(this, 2);
			__cMACD.FastPeriod = 5;
			__cMACD.SlowPeriod = 10;
			__cMACD.MACDPeriod = 10;

			停損點數 *= 下單口數;
			基礎停利點 *= 下單口數;
			__cPrevDate = Bars.Time[0].AddDays(-1);

			log.InfoFormat("最新KBars時間={0}", Bars.Time[0].ToString("yyyy-MM-dd HH:mm:ss"));
			Output.WriteLine(string.Format("最新KBars時間={0}", Bars.Time[0].ToString("yyyy-MM-dd HH:mm:ss")));
		}

		protected override void CalcBar() {
			//Output.WriteLine(Bars.Time[0].ToString("yyyy-MM-dd HH:mm:ss"));
			if (Bars.Time[0].Date > __cPrevDate.Date) {
				__bTrade = true;
				__dPrevProfit = 0;
				__dOpen = Bars.Open[0];
				__cPrevDate = Bars.Time[0];

				//加碼數量
				double dDiff = Math.Abs(__dOpen - Bars.Close[1]);
				if (加碼次數 == 0) {
					if (dDiff >= 跳多空點數) {
						log.InfoFormat("[加碼進場機制] 昨天收盤價={0}, 今日開盤價={1} 明天開盤繼續進場加碼...", Bars.Close[1], __dOpen);
						SendOpen();
						加碼次數 = 1;
					}
				} else {
					log.InfoFormat("[加碼進場機制] 前天收盤價={0}, 今日開盤價={1} 今日為最後一次加碼...", Bars.Close[2], __dOpen);
					SendOpen();
					--加碼次數;
				}
			}

			if (__bTrade && CurrentPosition.Side == EMarketPositionSide.Flat) {
				/*
				double dRate = Math.Atan2(__cBIAS[1], __cBIAS[0]) * 180 / Math.PI;
				if (GetOrderAction(dRate) == 1) {
				        BUY.Send("BIAS_BUY", 下單口數);
				} else {
				        SELL.Send("BIAS_SELL", 下單口數);
				}
				__bTrade = false;
				//*/

				//*
				int iOSD = __cMACD.OSD[0] >= 0 ? 1 : -1;
				double dRate = Math.Atan2(__cBIAS[1], __cBIAS[0]) * 180 / Math.PI;
				int iAction = GetOrderAction(dRate);
				if (iAction == iOSD) {
					if (iAction == 1) {
						BUY.Send("BIAS_BUY", 下單口數);
					} else {
						SELL.Send("BIAS_SELL", 下單口數);
					}
				}
				__bTrade = false;
				//*/ 
			} else {
				double dStoploss = GetStoploss();
				if (dStoploss >= 停損點數) {
					SendClose("BIAS_StopLoss");
				} else if (Bars.Time[0].TimeOfDay == GetCloseTime()) {
					SendClose("BIAS_TradingClosed");
#if STOP_PROFIT
				} else if (dStoploss < 0) {
					CalcStopProfit(dStoploss * -1);
#endif
				}
			}
		}

		protected override void Destroy() {
			Save("report.csv", "TXF0.tw", this.Positions);
			base.Destroy();
		}

		protected override void OnQuoteDateTime(Zeghs.Events.QuoteDateTimeEvent e) {
			//if (__bTrade && CurrentPosition.Side == EMarketPositionSide.Flat && e.QuoteDateTime.TimeOfDay >= __cPrevOpen) {
				//SendOpen();
				//__bTrade = false;
			//}
		}

		private void CalcStopProfit(double profit) {
			if (profit > 0) {
				if (profit > 基礎停利點 && __dPrevProfit > profit) {
					double dDiff = __dPrevProfit - profit;
					double dPercent = dDiff / __dPrevProfit * 100;
					if (dPercent >= 轉折停利率) {
#if HISTORY
						double dSourceProfit = __dPrevProfit - Math.Round(__dPrevProfit * 轉折停利率 / 100);
						double dDiffProfit = dSourceProfit - profit;
						if (CurrentPosition.Side == EMarketPositionSide.Long) {
							log.InfoFormat("最高獲利={0}, 目前獲利={1}, 轉折停利率={2}, 目前轉折率={3}", __dPrevProfit, profit, 轉折停利率, dPercent);
							BUY_LC.Send("BIAS_StopProfit", Bars.Close[0] + dDiffProfit, 下單口數);
						} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
							log.InfoFormat("最高獲利={0}, 目前獲利={1}, 轉折停利率={2}, 目前轉折率={3}", __dPrevProfit, profit, 轉折停利率, dPercent);
							SELL_LC.Send("BIAS_StopProfit", Bars.Close[0] - dDiffProfit, 下單口數);
						}
#else
						if (CurrentPosition.Side == EMarketPositionSide.Long) {
							BUY_C.Send("BIAS_StopProfit", 下單口數);
						} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
							SELL_C.Send("BIAS_StopProfit", 下單口數);
						}
#endif
					}
				}
				__dPrevProfit = profit;
			}
		}

		private double GetStoploss() {
			double dStoploss = 0;
			if (CurrentPosition.Side != EMarketPositionSide.Flat) {
				bool bLong = CurrentPosition.Side == EMarketPositionSide.Long;

				List<ITrade> cOrders = CurrentPosition.OpenTrades;
				int iCount = cOrders.Count;
				for (int i = 0; i < iCount; i++) {
					ITradeOrder cOrder = cOrders[i].EntryOrder;
					dStoploss += ((bLong) ? cOrder.Price - Bars.Close[0] : Bars.Close[0] - cOrder.Price) * cOrder.Contracts;
				}
			}
			return dStoploss;
		}

		private TimeSpan GetCloseTime() {
			return (Bars.Time.Value.Date == Bars.Info.Expiration.Date) ? __cClosedTime2 : __cClosedTime1;
		}

		private int GetOrderAction(double rate) {
			int i = 0, iRet = -1, iCount = __iTable.Length;
			for (i = 0; i < iCount; i++) {
				if (__iTable[i] >= rate) {
					iRet = (--i % 2 == 0) ? 1 : -1;
					break;
				}
			}

			if (log.IsInfoEnabled) {
				i = (i == iCount) ? i - 1 : i;
				log.InfoFormat("[觸發信號] 時間={0}, 斜率={1}, 參數區間={2}, 買賣別={3}", Bars.Time[0], rate.ToString("N2"), __iTable[i], iRet);
				Output.WriteLine("[觸發信號] 時間={0}, 斜率={1}, 參數區間={2}, 買賣別={3}", Bars.Time[0], rate.ToString("N2"), __iTable[i], iRet);
			}
			return iRet;
		}

		private void SendOpen() {
			double dRate = Math.Atan2(__cBIAS[1], __cBIAS[0]) * 180 / Math.PI;
			if (GetOrderAction(dRate) == 1) {
				BUY.Send("BIAS_BUY", 下單口數);
			} else {
				SELL.Send("BIAS_SELL", 下單口數);
			}
		}

		private void SendClose(string memo) {
			if (CurrentPosition.Side == EMarketPositionSide.Long) {
				BUY_C.Send(memo, 下單口數);
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				SELL_C.Send(memo, 下單口數);
			}
		}
	}
}