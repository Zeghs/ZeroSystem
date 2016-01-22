using System;
using System.Collections.Generic;
using Zeghs.Data;

namespace PowerLanguage {
	/// <summary>
	///    變數序列類別
	/// </summary>
	/// <typeparam name="T">泛型型態</typeparam>
	public sealed class VariableSeries<T> : SeriesAbstract<T>, IVariables {
		private bool __bDisposed = false;
		private T __cDefault = default(T);
		private Instrument __cBars = null;

		/// <summary>
		///   [取得] 資料串流編號
		/// </summary>
		public int DataStream {
			get;
			private set;
		}

		/// <summary>
		///   [設定] 預設值
		/// </summary>
		public T DefaultValue {
			set {
				__cDefault = value;
			}
		}

		/// <summary>
		///   [取得/設定] 目前資料
		/// </summary>
		public new T Value {
			get {
				return this.GetValue(this.Current - 1);
			}

			set {
				this.SetValue(this.Current - 1, value);
			}
		}

		/// <summary>
		///   [取得] 目前或是之前的資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public override T this[int barsAgo] {
			get {
				return this.GetValue(this.Current - barsAgo - 1);
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyControl 類別</param>
		public VariableSeries(IStudyControl master)
			: this(master, default(T), 1) {
		}
		
		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyControl 類別</param>
		/// <param name="defaultVal">預設值</param>
		public VariableSeries(IStudyControl master, T defaultVal) 
			: this(master, defaultVal, 1) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyControl 類別</param>
		/// <param name="defaultVal">預設值</param>
		/// <param name="dataStream">資料串流編號</param>
		public VariableSeries(IStudyControl master, T defaultVal, int dataStream) {
			__cDefault = defaultVal;
			this.DataStream = dataStream;

			__cBars = master.BarsOfData(dataStream) as Instrument;
			SeriesSymbolData cSeries = __cBars.SeriesSymbolData;
			cSeries.onReset += SeriesSymbolData_onReset;  //設定清盤重置事件

			SeriesSymbolDataRand cSeriesRand = __cBars.FullSymbolData as SeriesSymbolDataRand;
			this.Initialize(cSeriesRand.BarsSize);
			master.AddVariable(this);
		}

		/// <summary>
		///  移動 Current 索引
		/// </summary>
		/// <param name="index">索引值</param>
		public void Move(int index) {
			this.Current = index;
		}

		/// <summary>
		///   建立陣列(複寫原來的方法)
		/// </summary>
		/// <param name="size">陣列大小</param>
		/// <returns>返回值: 陣列空間</returns>
		protected override T[] CreateArray(int size) {
			T[] cArray = base.CreateArray(size);
			if (!EqualityComparer<T>.Default.Equals(__cDefault, default(T))) {
				for (int i = 0; i < size; i++) {
					cArray[i] = __cDefault;
				}
			}
			return cArray;
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					SeriesSymbolData cSeries = __cBars.SeriesSymbolData;
					cSeries.onReset -= SeriesSymbolData_onReset;

					base.Dispose();
				}
			}
		}

		private void SeriesSymbolData_onReset(object sender, EventArgs e) {
			SeriesSymbolDataRand cSeriesRand = __cBars.FullSymbolData as SeriesSymbolDataRand;
			int iSize = cSeriesRand.BarsSize - this.Count;
			if (iSize > 0) {
				this.AdjustSize(iSize);  //調整容量
			}
		}
	}
}