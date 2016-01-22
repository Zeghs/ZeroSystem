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
	public sealed class __WEEK_OPTIONS_Signal : SignalObject {
		private sealed class Report {
			internal double TXFOpen = 0;
			internal double TXFClose = 0;
			internal double CPriceC = 0;
			internal double OPriceC = 0;
			internal double MPriceC = 0;
			internal double WPriceC = 0;
			internal double CPriceP = 0;
			internal double OPriceP = 0;
			internal double MPriceP = 0;
			internal double WPriceP = 0;
			internal string SymbolC = null;
			internal string SymbolP = null;
			internal DateTime TXFTime = DateTime.MinValue;
		}

		private sealed class TotalReport {
			internal double TotalsWinsC = 0;
			internal double TotalsWinsP = 0;
			internal double TotalsLoseC = 0;
			internal double TotalsLoseP = 0;
			internal double TotalCountC = 0;
			internal double TotalCountP = 0;
			internal double WinnerCountC = 0;
			internal double WinnerCountP = 0;
		}

		private const int TARGET_SCALE_RANGE = 50;
		private const int MAX_LOAD_OPTIONS_COUNT = 2000;

		private Report[] __cReports = null;
		private List<Report[]> __cList = null;
		private TotalReport[] __cTReports = null;
		private AbstractExchange __cExchange = null;
		private OpenInterest __cOpenInterest = null;


		[Input("與現價價格差距空間")]
		public int Range {
			get;
			set;
		}

		public __WEEK_OPTIONS_Signal(object _ctx) 
			: base(_ctx) {
			
			this.Range = 150;
			
			__cList = new List<Report[]>(4096);
			__cExchange = ProductManager.Manager.GetExchange("TWSE");

			__cTReports = new TotalReport[7];
			for (int i = 0; i < 7; i++) {
				__cTReports[i] = new TotalReport();
			}
		}
		
		protected override void Create() {
			__cOpenInterest = new OpenInterest(this);
			__cOpenInterest.Initialize();
		}

		protected override void CalcBar() {
			double dOpen = Bars.Open[0];
			int iCTarget = CalculateTarget(dOpen, false);
			int iPTarget = CalculateTarget(dOpen, true);

			Instrument cCBars = GetBars("TXW0", 'C', iCTarget);
			Instrument cPBars = GetBars("TXW0", 'P', iPTarget);

			if (cCBars != null && cPBars != null) {
				DateTime cDate = Bars.Time[0];
				cCBars.MoveBars(cDate);
				cPBars.MoveBars(cDate);

				System.Console.Write("回測時間... {0}", cDate.ToString("yyyy-MM-dd"));
				System.Console.CursorLeft = 0;

				DateTime cTXFDate = cDate.Date;
				if (cTXFDate == cCBars.Time[0].Date && cTXFDate == cPBars.Time[0].Date) {
					DateTime eee = GetExpiration(cCBars.Request.Symbol, cDate);
					double dT = Math.Round((eee - cDate).TotalDays, 0) / 250d;

					if (dT > 0) {
						double dIV = OptionsUtil.CALL_IV(Bars.Close[0], iCTarget, 0.02d, dT, cCBars.Open[0]);
						double[] dValues = OptionsUtil.CALLGreeks(Bars.Close[0], iCTarget, 0.02d, dT, dIV);
						double dValues1 = OptionsUtil.CALLPrice(Bars.Close[0], iCTarget, 0.02d, dT, dIV);
						//System.Console.WriteLine("理論={0}, Price={1}, Delta={2}, Gamma={3}, Theta={4}, Vega={5}", dValues1, cCBars.Open[0], dValues[0], dValues[1], dValues[2], dValues[3]);
					}

					if (__cReports != null) {
						int iWeek = (int) cDate.DayOfWeek;
						if (iWeek == 4 && __cReports[3] == null) {
							__cReports = new Report[7];
						}

						Report cReport = new Report();
						cReport.TXFOpen = dOpen;
						cReport.TXFClose = Bars.Close[0];
						cReport.OPriceC = cCBars.Open[0];
						cReport.OPriceP = cPBars.Open[0];
						cReport.WPriceC = cCBars.Close[0];
						cReport.WPriceP = cPBars.Close[0];
						cReport.SymbolC = cCBars.Request.Symbol;
						cReport.SymbolP = cPBars.Request.Symbol;
						cReport.TXFTime = Bars.Time[0];

						__cReports[iWeek] = cReport;
					}

					if (cDate > GetExpiration(cCBars.Request.Symbol, cDate)) {  //周選擇權到期日
						if (__cReports != null) {
							int iNULLCount = 0;
							foreach (Report cReport in __cReports) {
								if (cReport == null) {
									++iNULLCount;
								} else {
									cReport.CPriceC = cCBars.Close[0];
									cReport.CPriceP = cPBars.Close[0];
									cReport.MPriceC = cCBars.High[0];
									cReport.MPriceP = cPBars.High[0];

									double dCProfit = (cReport.OPriceC - cReport.CPriceC) * 50;
									double dPProfit = (cReport.OPriceP - cReport.CPriceP) * 50;

									int iWeek = (int) cReport.TXFTime.DayOfWeek;
									TotalReport cTReport = __cTReports[iWeek];
									++cTReport.TotalCountC;
									++cTReport.TotalCountP;

									if (dCProfit < 0) {
										cTReport.TotalsLoseC += dCProfit;
									} else {
										cTReport.TotalsWinsC += dCProfit;
										++cTReport.WinnerCountC;
									}

									if (dPProfit < 0) {
										cTReport.TotalsLoseP += dPProfit;
									} else {
										cTReport.TotalsWinsP += dPProfit;
										++cTReport.WinnerCountP;
									}
								}
							}

							if (iNULLCount < 7) {
								__cList.Add(__cReports);
							}
						}

						__cReports = new Report[7];
					}
				}
			}
		}

		protected override void Destroy() {
			int[] iWeeks = new int[] { 4, 5, 1, 2, 3 };
			StringBuilder cBuilder = new StringBuilder(1048576);
			cBuilder.AppendLine("Time,TXF Open,TXF Close,CALL SymbolId,Open,Close,Settle,Today Profit,Max lose,Settle Profit,PUT SymbolId,Open,Close,Settle,Today Profit,Max lose,Settle Profit");

			foreach(Report[] cReports in __cList) {
				foreach (int iWeek in iWeeks) {
					Report cReport = cReports[iWeek];
					if (cReport != null) {
						cBuilder.Append(cReport.TXFTime.ToString("yyyy-MM-dd(ddd)")).Append(',').Append(Math.Round(cReport.TXFOpen, 2)).Append(',').Append(Math.Round(cReport.TXFClose, 2)).Append(',').Append(cReport.SymbolC).Append(',').Append(Math.Round(cReport.OPriceC, 2)).Append(',').Append(Math.Round(cReport.WPriceC, 2)).Append(',').Append(Math.Round(cReport.CPriceC, 2)).Append(',').Append(Math.Round(cReport.OPriceC - cReport.WPriceC, 0) * 50).Append(',').Append(Math.Round(cReport.OPriceC - cReport.MPriceC, 0) * 50).Append(',').Append(Math.Round(cReport.OPriceC - cReport.CPriceC, 0) * 50).Append(',').Append(cReport.SymbolP).Append(',').Append(Math.Round(cReport.OPriceP, 2)).Append(',').Append(Math.Round(cReport.WPriceP, 2)).Append(',').Append(Math.Round(cReport.CPriceP, 2)).Append(',').Append(Math.Round(cReport.OPriceP - cReport.WPriceP, 0) * 50).Append(',').Append(Math.Round(cReport.OPriceP - cReport.MPriceP, 0) * 50).Append(',').Append(Math.Round(cReport.OPriceP - cReport.CPriceP, 0) * 50).AppendLine();
					}
				}
			}

			cBuilder.AppendLine();
			cBuilder.AppendLine();

			cBuilder.AppendLine("Week,Profit,Lose,Totals,Winner,Count,Winner(%),Profit,Lose,Totals,Winner,Count,Winner(%)");
			foreach (int iWeek in iWeeks) {
				TotalReport cReport = __cTReports[iWeek];
				if (cReport.TotalCountC > 0 && cReport.TotalCountP > 0) {
					cBuilder.Append(string.Format("週{0}", iWeek)).Append(',').Append(cReport.TotalsWinsC).Append(',').Append(cReport.TotalsLoseC).Append(',').Append(Math.Round(cReport.TotalsWinsC + cReport.TotalsLoseC, 0)).Append(',').Append(cReport.WinnerCountC).Append(',').Append(cReport.TotalCountC).Append(',').Append(Math.Round((cReport.WinnerCountC / cReport.TotalCountC) * 100, 2)).Append(',').Append(cReport.TotalsWinsP).Append(',').Append(cReport.TotalsLoseP).Append(',').Append(Math.Round(cReport.TotalsWinsP + cReport.TotalsLoseP, 0)).Append(',').Append(cReport.WinnerCountP).Append(',').Append(cReport.TotalCountP).Append(',').Append(Math.Round((cReport.WinnerCountP / cReport.TotalCountP) * 100, 2)).AppendLine();
				}
			}

			File.WriteAllText("report.csv", cBuilder.ToString(), Encoding.UTF8);
		}

		private int CalculateTarget(double price, bool isDown) {
			int iPrice = (int) Math.Round(price, 0);
			int iDiff = iPrice % 100;
			int iBasePrice = iPrice - iDiff;
			iPrice = iBasePrice + ((iDiff < TARGET_SCALE_RANGE) ? 0 : TARGET_SCALE_RANGE);
			return iPrice + ((isDown) ? -this.Range : this.Range);
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
				 CommodityId = "TXW",
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