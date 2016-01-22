using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Orders;

namespace Zeghs.Events {
	/// <summary>
	///   回報事件類別
	/// </summary>
	public sealed class ResponseEvent : EventArgs {
		private string __sSymbolId = null;
		private ITradeOrder __cTradeOrder = null;
		private List<ITrade> __cCloseTrades = null;
		private TradeList<ITrade> __cOpenTrades = null;
		private ResponseType __cType = ResponseType.None;

		/// <summary>
		///   [取得] 已平倉交易單列表
		/// </summary>
		public List<ITrade> CloseTrades {
			get {
				return __cCloseTrades;
			}
		}

		/// <summary>
		///   [取得] 開倉交易單列表
		/// </summary>
		public TradeList<ITrade> OpenTrades {
			get {
				return __cOpenTrades;
			}
		}

		/// <summary>
		///   [取得] 回報類型
		/// </summary>
		public ResponseType ResponseType {
			get {
				return __cType;
			}
		}

		/// <summary>
		///   [取得] 商品代號
		/// </summary>
		public string SymbolId {
			get {
				return __sSymbolId;
			}
		}

		/// <summary>
		///   [取得] 交易訂單
		/// </summary>
		public ITradeOrder TradeOrder {
			get {
				return __cTradeOrder;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="tradeOrder">訂單資訊</param>
		/// <param name="symbolId">商品代號</param>
		/// <param name="type">回報類型</param>
		/// <param name="openTrades">開倉交易單列表</param>
		/// <param name="closeTrades">已平倉交易單列表</param>
		public ResponseEvent(ITradeOrder tradeOrder, string symbolId, ResponseType type, TradeList<ITrade> openTrades, List<ITrade> closeTrades) {
			__cTradeOrder = tradeOrder;
			__sSymbolId = symbolId;
			__cType = type;
			__cOpenTrades = openTrades;
			__cCloseTrades = closeTrades;
		}
	}
}