using System;
using PowerLanguage;

namespace Netwings.Orders {
	/// <summary>
	///   交易訂單資訊類別
	/// </summary>
	internal sealed class TradeOrder : ITradeOrder {
		/// <summary>
		///   [取得] 下單進出場動作
		/// </summary>
		public EOrderAction Action {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單時的 Bars Number
		/// </summary>
		public int BarNumber {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單類型
		/// </summary>
		public OrderCategory Category {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單合約數量
		/// </summary>
		public int Contracts {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 手續費
		/// </summary>
		public double Fee {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單名稱
		/// </summary>
		public string Name {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 其他佣金或手續費用
		/// </summary>
		public double OtherFees {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單價格
		/// </summary>
		public double Price {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 商品代號
		/// </summary>
		public string SymbolId {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 交易稅
		/// </summary>
		public double Tax {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單時間
		/// </summary>
		public DateTime Time {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] Ticket 編號
		/// </summary>
		public string Ticket {
			get;
			internal set;
		}

		internal bool IsCancel {
			get;
			set;
		}

		internal bool IsDealed {
			get;
			set;
		}

		internal bool IsReverse {
			get;
			set;
		}

		internal bool IsTrusted {
			get;
			set;
		}

		internal bool IsSended {
			get;
			set;
		}

		internal TradeOrder Clone() {
			return this.MemberwiseClone() as TradeOrder;
		}
	}
}