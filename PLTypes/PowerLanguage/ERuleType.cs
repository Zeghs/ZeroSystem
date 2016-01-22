namespace PowerLanguage {
	/// <summary>
	///   規則類別
	/// </summary>
	public enum ERuleType {
		/// <summary>
		///   無類型
		/// </summary>
		None = 0,

		/// <summary>
		///   合約(交割)時間規則
		/// </summary>
		Contract = 1,

		/// <summary>
		///   價格座標規則
		/// </summary>
		PriceScale = 2,

		/// <summary>
		///   交易稅規則
		/// </summary>
		Tax = 3,

		/// <summary>
		///   佣金規則
		/// </summary>
		Commission = 128,

		/// <summary>
		///   手續費規則
		/// </summary>
		Fee = 129
	}
}