using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Zeghs.Data;
using Zeghs.Rules;
using Zeghs.Utils;
using Zeghs.Scripts;
using Zeghs.Function;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace PowerLanguage.Strategy {
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "Web Electric Services", Copyright = "Copyright © 2004-2015 ZEGHS. 保留一切權利。", Comment = "Options Report")]
	public sealed class __OPTIONS_Signal : SignalObject {
		private sealed class Report {
			internal double TWIOpen = 0;
			internal double TWIClose = 0;
			internal double CPriceB = 0;
			internal double CPriceS = 0;
			internal double OPriceB = 0;
			internal double OPriceS = 0;
			internal string SymbolB = null;
			internal string SymbolS = null;
			internal DateTime TWITime = DateTime.MinValue;
		}

		private const int MAX_LOAD_OPTIONS_COUNT = 5000;

		private bool __bStart = false;
		private Instrument __cSCBars = null;
		private Instrument __cBCBars = null;
		private Instrument __cSPBars = null;
		private Instrument __cBPBars = null;
		private Report __cReport = null;
		private List<Report> __cList = null;
		private AbstractExchange __cExchange = null;
		private OpenInterest __cOpenInterest = null;

		[Input("點位差距")]
		public double GapPoint {
			get;
			set;
		}

		public __OPTIONS_Signal(object _ctx) 
			: base(_ctx) {

			this.GapPoint = 0;
			__cList = new List<Report>(4096);
			__cExchange = ProductManager.Manager.GetExchange("TWSE");
		}
		
		protected override void Create() {
			__cOpenInterest = new OpenInterest(this);
			__cOpenInterest.Initialize();
		}

		protected override void CalcBar() {
			if (!__bStart) {
				double dOpen = Bars.Open[0];
				int iBaseTarget = CalculateBaseTarget(dOpen);
				__cSCBars = GetBars("TXO0", 'C', iBaseTarget);
				__cBCBars = GetBars("TXO0", 'C', iBaseTarget + 100);
				__cSPBars = GetBars("TXO0", 'P', iBaseTarget);
				__cBPBars = GetBars("TXO0", 'P', iBaseTarget - 100);
			}

			if (__cSCBars != null && __cSPBars != null && __cBCBars != null && __cBPBars != null) {
				__bStart = true;
				DateTime cDate = Bars.Time[0].AddSeconds(900);
				__cSCBars.MoveBars(cDate);
				__cBCBars.MoveBars(cDate);
				__cSPBars.MoveBars(cDate);
				__cBPBars.MoveBars(cDate);

				System.Console.Write("回測時間... {0}", cDate.ToString("yyyy-MM-dd"));
				System.Console.CursorLeft = 0;

				DateTime cTWIDate = cDate.Date;
				if (cTWIDate == __cSCBars.Time[0].Date && cTWIDate == __cSPBars.Time[0].Date && cTWIDate == __cBCBars.Time[0].Date && cTWIDate == __cBPBars.Time[0].Date) {
					double dCDiff = __cSCBars.Open[0] - __cBCBars.Open[0];
					double dPDiff = __cSPBars.Open[0] - __cBPBars.Open[0];
					if (dCDiff < this.GapPoint && dPDiff < this.GapPoint) {
						__bStart = false;
						return;
					}

					bool bCALL = false;//dCDiff >= dPDiff;
					if (__cReport == null) {
						__cReport = new Report();
						__cReport.TWIOpen = Bars.Open[0];
						__cReport.TWIClose = Bars.Close[0];
						__cReport.OPriceB = (bCALL) ? __cBCBars.Close[0] : __cBPBars.Close[0];
						__cReport.OPriceS = (bCALL) ? __cSCBars.Close[0] : __cSPBars.Close[0];
						__cReport.SymbolB = (bCALL) ? __cBCBars.Request.Symbol : __cBPBars.Request.Symbol;
						__cReport.SymbolS = (bCALL) ? __cSCBars.Request.Symbol : __cSPBars.Request.Symbol;
						__cReport.TWITime = Bars.Time[0];
					}

					if (cDate > GetExpiration((bCALL) ? __cSCBars.Request.Symbol : __cSPBars.Request.Symbol, cDate)) {  //周選擇權到期日
						__cReport.CPriceB = (bCALL) ? __cBCBars.Close[0] : __cBPBars.Close[0];
						__cReport.CPriceS = (bCALL) ? __cSCBars.Close[0] : __cSPBars.Close[0];
						__cList.Add(__cReport);
						
						__cReport = null;
						__bStart = false;
					}
				}
			}
		}

		protected override void Destroy() {
			StringBuilder cBuilder = new StringBuilder(1048576);
			cBuilder.AppendLine("Time,TWI Open,TWI Close,SELL_SymbolId,BUY_SymbolId,SELL_Open,BUY_Open,SELL_Close,BUY_Close,SELL_Profit,BUY_Profit,Profit");

			foreach(Report cReport in __cList) {
				double dSProfit = Math.Round(cReport.OPriceS - cReport.CPriceS, 0) * 50;
				double dBProfit = Math.Round(cReport.CPriceB - cReport.OPriceB, 0) * 50;
				cBuilder.Append(cReport.TWITime.ToString("yyyy-MM-dd")).Append(',').Append(Math.Round(cReport.TWIOpen, 2)).Append(',').Append(Math.Round(cReport.TWIClose, 2)).Append(',').Append(cReport.SymbolS).Append(',').Append(cReport.SymbolB).Append(',').Append(Math.Round(cReport.OPriceS, 2)).Append(',').Append(Math.Round(cReport.OPriceB, 2)).Append(',').Append(Math.Round(cReport.CPriceS, 2)).Append(',').Append(Math.Round(cReport.CPriceB, 2)).Append(',').Append(dSProfit).Append(',').Append(dBProfit).Append(',').Append(dSProfit + dBProfit).AppendLine();
			}

			File.WriteAllText("report.csv", cBuilder.ToString(), Encoding.UTF8);
		}

		private int CalculateBaseTarget(double price) {
			int iPrice = (int) Math.Round(price, 0);
			int iDiff = iPrice % 100;
			int iBasePrice = iPrice - iDiff;
			return iBasePrice + ((iDiff < 50) ? 0 : 100);
		}

		private Instrument GetBars(string commodityId, char callOrPut, int targetPrice) {
			InstrumentDataRequest cRequest = new InstrumentDataRequest() {
				Exchange = "TWSE",
				DataFeed = "Mitake",
				Range = DataRequest.CreateBarsBack(DateTime.Now, MAX_LOAD_OPTIONS_COUNT),
				Resolution = new Resolution(EResolution.Day, 1),
				Symbol = string.Format("{0}{1}{2}.tw", commodityId, callOrPut, targetPrice)
			};

			__cExchange.AddProduct(new Product() {
				 Category = ESymbolCategory.IndexOption,
				 CommodityId = "TXO",
				 SymbolId = cRequest.Symbol,
				 SymbolName = cRequest.Symbol
			});
			return new Instrument(SeriesManager.Manager.GetSeries(cRequest), 0);
		}

		private DateTime GetExpiration(string symbolId, DateTime date) {
			AbstractProductProperty cProperty = __cExchange.GetProperty(symbolId, "Mitake");
			IContractTime cContractTime = cProperty.ContractRule as IContractTime;
			return cContractTime.GetContractTime(date).MaturityDate;
		}
	}
}
