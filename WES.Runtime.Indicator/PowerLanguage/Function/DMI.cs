using System;
using PowerLanguage;

namespace PowerLanguage.Function {
	/// <summary>
	///   [DMI] 趨向指標
	/// </summary>
	public sealed class DMI : FunctionObject<double> {
		private _Variable<double> __cPrevATR;  //儲存前一筆的ATR值(可以加快計算速度)
		private _Variable<double> __cPrevADMP;  //儲存前一筆的DMPlus值(可以加快計算速度)
		private _Variable<double> __cPrevADMM;  //儲存前一筆的ATR值(可以加快計算速度)
		private VariableSeries<double> __cADX = null;
		private VariableSeries<double> __cDIPlus = null;
		private VariableSeries<double> __cDMPlus = null;
		private VariableSeries<double> __cDIMinus = null;
		private VariableSeries<double> __cDMMinus = null;

		/// <summary>
		///   [取得/設定] ADX 週期長度
		/// </summary>
		public int ADXLength {
			get;
			set;
		}

		/// <summary>
		///   [取得] 趨向指標 -DI 值
		/// </summary>
		public ISeries<double> DIMinus {
			get {
				return __cDIMinus;
			}
		}

		/// <summary>
		///   [取得] 趨向指標 +DI 值
		/// </summary>
		public ISeries<double> DIPlus {
			get {
				return __cDIPlus;
			}
		}

		/// <summary>
		///   [取得/設定] DMI 週期長度
		/// </summary>
		public int DMILength {
			get;
			set;
		}

		/// <summary>
		///   [取得] 趨向指標 ADX 值
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cADX[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="manageFromStudy">由系統自動管理與釋放資源(預設:true)</param>
		public DMI(CStudyAbstract master, int dataStream = 1, bool manageFromStudy = true) 
			: base(master, dataStream) {

			this.ADXLength = 6;   //ADX 預設周期長度為6
			this.DMILength = 14;  //DMI 預設周期長度為14
		}
		
		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cPrevATR = new _Variable<double>(this, 0, this.MasterDataStream);
			__cPrevADMP = new _Variable<double>(this, 0, this.MasterDataStream);
			__cPrevADMM = new _Variable<double>(this, 0, this.MasterDataStream);
			__cADX = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cDIPlus = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cDMPlus = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cDIMinus = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cDMMinus = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			bool bChange = Bars.Status == EBarState.Close;
			if (bChange) {  //只計算收線狀態
				double dDM_P = Bars.High[0] - Bars.High[1];
				double dDM_M = Bars.Low[1] - Bars.Low[0];
				if (dDM_P > dDM_M && dDM_P > 0) {
					__cDMPlus.Value = dDM_P;
				} else if (dDM_P < dDM_M && dDM_M > 0) {
					__cDMMinus.Value = dDM_M;
				}

				double dATR = AvgTrueRange.AverageTrueRange(this, this.DMILength, this.MasterDataStream, __cPrevATR.Previous);
				double dADMP = (__cPrevADMP.Previous * (this.DMILength - 1) + __cDMPlus.Value) / this.DMILength;
				double dADMM = (__cPrevADMM.Previous * (this.DMILength - 1) + __cDMMinus.Value) / this.DMILength;
				double dDIPlus = dADMP / dATR * 100;
				double dDIMinus = dADMM / dATR * 100;
				double dSum = dDIPlus + dDIMinus;
				double dDX = Math.Abs(dDIPlus - dDIMinus) / ((dSum == 0) ? 1 : dSum) * 100;
				__cDIPlus.Value = dDIPlus;
				__cDIMinus.Value = dDIMinus;
				__cADX.Value = (__cADX[1] * (this.ADXLength - 1) + dDX) / this.ADXLength;
				
				__cPrevATR.Value = dATR;
				__cPrevADMP.Value = dADMP;
				__cPrevADMM.Value = dADMM;
			}
		}
	}
}