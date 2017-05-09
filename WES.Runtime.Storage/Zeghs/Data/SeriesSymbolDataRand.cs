using System;
using PowerLanguage;

namespace Zeghs.Data {
	internal sealed class SeriesSymbolDataRand : ISeriesSymbolDataRand, IDisposable {
		private bool __bDisposed = false;
		private int __iCurrent = 0;
		private int __iBaseAdjustTotal = 0;
		private int __iIgnoreBarsCount = 0;
		private Series<double> __cLows = null;
		private Series<double> __cHighs = null;
		private Series<double> __cOpens = null;
		private Series<double> __cCloses = null;
		private Series<double> __cVolumes = null;
		private Series<DateTime> __cTimes = null;
		private SeriesIndexer __cIndexer = null;

		/// <summary>
		///   [取得] 收盤價陣列資訊
		/// </summary>
		public ISeries<double> Close {
			get {
				return __cCloses;
			}
		}

		/// <summary>
		///   [取得] 序列資料總個數
		/// </summary>
		public int Count {
			get {
				return __cIndexer.Count;
			}
		}

		/// <summary>
		///   [取得] 目前 Bars 索引值(索引從 1 開始)
		/// </summary>
		public int Current {
			get {
				return __iCurrent;
			}

			internal set {
				__iCurrent = __cIndexer.AdjustTotalCount - __iBaseAdjustTotal + __iIgnoreBarsCount + value;

				int iIndex = __cIndexer.HistoryIndex + __iCurrent;
				__cOpens.Current = iIndex;
				__cHighs.Current = iIndex;
				__cLows.Current = iIndex;
				__cCloses.Current = iIndex;
				__cTimes.Current = iIndex;
				__cVolumes.Current = iIndex;
			}
		}

		/// <summary>
		///   [取得] 最高價陣列資訊
		/// </summary>
		public ISeries<double> High {
			get {
				return __cHighs;
			}
		}

		/// <summary>
		///   [取得] 最低價陣列資訊
		/// </summary>
		public ISeries<double> Low {
			get {
				return __cLows;
			}
		}

		/// <summary>
		///   [取得] 開盤價陣列資訊
		/// </summary>
		public ISeries<double> Open {
			get {
				return __cOpens;
			}
		}

		/// <summary>
		///   [取得] 日期時間陣列資訊
		/// </summary>
		public ISeries<DateTime> Time {
			get {
				return __cTimes;
			}
		}

		/// <summary>
		///   [取得] 成交量陣列資訊
		/// </summary>
		public ISeries<double> Volume {
			get {
				return __cVolumes;
			}
		}

		/// <summary>
		///   [取得] Bars 資料總個數
		/// </summary>
		internal int BarsCount {
			get {
				return this.BarsSize - (__cCloses.Count - __cIndexer.Count);
			}
		}

		/// <summary>
		///   [取得] Bars 資料容器大小(包含預留的即時資料空間)
		/// </summary>
		internal int BarsSize {
			get {
				return __cCloses.Count - __iIgnoreBarsCount - (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal);
			}
		}

		internal bool IsLastBars {
			get {
				int iIndex = __cIndexer.HistoryIndex + __iCurrent - 1;
				return iIndex == __cIndexer.RealtimeIndex;
			}
		}

		internal SeriesSymbolDataRand(SeriesSymbolData source, int maxBarsReferance) {
			__cIndexer = source.Indexer;
			__iBaseAdjustTotal = __cIndexer.AdjustTotalCount;
			source.Clone(out __cTimes, out __cOpens, out __cHighs, out __cLows, out __cCloses, out __cVolumes);

			if (maxBarsReferance > 0) {
				__iIgnoreBarsCount = __cCloses.Count - source.RealtimeCount - maxBarsReferance;
				if (__iIgnoreBarsCount < 0) {
					__iIgnoreBarsCount = 0;
				}
			}
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal int TryOutside() {
			int iRet = 0, iIndex = __cIndexer.HistoryIndex + __iCurrent - 1;
			if (iIndex > __cIndexer.RealtimeIndex) {
				iRet = __cIndexer.Count - __iIgnoreBarsCount - (__cIndexer.AdjustTotalCount - __iBaseAdjustTotal);
			}
			return iRet;
		}
		
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					__cTimes.Dispose();
					__cOpens.Dispose();
					__cHighs.Dispose();
					__cLows.Dispose();
					__cCloses.Dispose();
					__cVolumes.Dispose();
				}
			}
		}
	}
}  //170行