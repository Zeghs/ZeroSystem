using System;

namespace Zeghs.Data {
	/// <summary>
	///   SeriesSymbolData 資料索引子類別
	/// </summary>
	internal sealed class SeriesIndexer {
		private int __iBaseIndex = 0;
		private int __iOldBaseCount = -1;
		private int __iAdjustTotalCount = 0;

		/// <summary>
		///   [取得] 調整的個數總和(往前插入歷史資料的調整總個數)
		/// </summary>
		internal int AdjustTotalCount {
			get {
				return __iAdjustTotalCount;
			}
		}

		/// <summary>
		///   [取得] 序列資料總個數(歷史序列資料+今日即時序列資料)
		/// </summary>
		internal int Count {
			get {
				return this.RealtimeIndex - this.HistoryIndex + 1;
			}
		}

		/// <summary>
		///   [取得/設定] 目前歷史資料索引值
		/// </summary>
		internal int HistoryIndex {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 目前即時資料索引值
		/// </summary>
		internal int RealtimeIndex {
			get;
			set;
		}

		/// <summary>
		///   調整索引值(由前方插入歷史資料調整才需要調整索引值)
		/// </summary>
		/// <param name="adjustCount">調整的個數</param>
		internal void AdjustIndex(int adjustCount) {
			this.HistoryIndex += adjustCount;
			this.RealtimeIndex += adjustCount;

			__iAdjustTotalCount += adjustCount;
		}

		internal int GetBaseIndex(int count) {
			int iIndex = count - 1;
			if (__iOldBaseCount > -1) {
				++this.HistoryIndex;
				iIndex = count - __iOldBaseCount + __iBaseIndex;
			}
			__iOldBaseCount = count;
			return iIndex;
		}

		internal void SetBaseIndex(int baseIndex) {
			__iBaseIndex = baseIndex;
		}
	}
}