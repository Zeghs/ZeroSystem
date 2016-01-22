namespace PowerLanguage {
	/// <summary>
	///   交易編號介面
	/// </summary>
	public interface ITradeTicket {
		/// <summary>
		///   [取得] 下單名稱
		/// </summary>
		string Name {
			get;
		}

		/// <summary>
		///   [取得] Ticket 編號
		/// </summary>
		string Ticket {
			get;
		}
	}
}