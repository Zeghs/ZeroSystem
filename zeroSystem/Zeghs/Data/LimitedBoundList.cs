using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevAge;

namespace Zeghs.Data {
	internal sealed class LimitedBoundList<T> : AbstractBoundList<T> {
		private int __iClipCount = 0;
		private int __iLimitCount = 0;
		private List<T> __cSource = null;

		public override int Count {
			get {
				return __cSource.Count;
			}
		}

		internal int ClipCount {
			get {
				return __iClipCount;
			}
		}

		internal int LimitCount {
			get {
				return __iLimitCount;
			}
		}

		public override object this[int index] {
			get {
				return __cSource[index];
			}
		}

		internal LimitedBoundList(int capacity) {
			__cSource = new List<T>(capacity);
		}

		internal LimitedBoundList(List<T> list) {
			__cSource = list;
		}

		public override void ApplySort(ListSortDescriptionCollection sorts) {
			Comparison<T> cComparison = GetComparer(sorts[0]);
			if (cComparison != null) {
				__cSource.Sort(cComparison);
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}

		public override int IndexOf(object item) {
			return __cSource.IndexOf((T) item);
		}

		internal void Add(T item) {
			lock (__cSource) {
				__cSource.Add(item);
				
				int iCount = __cSource.Count;
				if (iCount > __iLimitCount) {
					Reduce();
				}
			}
		}

		internal void SetDataSource(List<T> source) {
			__cSource = source;
		}

		internal void SetReduce(int limit, int clip) {
			__iClipCount = clip;
			__iLimitCount = limit;
		}

		protected override T OnAddNew() {
			T cEditItem = default(T);
			lock (__cSource) {
				int iCount = __cSource.Count + 1;
				if (iCount > __iLimitCount) {
					Reduce();
				}

				cEditItem = Activator.CreateInstance<T>();
				__cSource.Add(cEditItem);
			}
			return cEditItem;
		}

		protected override void OnClear() {
			lock (__cSource) {
				__cSource.Clear();
			}
		}

		protected override void OnRemoveAt(int index) {
			lock (__cSource) {
				__cSource.RemoveAt(index);
			}
		}

		private void Reduce() {
			T[] cArray = new T[__iClipCount];
			__cSource.CopyTo(__cSource.Count - __iClipCount - 1, cArray, 0, __iClipCount);

			__cSource.Clear();
			__cSource.AddRange(cArray);
		}
	}
}