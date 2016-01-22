// -- 編譯所需的其他依賴程式碼(請勿移除) -------------------------------------
//css_import ..\..\Zeghs\Data\ForeignInvestmentData.cs;
//css_import ..\..\Zeghs\Data\ForeignInvestmentGroup.cs;
//css_import ..\..\Zeghs\Data\OpenInterestData.cs;
//css_import ..\..\Zeghs\Function\HistoryVolatility.cs;
//css_import ..\..\Zeghs\Function\ForeignInvestment.cs;
//css_import ..\..\Zeghs\Function\OpenInterest.cs;
//css_import ..\..\Zeghs\Utils\ForeignInvestmentUtil.cs;
//css_import ..\..\Zeghs\Utils\OpenInterestUtil.cs;
//css_import ..\..\Zeghs\Utils\OptionsUtil.cs;
//css_import ..\..\Zeghs\Utils\Parameter.cs;
//css_import ..\..\Zeghs\Utils\ZBuffer.cs;
//css_import ..\..\Zeghs\Web\ZReader.cs;
//css_import ..\..\Zeghs\Web\ZRequest.cs;
// --------------------------------------------------------------------------

//#define __BACKTEST
//#define __SIMULATE

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Rules;
using Zeghs.Utils;
using Zeghs.Events;
using Zeghs.Orders;
using Zeghs.Scripts;
using Zeghs.Products;
using Zeghs.Function;
using Zeghs.Services;
using Zeghs.Managers;

namespace PowerLanguage.Strategy {
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "Web Electric Services", Copyright = "Copyright © 2004-2015 ZEGHS. 保留一切權利。", Comment = "週選擇權模型")]
	public sealed class __WeekOption_Signal : SignalObject {
		private const int TARGET_SCALE_RANGE = 50;         //目標價格跳動(縮放)間距
#if __BACKTEST
		private const int MAX_LOAD_OPTIONS_COUNT = 200000; //最大讀取選擇權歷史資料個數
#else
		private const int MAX_LOAD_OPTIONS_COUNT = 20;     //最大讀取選擇權歷史資料個數
#endif

#if __BACKTEST
		private static void DumpReport(AbstractOrderService orderService1, AbstractOrderService orderService2, List<double[]> twiCloses) {
			StringBuilder cBuilder = new StringBuilder(1024 * 1024);

			int iCount = orderService1.Positions.Count;
			if (iCount > 0) {
				PositionSeries cPositions1 = orderService1.Positions;
				PositionSeries cPositions2 = orderService2.Positions;
				string sSymbolId1 = orderService1.Bars.Request.Symbol;
				string sSymbolId2 = orderService2.Bars.Request.Symbol;

				for (int i = 0; i < iCount; i++) {
					IMarketPosition cPosition1 = cPositions1[i];
					IMarketPosition cPosition2 = cPositions2[i];
					List<ITrade> cTrades1 = cPosition1.ClosedTrades;
					foreach (ITrade cTrade in cTrades1) {
						ITradeOrder cOpenO = cTrade.EntryOrder;
						ITradeOrder cCloseO = cTrade.ExitOrder;

						cBuilder.Append(cOpenO.Ticket).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][0]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][1]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][2]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][3]).Append(',').Append(sSymbolId1).Append(',').Append(cOpenO.Category).Append(',').Append(cOpenO.Action).Append(',').Append(cOpenO.Contracts).Append(',').Append(cOpenO.Price).Append(',').Append(string.Empty).Append(',').Append(cOpenO.Fee).Append(',').Append(cOpenO.Tax).Append(',').Append(cOpenO.Time.ToString("yyyy/MM/dd HH:mm(ddd)")).Append(',').AppendLine(cOpenO.Name);
						cBuilder.Append(cCloseO.Ticket).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][0]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][1]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][2]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][3]).Append(',').Append(sSymbolId1).Append(',').Append(cCloseO.Category).Append(',').Append(cCloseO.Action).Append(',').Append(cCloseO.Contracts).Append(',').Append(cCloseO.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cCloseO.Fee).Append(',').Append(cCloseO.Tax).Append(',').Append(cCloseO.Time.ToString("yyyy/MM/dd HH:mm(ddd)")).Append(',').AppendLine(cCloseO.Name);
					}

					List<ITrade> cTrades2 = cPosition2.ClosedTrades;
					foreach (ITrade cTrade in cTrades2) {
						ITradeOrder cOpenO = cTrade.EntryOrder;
						ITradeOrder cCloseO = cTrade.ExitOrder;

						cBuilder.Append(cOpenO.Ticket).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][0]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][1]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][2]).Append(',').Append(twiCloses[int.Parse(cOpenO.Ticket)][3]).Append(',').Append(sSymbolId2).Append(',').Append(cOpenO.Category).Append(',').Append(cOpenO.Action).Append(',').Append(cOpenO.Contracts).Append(',').Append(cOpenO.Price).Append(',').Append(string.Empty).Append(',').Append(cOpenO.Fee).Append(',').Append(cOpenO.Tax).Append(',').Append(cOpenO.Time.ToString("yyyy/MM/dd HH:mm(ddd)")).Append(',').AppendLine(cOpenO.Name);
						cBuilder.Append(cCloseO.Ticket).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][0]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][1]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][2]).Append(',').Append(twiCloses[int.Parse(cCloseO.Ticket)][3]).Append(',').Append(sSymbolId2).Append(',').Append(cCloseO.Category).Append(',').Append(cCloseO.Action).Append(',').Append(cCloseO.Contracts).Append(',').Append(cCloseO.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cCloseO.Fee).Append(',').Append(cCloseO.Tax).Append(',').Append(cCloseO.Time.ToString("yyyy/MM/dd HH:mm(ddd)")).Append(',').AppendLine(cCloseO.Name);
					}
				}

				File.AppendAllText("report.csv", cBuilder.ToString(), Encoding.UTF8);
			}
		}
