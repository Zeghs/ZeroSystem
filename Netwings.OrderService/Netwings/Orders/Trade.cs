using System.Collections.Generic;
using PowerLanguage;

namespace Netwings.Orders {
	/// <summary>
	///   交易類別
	/// </summary>
	internal sealed class Trade : ITrade {
		/// <summary>
		///   [取得] 交易佣金
		/// </summary>
		public double CommissionValue {
			get {
				double dTotals = _EntryOrder.Fee + _EntryOrder.OtherFees + _EntryOrder.Tax;
				if (_ExitOrder != null) {
					dTotals += _ExitOrder.Fee + _ExitOrder.OtherFees + _ExitOrder.Tax;
				}
				return dTotals;
			}
		}

		/// <summary>
		///   [取得] 開倉訂單資訊
		/// </summary>
		public ITradeOrder EntryOrder {
			get {
				return this._EntryOrder;
			}
		}

		/// <summary>
		///   [取得] 平倉訂單資訊(如果為 null 表示尚未平倉)
		/// </summary>
		public ITradeOrder ExitOrder {
			get {
				return this._ExitOrder;
			}
		}

		/// <summary>
		///   [取得] 是否為多單
		/// </summary>
		public bool IsLong {
			get {
				return this.EntryOrder.Action == EOrderAction.Buy;
			}
		}

		/// <summary>
		///   [取得] 是否為開倉
		/// </summary>
		public bool IsOpen {
			get {
				return this.ExitOrder == null;
			}
		}

		/// <summary>
		///   [取得] 下單名稱
		/// </summary>
		public string Name {
			get {
				return _EntryOrder.Name;
			}
		}

		/// <summary>
		///   [取得] 損益(如果已平倉則會一併計入手續費與交易稅, 如果是開倉的損益不會計入手續費與交易稅)
		/// </summary>
		public double Profit {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] Ticket 編號
		/// </summary>
		public string Ticket {
			get {
				return _EntryOrder.Ticket;
			}
		}

		internal TradeOrder _ExitOrder {
			get;
			set;
		}

		internal TradeOrder _EntryOrder {
			get;
			set;
		}

		internal void CalculateProfit(double bigPointValue) {
			double dDiffPrice = (this.IsLong) ? _ExitOrder.Price - _EntryOrder.Price : _EntryOrder.Price - _ExitOrder.Price;
			this.Profit = (dDiffPrice * _EntryOrder.Contracts) * bigPointValue - this.CommissionValue;
		}

		internal void CalculateProfit(double closePrice, double bigPointValue) {
			double dDiffPrice = (this.IsLong) ? closePrice - _EntryOrder.Price : _EntryOrder.Price - closePrice;
			this.Profit = (dDiffPrice * _EntryOrder.Contracts) * bigPointValue;
		}
	}
}