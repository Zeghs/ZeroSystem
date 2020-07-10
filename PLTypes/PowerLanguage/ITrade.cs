namespace PowerLanguage {
	/// <summary>
	///   交易介面
	/// </summary>
	public interface ITrade : ITradeTicket {
		/// <summary>
		///   [取得] 交易佣金
		/// </summary>
		double CommissionValue {
			get;
		}

		/// <summary>
		///   [取得] 開倉訂單資訊
		/// </summary>
		ITradeOrder EntryOrder {
			get;
		}

		/// <summary>
		///   [取得] 平倉訂單資訊(如果為 null 表示尚未平倉)
		/// </summary>
		ITradeOrder ExitOrder {
			get;
		}

		/// <summary>
		///   [取得] 是否為多單
		/// </summary>
		bool IsLong {
			get;
		}

		/// <summary>
		///   [取得] 是否為開倉
		/// </summary>
		bool IsOpen {
			get;
		}

		/// <summary>
		///   [取得] 是否已經交易結束(表示此開倉訂單已全部平倉完畢)
		/// </summary>
		bool IsTradeClosed {
			get;
		}

		/// <summary>
		///   [取得] 損益
		/// </summary>
		double Profit {
			get;
		}
	}
}