using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zeghs.Data {
	internal sealed class TradeBoundList : AbstractBoundList<_TradeInfo> {
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

		internal TradeBoundList(int capacity) {
			__cSource = new List<_TradeInfo>(capacity);
			__cIndex = new Dictionary<string, int>(capacity);
		}

		public override void ApplySort(ListSortDescriptionCollection sorts) {
		}

		public override int IndexOf(object item) {
			return this.IndexOf((item as _TradeInfo).Ticket);
		}

		internal void Add(_TradeInfo item) {
			int iIndex = 0;
			lock (__cIndex) {
				string cKey = item.Ticket;
				if (!__cIndex.TryGetValue(cKey, out iIndex)) {
					__cIndex.Add(cKey, __cSource.Count);
					__cSource.Add(item);
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
					int iLast = __cSource.Count - 1;
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