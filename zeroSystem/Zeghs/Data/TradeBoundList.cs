using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zeghs.Data {
	internal sealed class TradeBoundList : AbstractBoundList<_TradeInfo> {
		private static readonly Dictionary<string, int> __cTable = new Dictionary<string, int>() {
			{ "Ticket", -1 },
			{ "Contracts", 0 },
			{ "Profit", 1 },
			{ "Fee", 2 },
			{ "Tax", 3 }
		};

		private bool __bSubTotal = false;
		private double[] __cTotals = null;  //小計
		private List<_TradeInfo> __cSource = null;
		private Dictionary<string, int> __cIndex = null;

		public new bool AllowEdit {
			get {
				return false;
			}
		}

		public new bool AllowNew {
			get {
				return false;
			}
		}

		public new bool AllowSort {
			get {
				return false;
			}
		}

		public override int Count {
			get {
				return __cSource.Count;
			}
		}

		public override object this[int index] {
			get {
				return __cSource[index];
			}
		}

		internal bool IsSubTotal {
			get {
				return __bSubTotal;
			}
		}

		internal TradeBoundList(int capacity, bool subTotal = false) {
			__cSource = new List<_TradeInfo>(capacity);
			__cIndex = new Dictionary<string, int>(capacity);

			__bSubTotal = subTotal;
			if (__bSubTotal) {
				__cSource.Add(null);
				__cTotals = new double[4];
			}
		}

		public override void ApplySort(ListSortDescriptionCollection sorts) {
		}

		public override object GetItemValue(int index, PropertyDescriptor property) {
			int iLast = this.Count - 1;
			if (this.IsSubTotal && index == iLast) {
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

		public override int IndexOf(object item) {
			return this.IndexOf((item as _TradeInfo).Ticket);
		}

		internal void Add(_TradeInfo item) {
			int iIndex = 0;
			lock (__cIndex) {
				string cKey = item.Ticket;
				if (!__cIndex.TryGetValue(cKey, out iIndex)) {
					if (this.IsSubTotal) {
						int iRow = __cSource.Count - 1;
						__cSource.Insert(iRow, item);
						__cIndex.Add(cKey, iRow);
					} else {
						__cIndex.Add(cKey, __cSource.Count);
						__cSource.Add(item);
					}
				}
			}
		}

		internal _TradeInfo GetItemAt(int index) {
			return __cSource[index];
		}

		internal int IndexOf(string ticket) {
			int iRet = -1;
			bool bHave = false;
			lock (__cIndex) {
				bHave = __cIndex.TryGetValue(ticket, out iRet);
			}
			return (bHave) ? iRet : -1;
		}

		internal void Remove(string ticket) {
			int iIndex = 0;
			lock (__cIndex) {
				if (__cIndex.TryGetValue(ticket, out iIndex)) {
					int iLast = __cSource.Count - ((this.IsSubTotal) ? 2 : 1);
					if (iLast > 0 && iLast > iIndex) {
						_TradeInfo cLast = __cSource[iLast];

						string cLastKey = cLast.Ticket;
						__cIndex[cLastKey] = iIndex;
						__cSource[iIndex] = cLast;
					}

					__cSource.RemoveAt(iLast);
					__cIndex.Remove(ticket);
				}
			}
		}

		internal void SubTotal() {
			if (__bSubTotal) {
				double dContracts = 0, dProfits = 0, dFees = 0, dTaxes = 0;
				int iCount = __cSource.Count;
				for (int i = iCount - 2; i >= 0; i--) {
					_TradeInfo cTrade = __cSource[i];
					dContracts += cTrade.Contracts;
					dProfits += cTrade.Profit;
					dFees += cTrade.Fee;
					dTaxes += cTrade.Tax;
				}

				__cTotals[0] = dContracts;
				__cTotals[1] = dProfits;
				__cTotals[2] = dFees;
				__cTotals[3] = dTaxes;
			}
		}

		protected override _TradeInfo OnAddNew() {
			return null;
		}

		protected override void OnClear() {
			lock (__cIndex) {
				__cIndex.Clear();
				__cSource.Clear();
			}
		}

		protected override void OnRemoveAt(int index) {
			if (__cIndex.Count > 0) {
				Remove(GetItemAt(index).Ticket);
			}
		}
	}
}