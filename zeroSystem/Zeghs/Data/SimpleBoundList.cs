using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevAge;

namespace Zeghs.Data {
	internal class SimpleBoundList<T> : AbstractBoundList<T> {
		private List<T> __cSource = null;

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

		internal SimpleBoundList(int capacity) {
			__cSource = new List<T>(capacity);
		}

		internal SimpleBoundList(List<T> list) {
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

		internal virtual void Add(T item) {
			lock (__cSource) {
				__cSource.Add(item);
			}
		}

		internal void SetDataSource(List<T> source) {
			__cSource = source;
		}

		protected void Insert(int index, T item) {
			lock (__cSource) {
				__cSource.Insert(index, item);
			}
		}

		protected override T OnAddNew() {
			T cEditItem = Activator.CreateInstance<T>();
			this.Add(cEditItem);
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
	}
}