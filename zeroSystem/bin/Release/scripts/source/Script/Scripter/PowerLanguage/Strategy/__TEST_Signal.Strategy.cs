using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Globalization;
using System.Collections.Generic;
using PowerLanguage.Function;
using Zeghs.Scripts;
using Zeghs.Services;
using Zeghs.Orders;

namespace PowerLanguage.Strategy {
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "測試腳本", Copyright = "Copyright © 2014 Tester. 保留一切權利。", Comment = "Test Script")]
	public sealed class __TEST_Signal : SignalObject {
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

		private IOrderMarket 多單, 多沖, 空單, 空沖;
		private Timer __cTimer = null;
		private AbstractOrderService __cService = null;

		public __TEST_Signal(object _ctx)
			: base(_ctx) {
		}

		KD __cKD = null;
		MACD __cMACD = null;
		IInstrument bbb = null;
		VariableSeries<double> __cKDSum = null;
		protected override void Create() {
			// 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
			多單 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
			空單 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
			多沖 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
			空沖 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));

			__cService = OrderCreator as AbstractOrderService;

			__cTimer = new Timer(1);
			__cTimer.AutoReset = false;
			__cTimer.Elapsed += Timer_onElapsed;
			//__cTimer.Start();

			//bbb = BarsOfData(2);
			cBuilder.AppendLine("[F000]");

			__cKDSum = new VariableSeries<double>(this);

			__cMACD = new MACD(this);
			__cMACD.FastPeriod = 5;
			__cMACD.SlowPeriod = 10;
			__cMACD.MACDPeriod = 10;

			__cKD = new KD(this);
			__cKD.Length = 5;
		}

		protected override void Destroy() {
			//Save("report.csv", "TXF0.tw", this.Positions);
			
			base.Destroy();

			
			File.WriteAllText("abc.txt", cBuilder.ToString(), Encoding.UTF8);
		}

		private static TimeSpan __cClosedTime1 = new TimeSpan(13, 40, 0);
		private static TimeSpan __cClosedTime2 = new TimeSpan(13, 25, 0);
		private TimeSpan GetCloseTime() {
			return (Bars.Time.Value.Date == Bars.Info.Expiration.Date) ? __cClosedTime2 : __cClosedTime1;
		}
		
		private void SendClose(string memo) {
			if (CurrentPosition.Side == EMarketPositionSide.Long) {
				多沖.Send(memo, 1);
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				空沖.Send(memo, 1);
			}
		}
		
		StringBuilder cBuilder = new StringBuilder(8192);
		protected override void CalcBar() {
			/*
			if (Bars.Time[0].TimeOfDay >= GetCloseTime()) {
				SendClose("TradingClosed");
			} else {
				__cKDSum.Value = __cKD.K[0] + __cKD.D[0];
				int iKD = (__cKDSum[0] >= __cKDSum[1]) ? 1 : -1;
				if (__cMACD.OSD[0] * __cMACD.OSD[1] < 0) {
					int iOSD = __cMACD.OSD[0] >= 0 ? 1 : -1;
					int iFlag = iKD + iOSD;
					switch (iFlag) {
						case 2:
							多單.Send("多單", 1);
							break;
						case -2:
							空單.Send("空單", 1);
							break;
					}
				}
			}*/

			//if (this.CurrentPosition.Side == EMarketPositionSide.Flat) {
			//        多單.Send("作多", 100);
			//} else if (this.CurrentPosition.OpenLots == 100) {
			//        多沖.Send("沖銷", 100);
			//}

			///*
			if (Bars.Time[0].Date == new DateTime(2016, 1, 8)) {
				int iMinute = Bars.Time[0].Hour * 60 + Bars.Time[0].Minute;
				cBuilder.Append(iMinute).Append(',').Append(Bars.Open[0]).Append(',').Append(Bars.High[0]).Append(',').Append(Bars.Low[0]).Append(',').Append(Bars.Close[0]).Append(',').Append(Bars.Volume[0]).AppendLine();
			}
			System.Console.WriteLine(Bars.Time[0].ToString("yyyy-MM-dd HH:mm:ss") + " " + string.Format("{0,8:0.00}", Math.Round(Bars.Open[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(Bars.High[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(Bars.Low[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(Bars.Close[0], 2)) + " " + string.Format("{0,7}", Math.Round(Bars.Volume[0], 2)));
			// */
 

			//System.Console.WriteLine(bbb.Time[0].ToString("yyyy-MM-dd HH:mm:ss") + " " + string.Format("{0,8:0.00}", Math.Round(bbb.Open[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(bbb.High[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(bbb.Low[0], 2)) + " " + string.Format("{0,8:0.00}", Math.Round(bbb.Close[0], 2)) + " " + string.Format("{0,7}", Math.Round(bbb.Volume[0], 2)));

			/*
			if (CurrentPosition.Side == EMarketPositionSide.Flat) {
				多單.Send("買進", 100);
			} else if (CurrentPosition.Side == EMarketPositionSide.Long) {
				//空單.Send("賣出", 100);
				多沖.Send("多沖");
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				多單.Send("買進", 100);
			}
			//*/
		}

		private int xxx = 0, yyy = 0;
		private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
			if (CurrentPosition.Side == EMarketPositionSide.Flat) {
				多單.Send("買進", 100);
			} else if (CurrentPosition.Side == EMarketPositionSide.Long) {
				空單.Send("賣出", 100);
				//多沖.Send("多沖");
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				多單.Send("買進", 100);
			}

			/*
			//if (this.CurrentPosition.Side == EMarketPositionSide.Flat) {
			//	多單.Send("買進", 100);
			//} else {
			//	多沖.Send("沖銷");
			//}

			if (CurrentPosition.Side == EMarketPositionSide.Flat) {
				//if (yyy == 0) {
				//	xxx = 0;

				//	yyy = new Random().Next(100);
				//	if (yyy < 50) {
				多單.Send("買進", 1);
				//	} else {
				//		空單.Send("賣出", 100);
				//	}
				//}
			} else if (CurrentPosition.Side == EMarketPositionSide.Long) {
			//} else if (xxx == 40) {
				//if (yyy < 50) {
				多沖.Send("多沖", 1);
					//多沖.Send("多沖");
				//} else {
				//	空沖.Send("空沖");
				//}
				//yyy = 0;
			} else if (CurrentPosition.Side == EMarketPositionSide.Short) {
				//多單.Send("買進", 10);
			}*/

			__cService.OnWork();
			__cTimer.Start();
		}
	}
}