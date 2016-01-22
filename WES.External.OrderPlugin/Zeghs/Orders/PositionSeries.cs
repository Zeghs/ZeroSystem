using System;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Orders {
	/// <summary>
	///   留倉部位列表資訊
	/// </summary>
	public sealed class PositionSeries : SeriesAbstract<IMarketPosition> {
		private const int EXPAND_POSITION_COUNT = 32;
		
		private double __dTotalHistoryProfit = 0;  //留倉部位列表內的歷史總損益

		/// <summary>
		///   [取得] 資料總個數
		/// </summary>
		public new int Count {
			get {
				return this.Current;
			}
		}

		/// <summary>
		///   [取得] 已平倉的總損益
		/// </summary>
		public double NetProfit {
			get {
				double dTotals = __dTotalHistoryProfit;
				IMarketPosition cPosition = this[0];  //取得最新的留倉部位資訊
				if (cPosition != null) {
					dTotals += cPosition.Profit;  //將損益值加總(最新的留倉部位可能已經有平倉後的損益, 所以需要加總)
				}
				return dTotals;
			}
		}

		/// <summary>
		///   [取得/設定] 目前資料
		/// </summary>
		public new IMarketPosition Value {
			get {
				return this.GetValue(this.Current - 1);
			}

			set {
				this.SetData(this.Current - 1, value);
				
				IMarketPosition cPosition = this[1];  //取得前一筆留倉部位資訊(0=最新的留倉部位資訊, 可能尚未有損益)
				__dTotalHistoryProfit += cPosition.Profit;  //將損益值加總
			}
		}

		/// <summary>
		///   [取得] 目前或是之前的資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public override IMarketPosition this[int barsAgo] {
			get {
				return this.GetValue(this.Current - barsAgo - 1);
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public PositionSeries() {
			this.Current = 1;
			this.Initialize(EXPAND_POSITION_COUNT);
		}

		private void SetData(int index, IMarketPosition value) {
			int iCount = base.Count;
			if (index >= iCount) {  //如果超過容量, 自動調整集合容量大小
				this.AdjustSize(EXPAND_POSITION_COUNT);  //調整容量
			}
			this.SetValue(index, value);
		}
	}
}