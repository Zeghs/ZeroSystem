using System;
using PowerLanguage;

namespace PowerLanguage.Function {
	/// <summary>
	///   [BollingerBands] 布林通道指標
	/// </summary>
	public sealed class BollingerBands : FunctionObject<double> {
		private _Variable<double> __cPrevSMA;  //儲存前一筆的移動平均值(可以加快計算速度)
		private VariableSeries<double> __cUP = null;
		private VariableSeries<double> __cMD = null;
		private VariableSeries<double> __cDN = null;
		private VariableSeries<double> __cBB = null;
		private VariableSeries<double> __cBW = null;

		/// <summary>
		///   [取得] 布林通道的通道寬度
		/// </summary>
		public ISeries<double> BW {
			get {
				return __cBW;
			}
		}

		/// <summary>
		///   [取得] 布林通道下限
		/// </summary>
		public ISeries<double> DN {
			get {
				return __cDN;
			}
		}

		/// <summary>
		///   [取得/設定] 週期長度
		/// </summary>
		public int Length {
			get;
			set;
		}

		/// <summary>
		///   [取得] 布林通道中線(移動平均中線)
		/// </summary>
		public ISeries<double> MD {
			get {
				return __cMD;
			}
		}

		/// <summary>
		///   [取得] 布林通道上限
		/// </summary>
		public ISeries<double> UP {
			get {
				return __cUP;
			}
		}

		/// <summary>
		///   [取得] 布林通道%B極限值
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cBB[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="manageFromStudy">由系統自動管理與釋放資源(預設:true)</param>
		public BollingerBands(CStudyAbstract master, int dataStream = 1, bool manageFromStudy = true) 
			: base(master, dataStream) {

			this.Length = 20;  //預設周期長度為20
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cPrevSMA = new _Variable<double>(this, 0, this.MasterDataStream);
			__cUP = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cMD = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cDN = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cBB = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cBW = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			bool bChange = Bars.Status == EBarState.Close;
			if (bChange) {  //只計算收線狀態
				double dMD = Bars.Close.Average(this.Length, EAverageMode.SMA, __cPrevSMA.Previous);
				double dSDEV = StandardDEV(dMD);
				double dUP = dMD + 2 * dSDEV;
				double dDN = dMD - 2 * dSDEV;

				__cMD.Value = dMD;
				__cUP.Value = dUP;
				__cDN.Value = dDN;
				__cBB.Value = (Bars.Close.Value - dDN) / (dUP - dDN);
				__cBW.Value = (dUP - dDN) / dMD;
				__cPrevSMA.Value = dMD;
			}
		}

		/// <summary>
		///   計算標準差
		/// </summary>
		/// <param name="middle">布林通道中線移動平均值</param>
		/// <returns>返回值: 平均標準差</returns>
		private double StandardDEV(double middle) {
			double dRet = 0;
			for (int i = this.Length - 1; i >= 0; i--) {
				dRet += Math.Pow(Bars.Close[i] - middle, 2);
			}
			return Math.Sqrt(dRet / this.Length);
		}
	}
}