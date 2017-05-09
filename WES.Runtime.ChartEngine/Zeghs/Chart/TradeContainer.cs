using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Orders;

namespace Zeghs.Chart {
	/// <summary>
	///   交易資料容器存放類別
	/// </summary>
	public sealed class TradeContainer {
		private static void AddTradeFromRange(Dictionary<int, List<ITrade>> source, int min, int max, ITrade item) {
			List<ITrade> cList = GetTrades(source, min);
			cList.Add(item);

			if (max > min) {
				cList = GetTrades(source, max);
				cList.Add(item);
			}
		}

		private static List<ITrade> GetTrades(Dictionary<int, List<ITrade>> source, int barNumber) {
			List<ITrade> cList = null;
			if (!source.TryGetValue(barNumber, out cList)) {
				cList = new List<ITrade>(4);
				source.Add(barNumber, cList);
			}
			return cList;
		}

		private static List<ITrade> GetTradesWithoutCreate(Dictionary<int, List<ITrade>> source, int barNumber) {
			List<ITrade> cList = null;
			source.TryGetValue(barNumber, out cList);
			return cList;
		}

		private int __iPrevHistorys = 0;
		private ITrade __cLastOpenTrade = null;
		private Dictionary<int, List<ITrade>> __cOpenTrades = null;
		private Dictionary<int, List<ITrade>> __cHistoryTrades = null;

		/// <summary>
		///   建構子
		/// </summary>
		public TradeContainer()
			: this(128) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="capacity">預設交易容器數量</param>
		public TradeContainer(int capacity) {
			__cOpenTrades = new Dictionary<int, List<ITrade>>(32);
			__cHistoryTrades = new Dictionary<int, List<ITrade>>(capacity);
		}

		public List<ITrade> GetTradeObject(int barNumber) {
			return GetTrades(__cHistoryTrades, barNumber);
		}

		/// <summary>
		///   取得區間內的交易物件
		/// </summary>
		/// <param name="min">最小 barNumber 編號</param>
		/// <param name="max">最大 barNumber 編號</param>
		/// <returns>返回值: 交易物件的 HashSet 集合</returns>
		public HashSet<ITrade> GetTradeObjects(int min, int max) {
			HashSet<ITrade> cTrades = new HashSet<ITrade>();
			for (int i = min; i <= max; i++) {
				List<ITrade> cList = GetTradesWithoutCreate(__cHistoryTrades, i);
				if (cList != null) {
					int iCount = cList.Count;
					for (int j = 0; j < iCount; j++) {
						cTrades.Add(cList[j]);
					}
				}

				cList = GetTradesWithoutCreate(__cOpenTrades, i);
				if (cList != null) {
					int iCount = cList.Count;
					for (int j = 0; j < iCount; j++) {
						cTrades.Add(cList[j]);
					}
				}
			}
			return cTrades;
		}

		internal bool AddTrades(TradeList<ITrade> openTrades, List<ITrade> historyTrades) {
			bool bRet = false;
			if (openTrades == null) {
				int iCount = historyTrades.Count;
				for (int i = 0; i < iCount; i++) {
					ITrade cTrade = historyTrades[i];
			
					AddTradeFromRange(__cHistoryTrades, cTrade.EntryOrder.BarNumber, cTrade.ExitOrder.BarNumber, cTrade);
				}

				__cLastOpenTrade = null;
				__cOpenTrades.Clear();
				bRet = true;
			} else {
				int iHistoruCount = historyTrades.Count;
				if (iHistoruCount > __iPrevHistorys) {
					__iPrevHistorys = iHistoruCount;
					bRet = true;
				}

				int iIndex = openTrades.Count - 1;
				if (iIndex > -1) {
					ITrade cTrade = openTrades[iIndex];
					if (__cLastOpenTrade != cTrade) {
						AddTradeFromRange(__cOpenTrades, cTrade.EntryOrder.BarNumber, 0, cTrade);
						__cLastOpenTrade = cTrade;
						bRet = true;
					}
				}
			}
			return bRet;
		}
	}
}