using System;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Utils;

namespace Zeghs.Function {
	/// <summary>
	///   台灣指數選擇權未平倉量指標
	/// </summary>
	public sealed class OpenInterest : FunctionObject<OpenInterestData> {
		private VariableSeries<OpenInterestData> __cInterest = null;

		/// <summary>
		///   [取得] 台灣指數選擇權未平倉量資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: OpenInterestData 類別</returns>
		public override OpenInterestData this[int barsAgo] {
			get {
				return __cInterest[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		public OpenInterest(CStudyAbstract master, int dataStream = 1) 
			: base(master, dataStream) {
		}

		/// <summary>
		///   初始化(讀入歷史台灣指數選擇權未平倉資訊)
		/// </summary>
		public void Initialize() {
			ISeriesSymbolDataRand cSeries = Bars.FullSymbolData;
			int iCount = cSeries.Count;
			for (int i = 0; i < iCount; i++) {
				OpenInterestData cData = OpenInterestUtil.Load(cSeries.Time[-i]);
				if (cData != null) {
					__cInterest.Move(i + 1);
					__cInterest.Value = cData;
				}
			}
			__cInterest.Current = 1;
		}

		/// <summary>
		///   設定今日下載下來的台灣指數選擇權最新未平倉量資訊
		/// </summary>
		/// <param name="value">OpenInterestData 類別</param>
		public void SetValue(OpenInterestData value) {
			__cInterest.Value = value;
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cInterest = new VariableSeries<OpenInterestData>(this, null, this.MasterDataStream);
		}
		
		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			//不需要做任何事情
		}
	}
}