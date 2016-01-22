using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevAge;
using DevAge.ComponentModel;

namespace Zeghs.Data {
	[Serializable]
	internal abstract class AbstractBoundList<T> : IBoundList {
		public event ItemDeletedEventHandler ItemDeleted = null;
		public event EventHandler ListCleared = null;
		public event ListChangedEventHandler ListChanged = null;

		private T __cEditItem;
		private int __iEditIndex = -1;
		private bool __bAdding = false;
		private Dictionary<string, Comparison<T>> __cComparers = null;
		private Dictionary<PropertyDescriptor, object> __cPreviousValues = new Dictionary<PropertyDescriptor, object>();

		public bool AllowDelete {
			get;
			set;
		}

		public bool AllowEdit {
			get;
			set;
		}

		public bool AllowNew {
			get;
			set;
		}

		public bool AllowSort {
			get;
			set;
		}

		public int BeginAddNew() {
			if (__cEditItem != null) {
				return __iEditIndex;
			}

			__cEditItem = OnAddNew();
			__iEditIndex = Count - 1;
			__bAdding = true;

			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, __iEditIndex));
			return __iEditIndex;
		}

		public void BeginEdit(int index) {
			if (__cEditItem != null) {
				return;
			}

			__cEditItem = (T) this[index];
			__iEditIndex = index;
		}

		public void EndEdit(bool cancel) {
			if (__cEditItem == null) {
				return;
			}

			if (cancel) {
				if (__bAdding) {
					RemoveAt(__iEditIndex);
				} else {
					foreach (KeyValuePair<PropertyDescriptor, object> editVal in __cPreviousValues) {
						editVal.Key.SetValue(__cEditItem, editVal.Value);
					}
				}
			}

			__bAdding = false;
			__iEditIndex = -1;
			__cEditItem = default(T);
			__cPreviousValues.Clear();

			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public void Clear() {
			__bAdding = false;
			__iEditIndex = -1;
			__cEditItem = default(T);
			__cPreviousValues.Clear();

			OnClear();

			OnListCleared(EventArgs.Empty);
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public object EditedObject {
			get {
				return __cEditItem;
			}
		}

		public void RemoveAt(int index) {
			T item = (T) this[index];

			OnRemoveAt(index);

			OnItemDeleted(new ItemDeletedEventArgs(item));
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public PropertyDescriptorCollection GetItemProperties() {
			return TypeDescriptor.GetProperties(typeof(T));
		}

		public object GetItemValue(int index, PropertyDescriptor property) {
			return property.GetValue(this[index]);
		}

		public PropertyDescriptor GetItemProperty(string name, StringComparison comparison) {
			foreach (System.ComponentModel.PropertyDescriptor prop in GetItemProperties()) {
				if (prop.Name.Equals(name, comparison)) {
					return prop;
				}
			}

			return null;
		}

		public void SetEditValue(PropertyDescriptor property, object value) {
			if (__cEditItem == null) {
				return;
			}

			if (__cPreviousValues.ContainsKey(property) == false) {
				__cPreviousValues.Add(property, property.GetValue(__cEditItem));
			}

			property.SetValue(__cEditItem, value);

			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, __iEditIndex, property));
		}

		internal void Refresh() {
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		internal void SetComparers(Dictionary<string, Comparison<T>> compares) {
			__cComparers = compares;
		}

		protected Comparison<T> GetComparer(ListSortDescription sort) {
			if (__cComparers != null) {
				string sKey = sort.PropertyDescriptor.DisplayName + ((sort.SortDirection == ListSortDirection.Ascending) ? "_A" : "_D");

				Comparison<T> cComparison = null;
				__cComparers.TryGetValue(sKey, out cComparison);
				return cComparison;
			}
			return null;
		}

		protected virtual void OnListChanged(ListChangedEventArgs e) {
			if (ListChanged != null) {
				ListChanged(this, e);
			}
		}

		protected virtual void OnListCleared(EventArgs e) {
			if (ListCleared != null) {
				ListCleared(this, e);
			}
		}

		protected virtual void OnItemDeleted(ItemDeletedEventArgs e) {
			if (ItemDeleted != null) {
				ItemDeleted(this, e);
			}
		}

		#region Abstract methods
		public abstract int Count {
			get;
		}

		public abstract object this[int index] {
			get;
		}

		public abstract void ApplySort(ListSortDescriptionCollection sorts);

		public abstract int IndexOf(object item);

		protected abstract T OnAddNew();
		
		protected abstract void OnRemoveAt(int index);
		
		protected abstract void OnClear();
		#endregion
	}
}