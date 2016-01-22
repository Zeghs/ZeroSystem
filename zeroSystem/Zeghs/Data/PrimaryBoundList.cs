using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevAge;

namespace Zeghs.Data {
	internal sealed class PrimaryBoundList<TKey, TValue> : AbstractBoundList<TValue> {
		private List<TValue> __cSource = null;
		private Dictionary<TKey, int> __cIndex = null;
		private Func<TValue, TKey> __cGetPrimary = null;

		public new bool AllowNew {
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

		internal PrimaryBoundList(int capacity) {
			__cSource = new List<TValue>(capacity);
			__cIndex = new Dictionary<TKey, int>(capacity);
		}

		public override void ApplySort(ListSortDescriptionCollection sorts) {
			Comparison<TValue> cComparison = GetComparer(sorts[0]);
			if (cComparison != null) {
				__cSource.Sort(cComparison);

				int iCount = __cSource.Count;
				for (int i = 0; i < iCount; i++) {
					__cIndex[__cGetPrimary(__cSource[i])] = i;
				}
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}

		public override int IndexOf(object item) {
			return this.IndexOf(__cGetPrimary((TValue) item));
		}

		internal void Add(TValue item) {
			int iIndex = 0;
			lock (__cIndex) {
				TKey cKey = __cGetPrimary(item);
				if (!__cIndex.TryGetValue(cKey, out iIndex)) {
					__cIndex.Add(cKey, __cSource.Count);
					__cSource.Add(item);
				}
			}
		}

		internal TValue GetItemAt(int index) {
			return __cSource[index];
		}

		internal int IndexOf(TKey primary) {
			int iRet = -1;
			bool bHave = __cIndex.TryGetValue(primary, out iRet);
			return (bHave) ? iRet : -1;
		}

		internal void Remove(TKey primary) {
			int iIndex = 0;
			lock (__cIndex) {
				if (__cIndex.TryGetValue(primary, out iIndex)) {
					int iLast = __cSource.Count - 1;
					if (iLast > 0 && iLast > iIndex) {
						TValue cLast = __cSource[iLast];

						TKey cLastKey = __cGetPrimary(cLast);
						__cIndex[cLastKey] = iIndex;
						__cSource[iIndex] = cLast;
					}

					__cSource.RemoveAt(iLast);
					__cIndex.Remove(primary);
				}
			}
		}

		internal void SetFunctionForGetPrimary(Func<TValue, TKey> func) {
			__cGetPrimary = func;
		}

		protected override TValue OnAddNew() {
			return default(TValue);
		}

		protected override void OnClear() {
			lock (__cIndex) {
				__cIndex.Clear();
				__cSource.Clear();
			}
		}

		protected override void OnRemoveAt(int index) {
			TKey cKey = __cGetPrimary(__cSource[index]);
			Remove(cKey);
		}
	}
}