#endif

#if !__SIMULATE
		private static void SetPipeNumber(List<InputAttribute> args, int apiNumber, int localNumber) {
			args[0].SetValue(apiNumber.ToString());
			args[1].SetValue(localNumber.ToString());
		}
#endif

		private int __i下單口數 = 1;                //下單數量(可以修改這裡放大口數)
		private double __d停損倍率 = 2;             //停損率(初始價差 * 停損率 >= 目前價差則停損)
		private double __dInitDiff = 0;
		private int __iSetupFlag = 0;
		private int __iOptionsStreams = 3;          //選擇權資料由第4個區間開始存放(1=TXF0.tw(1分), 2=TXF0.tw(日), 3=TWI.tw(1分))
		private string __sSymbolId = string.Empty;  //保存目前洲選擇權的商品代號(用來抓取到期日使用)
		private int __iCALL_S = 0, __iCALL_B = 0, __iPUT_S = 0, __iPUT_B = 0;
		private DateTime __cDownloanTime = DateTime.MinValue;     //下載未平倉口數時間
		private TimeSpan __cCloseTime = new TimeSpan(13, 30, 0);  //平倉時間

		private Timer __cTimer = null;
		private Instrument __cTWIBars = null;
#if __BACKTEST
		private List<double[]> __cCloses = null;
