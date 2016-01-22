using System;
using System.Collections.Generic;
using PowerLanguage;

namespace PowerLanguage {
	/// <summary>
	///   資料陣列儲存類別
	/// </summary>
	/// <typeparam name="T">資料型態</typeparam>
	public abstract class SeriesAbstract<T> : ISeries<T>, IDisposable {
		private bool __bDisposed = false;
		private List<T> __cSeries = null;

		/// <summary>
		///   [取得] 資料總個數
		/// </summary>
		public int Count {
			get {
				return __cSeries.Count;
			}
		}

		/// <summary>
		///   [取得/設定] 目前索引位置(索引從 1 開始)
		/// </summary>
		public int Current {
			get;
			set;
		}

		/// <summary>
		///   [取得] 目前資料
		/// </summary>
		public T Value {
			get {
				return this.GetValue(this.Current - 1);
			}
		}

		/// <summary>
		///   [取得] 目前或是之前的資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public abstract T this[int barsAgo] {
			get;
		}

		/// <summary>
		///   調整序列空間大小
		/// </summary>
		/// <param name="count">新增加的個數</param>
		/// <param name="isInsert">是否插入在最前面(預設為加入空間在尾端)</param>
		public void AdjustSize(int count, bool isInsert = false) {
			if (isInsert) {
				__cSeries.InsertRange(0, CreateArray(count));
			} else {
				__cSeries.AddRange(CreateArray(count));
			}
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   建立陣列
		/// </summary>
		/// <param name="size">陣列大小</param>
		/// <returns>返回值: 陣列空間</returns>
		protected virtual T[] CreateArray(int size) {
			return new T[size];
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					__cSeries.Clear();
				}
			}
		}

		/// <summary>
		///   取得資料
		/// </summary>
		/// <param name="index">資料絕對索引值</param>
		/// <returns>返回值: 陣列內的資料</returns>
		protected T GetValue(int index) {
			int iCount = __cSeries.Count - 1;
			index = ((index < 0) ? 0 : (index > iCount) ? iCount : index);
			return __cSeries[index];
		}

		/// <summary>
		///   初始化
		/// </summary>
		/// <param name="size">陣列大小</param>
		protected void Initialize(int size) {
			__cSeries = new List<T>(CreateArray(size));
		}

		/// <summary>
		///   設定資料
		/// </summary>
		/// <param name="index">資料絕對索引值</param>
		/// <param name="value">欲設定資料</param>
		protected void SetValue(int index, T value) {
			int iCount = __cSeries.Count;
			if (index >= 0 && index < iCount) {
				__cSeries[index] = value;
			}
		}
	}
}