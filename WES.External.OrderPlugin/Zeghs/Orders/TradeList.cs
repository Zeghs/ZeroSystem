using System;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Orders {
	/// <summary>
	///   交易明細列表類別
	/// </summary>
	public class TradeList<T> where T : ITradeTicket {
		private List<T> __cTrades = null;
		private Dictionary<string, int> __cNames = null;
		private Dictionary<string, int> __cIndexs = null;

		/// <summary>
		///   [取得] 訂單資訊列表個數
		/// </summary>
		public int Count {
			get {
				return __cTrades.Count;
			}
		}

		/// <summary>
		///   [取得] 交易訂單明細列表
		/// </summary>
		public List<T> Trades {
			get {
				return __cTrades;
			}
		}

		/// <summary>
		///   [取得] 訂單資訊
		/// </summary>
		public T this[int index] {
			get {
				index = (index < 0) ? 0 : (index < __cTrades.Count) ? index : 0;
				return __cTrades[index];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="capacity">擴充的容量大小</param>
		public TradeList(int capacity) {
			__cTrades = new List<T>(capacity);
			__cNames = new Dictionary<string, int>(capacity);
			__cIndexs = new Dictionary<string, int>(capacity);
		}

		/// <summary>
		///   加入訂單資訊
		/// </summary>
		/// <param name="trade">訂單資訊</param>
		public void Add(T trade) {
			string sTicket = trade.Ticket;
			lock (__cIndexs) {
				if (!__cIndexs.ContainsKey(sTicket)) {
					int iIndex = __cTrades.Count;
					__cTrades.Add(trade);
					__cIndexs.Add(sTicket, iIndex);

					string sName = trade.Name;
					if (!__cNames.ContainsKey(sName)) {
						__cNames.Add(sName, iIndex);
					}
				}
			}
		}

		/// <summary>
		///   清除訂單資訊
		/// </summary>
		public virtual void Clear() {
			lock (__cIndexs) {
				__cNames.Clear();
				__cTrades.Clear();
				__cIndexs.Clear();
			}
		}

		/// <summary>
		///   取得訂單資訊
		/// </summary>
		/// <param name="ticket">訂單編號</param>
		public T GetTrade(string ticket) {
			int iIndex = 0;
			T cOrder = default(T);

			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(ticket, out iIndex)) {
					cOrder = __cTrades[iIndex];
				}
			}
			return cOrder;
		}

		/// <summary>
		///   從訂單註解取得訂單資訊
		/// </summary>
		/// <param name="name">訂單註解</param>
		public T GetTradeFromName(string name) {
			int iIndex = 0;
			T cOrder = default(T);

			lock (__cIndexs) {
				if (__cNames.TryGetValue(name, out iIndex)) {
					cOrder = __cTrades[iIndex];
				}
			}
			return cOrder;
		}

		/// <summary>
		///   移除訂單資訊
		/// </summary>
		/// <param name="ticket">訂單編號</param>
		public void Remove(string ticket) {
			int iIndex = 0;
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(ticket, out iIndex)) {
					T cTrade = __cTrades[iIndex];
					int iLast = __cTrades.Count - 1;
					if (iLast > 0 && iLast > iIndex) {
						T cLast = __cTrades[iLast];

						__cNames[cLast.Name] = iIndex;
						__cIndexs[cLast.Ticket] = iIndex;
						__cTrades[iIndex] = cLast;
					}

					__cIndexs.Remove(ticket);
					__cTrades.RemoveAt(iLast);
					__cNames.Remove(cTrade.Name);
				}
			}
		}
	}
}