#endif
		private OpenInterest __cOpenInterest = null;
		private AbstractExchange __cExchange = null;
		private HistoryVolatility __cHistoryV = null;
		private Dictionary<string, int> __cSymbolIds = null;
		private ForeignInvestment __cForeignInvestment = null;

		private IOrderMarket __cBOrderO = null, __cSOrderO = null;
		private IOrderMarket __cBOrderC = null, __cSOrderC = null;
		private AbstractOrderService __cOrderService1 = null, __cOrderService2 = null;

		private object __oLock = new object();

		[Input("與現價價格差距空間")]
		public int Range {
			get;
			set;
		}

		[Input("點位差距")]
		public double GapPoint {
			get;
			set;
		}

		public __WeekOption_Signal(object _ctx) 
			: base(_ctx) {
			
			this.Range = 100;
			this.GapPoint = 8;

			__cExchange = ProductManager.Manager.GetExchange("TWSE");
			__cDownloanTime = DateTime.Today.AddSeconds(55800);  //調整為今日15:30
		}
		protected override void Create() {
#if __BACKTEST
			__cCloses = new List<double[]>(1024);
			__cCloses.Add(new double[] {0, 0, 0, 0});
#endif
			__cTWIBars = BarsOfData(3) as Instrument;
			__cSymbolIds = new Dictionary<string, int>(32);

			//讀取未平倉量
			__cOpenInterest = new OpenInterest(this, 2);
			__cOpenInterest.Initialize();

			//讀取三大法人多空與未平倉量
			__cForeignInvestment = new ForeignInvestment(this, 2);
			__cForeignInvestment.Initialize();

			//建立歷史波動率指標
			__cHistoryV = new HistoryVolatility(this, 2);

			//設定計時器(定期抓未平倉量)
			__cTimer = new Timer(1000);
			__cTimer.AutoReset = false;
			__cTimer.Elapsed += Timer_onElapsed;
			__cTimer.Start();
		}

		protected override void CalcBar() {
			DateTime cToday = Bars.Time[0].Date;
			if (__iSetupFlag == 0) {
				int iWeek = (int) cToday.DayOfWeek;
				if (iWeek == 3) {
					return;  //星期三都不做(週選擇權星期三換週)
				}

				double dTWIPrice = __cTWIBars.Close[0];  //取得加權指數目前價格				
				if (dTWIPrice == 0) {
					return;
				} else {
					__iCALL_S = CalculateTarget(dTWIPrice, false);  //計算 CALL 目標價格差距 this.Range 的目標價(作賣方)
					__iCALL_B = __iCALL_S + TARGET_SCALE_RANGE;  //往上移動一個 CALL 目標價格當作保護價格(作買方)
					__iPUT_S = CalculateTarget(dTWIPrice, true);  //計算 PUT 目標價格差距 this.Range 的目標價(作賣方)
					__iPUT_B = __iPUT_S - TARGET_SCALE_RANGE;  //往下移動一個 PUT 目標價格當作保護價格(作買方)
				}
			}

			if (__iCALL_B == 0 && __iCALL_S == 0 && __iPUT_B == 0 && __iPUT_S == 0) {
				return;
			}

			//讀取四組週選擇權報價資訊
			Instrument cCBBars = GetBars("TXW0", 'C', __iCALL_B);
			Instrument cCSBars = GetBars("TXW0", 'C', __iCALL_S);
			Instrument cPBBars = GetBars("TXW0", 'P', __iPUT_B);
			Instrument cPSBars = GetBars("TXW0", 'P', __iPUT_S);

			//計算 CALL 權買賣組合的價差
			double dCDiff = 0;
			if (cCBBars != null && cCSBars != null) {
#if __BACKTEST
				if (cToday == __cTWIBars.Time[0].Date && cToday == cCSBars.Time[0].Date && cToday == cCBBars.Time[0].Date) {
					dCDiff = cCSBars.Close[0] - cCBBars.Close[0];
				}
#else
				dCDiff = cCSBars.Close[0] - cCBBars.Close[0];
#endif
			}

			//計算 PUT 權買賣組合的價差
			double dPDiff = 0;
			if (cPBBars != null && cPSBars != null) {
#if __BACKTEST
				if (cToday == __cTWIBars.Time[0].Date && cToday == cPSBars.Time[0].Date && cToday == cPBBars.Time[0].Date) {
					dPDiff = cPSBars.Close[0] - cPBBars.Close[0];
				}
#else
				dPDiff = cPSBars.Close[0] - cPBBars.Close[0];
#endif
			}

			if (__iSetupFlag == 0) {  //如果旗標為 0 (表示還沒有做買賣交易)
				if (dCDiff >= dPDiff) {  //如果 CALL 權買賣組合的價差比較大
					if (dCDiff >= this.GapPoint) {  //是否有超過使用者設定的價差空間
						__iSetupFlag = 1;  //旗標=1 (CALL 權買賣組合)
						__dInitDiff = dCDiff;
						__sSymbolId = cCSBars.Request.Symbol;
#if __BACKTEST
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
#endif
						CreateOrderService(cCBBars, cCSBars);

						__cBOrderO.Send("CALL_BUY_OPEN", __i下單口數);
						__cSOrderO.Send("CALL_SELL_OPEN", __i下單口數);
					}
				} else {  //如果 PUT 權買賣組合的價差比較大
					if (dPDiff >= this.GapPoint) {  //是否有超過使用者設定的價差空間
						__iSetupFlag = 2;  //旗標=2 (PUT 權買賣組合)
						__dInitDiff = dPDiff;
						__sSymbolId = cPSBars.Request.Symbol;
#if __BACKTEST
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
#endif
						CreateOrderService(cPBBars, cPSBars);

						__cBOrderO.Send("PUT_BUY_OPEN", __i下單口數);
						__cSOrderO.Send("PUT_SELL_OPEN", __i下單口數);
					}
				}
			} else {
				DateTime cDate = Bars.Time[0];
				DateTime cExpir = cDate.Date;
				TimeSpan cCloseTime = cDate.TimeOfDay;
				if (cExpir == GetExpiration(__sSymbolId, cExpir).Date && cCloseTime >= __cCloseTime) {  //結算日當天平倉
					SendClose("CLOSE");
#if __BACKTEST
					__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
					__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });

					this.AfterBars();  //使用目前規格模式無法下出組合單, 所以必須自行建立下單模組
					DumpReport(__cOrderService1, __cOrderService2, __cCloses);
#endif
				} else {
					double dDiff = (__iSetupFlag == 1) ? cCSBars.Close[0] - cCBBars.Close[0] : (__iSetupFlag == 2) ? cPSBars.Close[0] - cPBBars.Close[0] : 0;
					if (dDiff >= __dInitDiff * __d停損倍率) {
						SendClose("STOPLOSS");
#if __BACKTEST
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });
						__cCloses.Add(new double[] { __cTWIBars.Open[0], __cTWIBars.High[0], __cTWIBars.Low[0], __cTWIBars.Close[0] });

						this.AfterBars();  //使用目前規格模式無法下出組合單, 所以必須自行建立下單模組
						DumpReport(__cOrderService1, __cOrderService2, __cCloses);
