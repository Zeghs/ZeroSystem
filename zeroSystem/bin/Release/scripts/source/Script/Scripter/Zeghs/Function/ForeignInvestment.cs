using System;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Utils;

namespace Zeghs.Function {
	public sealed class ForeignInvestment : FunctionObject<ForeignInvestmentGroup> {
		private VariableSeries<ForeignInvestmentGroup> __cForeigns = null;

		/// <summary>
		///   [取得] 台灣指數選擇權未平倉量資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: OpenInterestData 類別</returns>
		public override ForeignInvestmentGroup this[int barsAgo] {
			get {
				return __cForeigns[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		public ForeignInvestment(CStudyAbstract master, int dataStream = 1) 
			: base(master, dataStream) {
		}

		/// <summary>
		///   初始化(讀入歷史台灣指數選擇權未平倉資訊)
		/// </summary>
		public void Initialize() {
			ISeriesSymbolDataRand cSeries = Bars.FullSymbolData;
			int iCount = cSeries.Count;
			for (int i = 0; i < iCount; i++) {
				ForeignInvestmentGroup cData = ForeignInvestmentUtil.Load(cSeries.Time[-i]);
				if (cData != null) {
					__cForeigns.Move(i + 1);
					__cForeigns.Value = cData;
				}
			}
			__cForeigns.Current = 1;
		}

		/// <summary>
		///   設定今日下載下來的三大法人最新多空口數與未平倉量資訊
		/// </summary>
		/// <param name="value">ForeignInvestmentGroup 類別</param>
		public void SetValue(ForeignInvestmentGroup value) {
			__cForeigns.Value = value;
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cForeigns = new VariableSeries<ForeignInvestmentGroup>(this, null, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			//不需要做任何事情
		}
	}
}