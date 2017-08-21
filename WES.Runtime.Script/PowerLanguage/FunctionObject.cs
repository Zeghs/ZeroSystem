using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Zeghs.Data;
using Zeghs.Events;

namespace PowerLanguage {
	/// <summary>
	///   內建指標方法抽象類別
	/// </summary>
	/// <typeparam name="T">泛型型別</typeparam>
	public abstract class FunctionObject<T> : ISeries<T>, IStudyControl, IDisposable {
		private bool __bDisposed = false;  //Dispose旗標
		private List<IVariables> __cVariables = null;
		private List<Instrument> __cInstruments = null;

		/// <summary>
		///   [取得] 目前資料
		/// </summary>
		public T Value {
			get {
				return this[0];
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
		///   [取得] IInstrument 資訊
		/// </summary>
		protected IInstrument Bars {
			get;
			private set;
		}

		/// <summary>
		///   [取得/設定] 主要資料串流編號
		/// </summary>
		protected int MasterDataStream {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="manageFromStudy">由 CStudyAbstract 自動管理與釋放資源(預設:true)</param>
		public FunctionObject(CStudyAbstract master, int dataStream = 1, bool manageFromStudy = true) {
			this.MasterDataStream = dataStream;
			
			__cVariables = new List<IVariables>(8);
			__cInstruments = master.Instruments;

			int iCount = __cInstruments.Count;
			if (dataStream > 0 && dataStream <= iCount) {
				Instrument cInstrument = __cInstruments[dataStream - 1];
				cInstrument.onPositionChange += Instrument_onPositionChange;

				this.Bars = cInstrument;
				if (manageFromStudy) {
					master.AddFunction(this);
				}

				Create();
				StartCalc();
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
			if (data_stream >= 0 && data_stream < __cInstruments.Count) {
				cInstrument = __cInstruments[data_stream];
			}
			return cInstrument;
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
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

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					Instrument cInstrument = this.Bars as Instrument;
					cInstrument.onPositionChange -= Instrument_onPositionChange;

					StopCalc();
					Destroy();

					ClearVariables();
				}
			}
		}
		
		private void Instrument_onPositionChange(object sender, SeriesPositionChangeEvent e) {
			int iCount = __cVariables.Count;
			if (iCount > 0) {
				lock (__cVariables) {
					Parallel.For(0, iCount, (i) => {
						IVariables cVariables = __cVariables[i];
						cVariables.Move(e.Current);
					});
				}
			}
			CalcBar();
		}
	}
}