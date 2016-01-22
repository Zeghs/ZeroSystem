namespace PowerLanguage {
	/// <summary>
	///   價格單交易介面
	/// </summary>
	public interface IOrderPriced : IOrderObject {
		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		void Send(double price);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		void Send(double price, int numLots);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		void Send(string new_name, double price);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		void Send(string new_name, double price, int numLots);
	}
}