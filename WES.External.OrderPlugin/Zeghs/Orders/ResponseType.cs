namespace Zeghs.Orders {
	/// <summary>
	///   回報類型
	/// </summary>
	public enum ResponseType {
		/// <summary>
		///   無回報類型
		/// </summary>
		None = 0,

		/// <summary>
		///   委託回報
		/// </summary>
		Trust = 1,
		
		/// <summary>
		///   成交回報
		/// </summary>
		Deal = 2,

		/// <summary>
		///   取消回報
		/// </summary>
		Cancel = 3,

		/// <summary>
		///   資訊更新
		/// </summary>
		Update = 4
	}
}