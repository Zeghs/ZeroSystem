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

		private double[] __cTotals = null;  //總計

		internal HistoryBoundList(int capacity) 
			: base(capacity) {

			__cTotals = new double[4];
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
				return base.GetItemValue(index, property);
			}
		}

		internal override void Add(_TradeInfo item) {
			__cTotals[0] += item.Contracts;
			__cTotals[1] += item.Profit;
			__cTotals[2] += item.Fee;
			__cTotals[3] += item.Tax;

			this.Insert(this.Count - 1, item);
		}
	}
}