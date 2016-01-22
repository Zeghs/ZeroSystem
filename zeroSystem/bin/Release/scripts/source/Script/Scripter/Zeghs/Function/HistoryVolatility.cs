using System;
using PowerLanguage;

namespace Zeghs.Function {
	/// <summary>
	///   歷史波動率指標
	/// </summary>
	public sealed class HistoryVolatility : FunctionObject<double> {
		private VariableSeries<double> __cLNs = null;
		private VariableSeries<double> __cLNDiffs = null;
		private VariableSeries<double> __cSTDEVs = null;
		private VariableSeries<double> __cHVs = null;

		/// <summary>
		///   [取得/設定] 標準差週期長度(預設值=180)
		/// </summary>
		public int Length {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 一年長度(預設值=250)
		/// </summary>
		public int YearLength {
			get;
			set;
		}

		/// <summary>
		///   [取得] 歷史波動率
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: 歷史波動率</returns>
		public override double this[int barsAgo] {
			get {
				return __cHVs[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		public HistoryVolatility(CStudyAbstract master, int dataStream = 1) 
			: base(master, dataStream) {

			this.Length = 180;
			this.YearLength = 250;
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cLNs = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cLNDiffs = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cSTDEVs = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cHVs = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}
		
		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			__cLNs.Value = Math.Log(Bars.Close[0]);
			__cLNDiffs.Value = __cLNs[0] - __cLNs[1];
			__cSTDEVs.Value = StandardDEV(__cLNDiffs.Average(this.Length));
			__cHVs.Value = __cSTDEVs.Value * Math.Sqrt(this.YearLength);
		}

		/// <summary>
		///   計算標準差
		/// </summary>
		/// <param name="middle">布林通道中線移動平均值</param>
		/// <returns>返回值: 平均標準差</returns>
		private double StandardDEV(double middle) {
			double dRet = 0;
			for (int i = this.Length - 1; i >= 0; i--) {
				dRet += Math.Pow(__cLNDiffs[i] - middle, 2);
			}
			return Math.Sqrt(dRet / this.Length);
		}
	}
}