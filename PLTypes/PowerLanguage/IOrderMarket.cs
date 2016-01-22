namespace PowerLanguage {
	/// <summary>
	///   市價單交易介面
	/// </summary>
	public interface IOrderMarket : IOrderObject {
		/// <summary>
		///   送出下單命令
		/// </summary>
		void Send();

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="numLots">下單數量</param>
		void Send(int numLots);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		void Send(string new_name);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="numLots">下單數量</param>
		void Send(string new_name, int numLots);
	}
}