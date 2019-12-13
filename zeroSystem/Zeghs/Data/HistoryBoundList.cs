using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zeghs.Data {
	internal sealed class HistoryBoundList : SimpleBoundList<_TradeInfo> {
		private static readonly Dictionary<string, int> __cTable = new Dictionary<string, int>() {
			{ "Ticket", -1 },
			{ "Contracts", 0 },
			{ "Profit", 1 },
			{ "Fee", 2 },
			{ "Tax", 3 }
		};

		private bool __bFilter = false;
		private double[] __cTotals = null;  //總計
		private List<int> __cFilters = null;
		private string __sSymbol = string.Empty;
		private DateTime __cStartTime = DateTime.MinValue, __cStopTime = DateTime.MaxValue;

		public override int Count {
			get {
				return (__bFilter) ? __cFilters.Count : base.Count;
			}
		}

		internal HistoryBoundList(int capacity) 
			: base(capacity) {

			__cTotals = new double[4];
			__cFilters = new List<int>(capacity);

			base.Add(null);
		}

		public override object GetItemValue(int index, PropertyDescriptor property) {
			int iLast = this.Count - 1;
			if (index == iLast) {
				int iIndex = 0;
				string sName = property.Name;
				if (__cTable.TryGetValue(sName, out iIndex)) {
					if (iIndex == -1) {
						return "Summation";
					} else {
						return Math.Round(__cTotals[iIndex], 2);
					}
				} else {
					return string.Empty;
				}
			} else {
				return base.GetItemValue((__bFilter) ? __cFilters[index] : index, property);
			}
		}

		internal override void Add(_TradeInfo item) {
			this.Insert(base.Count - 1, item);

			DateTime cTime = item.GetTime();
			if (!__bFilter || cTime >= __cStartTime && cTime <= __cStopTime) {
				if (__sSymbol.Length == 0 || item.SymbolId.Equals(__sSymbol)) {
					__cTotals[0] += item.Contracts;
					__cTotals[1] += item.Profit;
					__cTotals[2] += item.Fee;
					__cTotals[3] += item.Tax;

					if (__bFilter) {
						lock (__cFilters) {
							__cFilters.Insert(__cFilters.Count - 1, base.Count - 2);
						}
					}
				}
			}
		}

		internal void Filter(string symbol, DateTime start, DateTime stop) {
			__sSymbol = symbol;
			__cStartTime = start;
			__cStopTime = stop;
			
			lock (__cFilters) {
				__cFilters.Clear();
			}

			if (symbol == null && start == DateTime.MinValue && stop == DateTime.MaxValue) {
				__bFilter = false;
			} else {
				__bFilter = true;
				
				int iCount = base.Count - 1;
				double dContracts = 0, dProfit = 0, dFee = 0, dTax = 0;
				for (int i = 0; i < iCount; i++) {
					_TradeInfo cTrade = this.GetItemAt(i);

					DateTime cTime = cTrade.GetTime();
					if (cTime >= start && cTime <= stop) {
						if (symbol.Length == 0 || cTrade.SymbolId.Equals(symbol)) {
							dFee += cTrade.Fee;
							dTax += cTrade.Tax;
							dProfit += cTrade.Profit;
							dContracts += cTrade.Contracts;

							lock (__cFilters) {
								__cFilters.Add(i);
							}
						}
					}
				}

				lock (__cFilters) {
					__cFilters.Add(iCount);
				}

				__cTotals[0] = dContracts;
				__cTotals[1] = dProfit;
				__cTotals[2] = dFee;
				__cTotals[3] = dTax;
			}

			this.Refresh();
		}
		
		protected override void OnClear() {
			lock (__cFilters) {
				__cFilters.Clear();
			}
			
			base.OnClear();
		}
	}
}