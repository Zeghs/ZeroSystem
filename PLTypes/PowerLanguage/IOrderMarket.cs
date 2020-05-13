namespace PowerLanguage {
	/// <summary>
	///   市價單交易介面
	/// </summary>
	public interface IOrderMarket : IOrderObject {
		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send();

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(int numLots);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name, int numLots);
	}
}