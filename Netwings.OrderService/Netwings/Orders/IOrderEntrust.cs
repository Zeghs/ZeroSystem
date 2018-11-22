using Zeghs.Orders;

namespace Netwings.Orders {
	/// <summary>
	///   委託訂單介面
	/// </summary>
	internal interface IOrderEntrust {
		/// <summary>
		///   [取得] 委託訂單
		/// </summary>
		TradeList<TradeOrder> Entrusts {
			get;
		}
	}
}