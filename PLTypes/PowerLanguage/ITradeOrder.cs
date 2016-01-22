using System;

namespace PowerLanguage {
	/// <summary>
	///   交易訂單資訊介面
	/// </summary>
	public interface ITradeOrder : ITradeTicket {
		/// <summary>
		///   [取得] 下單進出場動作
		/// </summary>
		EOrderAction Action {
			get;
		}

		/// <summary>
		///   [取得] 下單時的 Bars Number
		/// </summary>
		int BarNumber {
			get;
		}

		/// <summary>
		///   [取得] 下單類型
		/// </summary>
		OrderCategory Category {
			get;
		}

		/// <summary>
		///   [取得] 下單合約數量
		/// </summary>
		int Contracts {
			get;
		}

		/// <summary>
		///   [取得] 手續費
		/// </summary>
		double Fee {
			get;
		}

		/// <summary>
		///   [取得] 其他佣金或手續費用
		/// </summary>
		double OtherFees {
			get;
		}

		/// <summary>
		///   [取得] 下單價格
		/// </summary>
		double Price {
			get;
		}

		/// <summary>
		///   [取得] 交易稅
		/// </summary>
		double Tax {
			get;
		}

		/// <summary>
		///   [取得] 下單時間
		/// </summary>
		DateTime Time {
			get;
		}
	}
}