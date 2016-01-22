using System;
using PowerLanguage;

namespace Zeghs.Data {
	internal sealed class _TradeInfo {
		private double __dProfit = 0;
		private string __sSymbolId = null;
		private ITrade __cTrade = null;
		private ITradeOrder __cOrder = null;

		public string Action {
			get {
				return __cOrder.Action.ToString();
			}
		}

		public string Category {
			get {
				return __cOrder.Category.ToString();
			}
		}

		public string Comment {
			get {
				return __cOrder.Name;
			}
		}

		public int Contracts {
			get {
				return __cOrder.Contracts;
			}
		}

		public double Fee {
			get {
				return Math.Round(__cOrder.Fee, 2);
			}
		}

		public double Price {
			get {
				return __cOrder.Price;
			}
		}

		public double Profit {
			get {
				return (__cTrade == null) ? __dProfit : Math.Round(__cTrade.Profit, 2);
			}
		}

		public string SymbolId {
			get {
				return __sSymbolId;
			}
		}

		public double Tax {
			get {
				return Math.Round(__cOrder.Tax, 2);
			}
		}

		public string Ticket {
			get {
				return __cOrder.Ticket;
			}
		}

		public string Time {
			get {
				return __cOrder.Time.ToString("yyyy-MM-dd HH:mm:ss");
			}
		}

		internal _TradeInfo(ITrade trade, string symbolId) {
			__cTrade = trade;
			__cOrder = trade.EntryOrder;
			__sSymbolId = symbolId;
		}

		internal _TradeInfo(ITradeOrder order, string symbolId, double profit = 0) {
			__cOrder = order;
			__sSymbolId = symbolId;
			__dProfit = Math.Round(profit, 2);
		}
	}
}