#endif
					}
				}
			}

			//After bars(因為是自己建立的下單模組, 需要自己執行 OnWork 方法)
			this.AfterBars();  //使用目前規格模式無法下出組合單, 所以必須自行建立下單模組
		}

		protected override void Destroy() {
#if __BACKTEST
			__cCloses.Clear();
#endif
			lock (__oLock) {
				__cTimer.Dispose();
				__cTimer = null;
			}
		}

		private void AfterBars() {
			if (__cOrderService1 != null) {
				__cOrderService1.OnWork();
			}

			if (__cOrderService2 != null) {
				__cOrderService2.OnWork();
			}
		}

		private int CalculateTarget(double price, bool isPUT) {
			int iPrice = (int) Math.Round(price, 0);
			int iDiff = iPrice % 100;
			int iBasePrice = iPrice - iDiff;
			iPrice = iBasePrice + ((iDiff < TARGET_SCALE_RANGE) ? 0 : TARGET_SCALE_RANGE);
			return iPrice + ((isPUT) ? -this.Range : this.Range);
		}

		private void CreateOrderService(Instrument buyBars, Instrument sellBars) {
			if(__cOrderService1 != null) {
				__cOrderService1.Dispose();
			}

#if __SIMULATE
			__cOrderService1 = OrderManager.Manager.CreateOrderService("Netwings.OrderService;Netwings.SimulateOrderService");
#else
			__cOrderService1 = OrderManager.Manager.CreateOrderService("Netwings.OrderService;Netwings.RealOrderService");
			SetPipeNumber(InputAttribute.GetParameters(__cOrderService1), 1, 1);

#endif
			__cOrderService1.onResponse += OrderService_onResponse;
			__cOrderService1.SetInstrument(buyBars);
			__cOrderService1.SetDefaultContracts(1);
			__cOrderService1.Initialize();

			IOrderCreator cOrderCreator1 = __cOrderService1 as IOrderCreator;
			__cBOrderO = cOrderCreator1.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, "CALL_BUY", EOrderAction.Buy, OrderExit.FromAll));
			__cBOrderC = cOrderCreator1.MarketThisBar(new SOrderParameters(Contracts.Default, "CALL_CLOSE", EOrderAction.Sell, OrderExit.FromAll));

			if (log.IsInfoEnabled) log.Info("[CreateOrderService] Set BUY \"Netwings.OrderService;Netwings.SimulateOrderService\" and create...");

			if(__cOrderService2 != null) {
				__cOrderService2.Dispose();
			}

#if __SIMULATE
			__cOrderService2 = OrderManager.Manager.CreateOrderService("Netwings.OrderService;Netwings.SimulateOrderService");
