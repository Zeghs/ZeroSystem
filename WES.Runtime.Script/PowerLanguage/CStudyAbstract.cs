﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Zeghs.Data;
using Zeghs.Chart;
using Zeghs.Utils;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   基礎腳本抽象類別
	/// </summary>
	public abstract class CStudyAbstract : CStudyControl, IStudyControl {
		private static readonly ILog logger = LogManager.GetLogger(typeof(CStudyAbstract));

		/// <summary>
		///   腳本初始化所觸發的事件
		/// </summary>
		public event EventHandler<ScriptParametersEvent> onScriptParameters = null;

		private bool __bDisposed = false;  //Dispose旗標
		private ZChart __cChart = null;
		private IOutput __cOutputWriter = null;
		private TextContainer __cDrawTexts = null;
		private List<IVariables> __cVariables = null;
		private List<IStudyControl> __cFunctions = null;

		/// <summary>
		///   [取得] 腳本開發者資訊
		/// </summary>
		public ScriptPropertyAttribute About {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] ZChart類別
		/// </summary>
		internal ZChart Chart {
			get {
				return __cChart;
			}
		}

		/// <summary>
		///   [取得] 文字繪製容器
		/// </summary>
		protected ITextContainer DrwText {
			get {
				return __cDrawTexts;
			}
		}

		/// <summary>
		///   [取得] log 日誌紀錄員
		/// </summary>
		protected ILog log {
			get {
				return logger;
			}
		}

		/// <summary>
		///   [取得] 輸出訊息介面
		/// </summary>
		public IOutput Output {
			get {
				return __cOutputWriter;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="_ctx">ZChart 圖表物件</param>
		public CStudyAbstract(object _ctx) {
			__cChart = _ctx as ZChart;
			__cDrawTexts = new TextContainer();
			__cOutputWriter = new OutputWriter();
			__cVariables = new List<IVariables>(16);
			__cFunctions = new List<IStudyControl>(16);
		}

		/// <summary>
		///   加入 FunctionObject 類別方法
		/// </summary>
		/// <param name="func">IStudyControl 介面</param>
		public void AddFunction(IStudyControl func) {
			lock (__cFunctions) {
				__cFunctions.Add(func);
			}
		}

		/// <summary>
		///   加入變數類別方法
		/// </summary>
		/// <param name="var">變數擴充介面</param>
		public void AddVariable(IVariables var) {
			lock (__cVariables) {
				__cVariables.Add(var);
			}
		}

		/// <summary>
		///   取得其他的 Bars 資訊
		/// </summary>
		/// <param name="data_stream">資料串流編號(1 為起始編號)</param>
		/// <returns>返回值: IInstrument 介面</returns>
		public IInstrument BarsOfData(int data_stream) {
			IInstrument cInstrument = null;

			--data_stream;
			if (data_stream >= 0 && data_stream < this.Instruments.Count) {
				cInstrument = this.Instruments[data_stream];
			}
			return cInstrument;
		}

		/// <summary>
		///   更新腳本使用者自訂參數
		/// </summary>
		public virtual void UpdateParameters() {
		}

		/// <summary>
		///   加入使用者自訂形狀物件
		/// </summary>
		/// <typeparam name="T">資料類型型別, 須配合 EPlotSharps 列舉來決定型別(int, double, double[]...)</typeparam>
		/// <param name="args">PlotAttributes繪製參數</param>
		/// <param name="data_stream">資料串流編號(從 1 開始編號)</param>
		protected IPlotObject<T> AddPlot<T>(PlotAttributes args, int data_stream = 1) {
			if (__cChart == null) {
				return null;
			}

			PlotObject<T> cObject = new PlotObject<T>(this, data_stream);
			cObject.Name = args.Name;
			cObject.BGColor = args.BackgroundColor;

			__cChart.AddPlotShape(args, cObject, data_stream);
			return cObject;
		}
	
		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected abstract void CalcBar();
		
		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected virtual void Create() {
		}
		
		/// <summary>
		///   模型釋放時所呼叫的方法
		/// </summary>
		protected virtual void Destroy() {
		}
		
		/// <summary>
		///   開始計算 Bars 時所呼叫的方法
		/// </summary>
		protected virtual void StartCalc() {
		}

		/// <summary>
		///   停止計算 Bars 時所呼叫的方法
		/// </summary>
		protected virtual void StopCalc() {
		}

		/// <summary>
		///   腳本內部資源釋放行為
		/// </summary>
		internal abstract void CStudyDestroy();

		/// <summary>
		///   腳本內部初始化行為
		/// </summary>
		internal abstract void CStudyInitialize();

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		internal override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					base.Dispose(disposing);

					onScriptParameters = null;

					//清除腳本設計者所建立的物件與變數
					try {
						StopCalc();
					} catch(Exception __errExcep1) {
						if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep1.Message, __errExcep1.StackTrace);
					}

					try {
						Destroy();
					} catch(Exception __errExcep2) {
						if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep2.Message, __errExcep2.StackTrace);
					}

					CStudyDestroy();

					//清理使用者繪圖文字物件
					__cDrawTexts.Clear();

					//清理輸出寫入者
					OutputWriter cOutputWriter = __cOutputWriter as OutputWriter;
					cOutputWriter.Dispose();

					//清除函數與變數
					ClearFunctions();
					ClearVariables();
				}
			}
		}

		/// <summary>
		///   計算使用者設計的邏輯方法 CalcBar
		/// </summary>
		internal void OnCalculate() {
			int iCount = __cVariables.Count;
			if (iCount > 0) {
				lock (__cVariables) {
					Parallel.For(0, iCount, (i) => {
						IVariables cVariables = __cVariables[i];

						Instrument cInstrument = this.Instruments[cVariables.DataStream - 1];
						cVariables.Move(cInstrument.CurrentBar);
					});
				}
			}

			try {
				this.CalcBar();
			} catch(Exception __errExcep) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
			}
		}

		/// <summary>
		///   載入腳本後觸發腳本使用者自訂參數事件(將腳本參數利用此事件傳遞到外部)
		/// </summary>
		/// <param name="e">腳本初始化事件</param>
		internal void OnScriptParameters(ScriptParametersEvent e) {
			if (onScriptParameters != null) {
				onScriptParameters(this, e);
			}
		}

		/// <summary>
		///   啟動腳本
		/// </summary>
		internal override void Start() {
			int iCount = this.MaxDataStream;  //加入資訊源(DataStream)
			for (int i = 1; i <= iCount; i++) {
				__cChart.AddSeries(this.BarsOfData(i), i);
			}

			//初始化使用者繪圖字型類別
			__cDrawTexts.Initialate(this);
			__cChart.AddDrwText(__cDrawTexts);

			//腳本初始化工作
			CStudyInitialize();

			try {
				this.Create();
			} catch (Exception __errExcep1) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep1.Message, __errExcep1.StackTrace);
			}
			
			if (!__bDisposed) {  //檢查是否呼叫了 Dispose 方法(在呼叫 Create 方法內, 可能會因為某些資訊不正確而需要停止腳本運作的情況下, 會在呼叫 Dispose 後, 便停止後續的策略執行)
				try {
					this.StartCalc();
				} catch(Exception __errExcep2) {
					if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep2.Message, __errExcep2.StackTrace);
				}
				base.Start();  //呼叫父類別作啟動工作
			}
		}

		private void ClearFunctions() {
			int iCount = __cFunctions.Count;
			if (iCount > 0) {
				lock (__cFunctions) {
					for (int i = 0; i < iCount; i++) {
						IDisposable cDisposer = __cFunctions[i] as IDisposable;
						cDisposer.Dispose();
					}
					__cFunctions.Clear();
				}
			}
		}

		private void ClearVariables() {
			int iCount = __cVariables.Count;
			if (iCount > 0) {
				lock (__cVariables) {
					for (int i = 0; i < iCount; i++) {
						IDisposable cDisposer = __cVariables[i] as IDisposable;
						cDisposer.Dispose();
					}
					__cVariables.Clear();
				}
			}
		}
	}
}  //318行