#else
			__cOrderService2 = OrderManager.Manager.CreateOrderService("Netwings.OrderService;Netwings.RealOrderService");
			SetPipeNumber(InputAttribute.GetParameters(__cOrderService2), 1, 2);
#endif
			__cOrderService2.onResponse += OrderService_onResponse;
			__cOrderService2.SetInstrument(sellBars);
			__cOrderService2.SetDefaultContracts(1);
			__cOrderService2.Initialize();

			IOrderCreator cOrderCreator2 = __cOrderService2 as IOrderCreator;
			__cSOrderO = cOrderCreator2.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, "PUT_SELL", EOrderAction.SellShort, OrderExit.FromAll));
			__cSOrderC = cOrderCreator2.MarketThisBar(new SOrderParameters(Contracts.Default, "PUT_CLOSE", EOrderAction.BuyToCover, OrderExit.FromAll));

			if (log.IsInfoEnabled) log.Info("[CreateOrderService] Set SELL \"Netwings.OrderService;Netwings.SimulateOrderService\" and create...");
		}

		private Instrument GetBars(string commodityId, char callOrPut, int targetPrice) {
			int iStream = 0;
			string sSymbolId = string.Format("{0}{1}{2}.tw", commodityId, callOrPut, targetPrice);
			if (__cSymbolIds.TryGetValue(sSymbolId, out iStream)) {
				return BarsOfData(iStream) as Instrument;
			} else {
				__cSymbolIds.Add(sSymbolId, ++__iOptionsStreams);

#if __BACKTEST
				__cExchange.AddProduct(new Product() {
					Category = ESymbolCategory.IndexOption,
					CommodityId = "TXW",
					SymbolId = sSymbolId,
					SymbolName = sSymbolId
				});
#endif

				this.AddDataStreams(new List<InstrumentDataRequest>() {
					new InstrumentDataRequest() {
						Exchange = "TWSE",
						DataFeed = "Mitake",
						Range = DataRequest.CreateBarsBack(DateTime.Now, MAX_LOAD_OPTIONS_COUNT),
						Resolution = new Resolution(EResolution.Minute, 1),
						Symbol = sSymbolId
					}
				});
				return null;
			}
		}

		private DateTime GetExpiration(string symbolId, DateTime date) {
			AbstractProductProperty cProperty = __cExchange.GetProperty(symbolId, "Mitake");
			IContractTime cContractTime = cProperty.ContractRule as IContractTime;
			return cContractTime.GetContractTime(date).MaturityDate;
		}

		private void SendClose(string memo) {
			if (__iSetupFlag == 1) {
				__cBOrderC.Send(string.Format("CALL_BUY_{0}", memo));
				__cSOrderC.Send(string.Format("CALL_SELL_{0}", memo));
			} else {
				__cBOrderC.Send(string.Format("PUT_BUY_{0}", memo));
				__cSOrderC.Send(string.Format("PUT_SELL_{0}", memo));
			}
			__iSetupFlag = 0;
		}

		private void OrderService_onResponse(object sender, ResponseEvent e) {
			this.OnTradeResponse(e);
		}

		private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
#if !__SIMULATE
			if (__cOrderService1 != null) {
				__cOrderService1.OnWork();
			}

			if (__cOrderService2 != null) {
				__cOrderService2.OnWork();
			}
#endif

			if (DateTime.Now >= __cDownloanTime) {
				//下載今日未平倉量
				if(log.IsInfoEnabled) log.Info("開始下載今日未平倉量資訊...");
				OpenInterestUtil.Download(__cDownloanTime);
				OpenInterestData cData = OpenInterestUtil.Load(__cDownloanTime);
				__cOpenInterest.SetValue(cData);

				//下載今日三大法人多空口數與未平倉口數
				if (log.IsInfoEnabled) log.Info("開始下載今日三大法人多空口數與未平倉量資訊...");
				ForeignInvestmentUtil.Download(__cDownloanTime);
				ForeignInvestmentGroup cGroup = ForeignInvestmentUtil.Load(__cDownloanTime);
				__cForeignInvestment.SetValue(cGroup);

				__cDownloanTime = __cDownloanTime.AddSeconds(86400);
			}

			lock (__oLock) {
				if (__cTimer != null) {
					__cTimer.Start();
				}
			}
		}
	}
}