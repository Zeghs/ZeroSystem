namespace PowerLanguage {
	/// <summary>
	///   商品分類
	/// </summary>
	public enum ESymbolCategory {
		/// <summary>
		///   期貨
		/// </summary>
		Future = 0,

		/// <summary>
		///   期貨選擇權
		/// </summary>
		FutureOption = 1,

		/// <summary>
		///   股票
		/// </summary>
		Stock = 2,

		/// <summary>
		///   股票選擇權
		/// </summary>
		StockOption = 3,

		/// <summary>
		///   指數
		/// </summary>
		Index = 4,

		/// <summary>
		///   貨幣選擇權
		/// </summary>
		CurrOption = 5,

		/// <summary>
		///   指數選擇權
		/// </summary>
		IndexOption = 8,

		/// <summary>
		///   現貨選擇權
		/// </summary>
		Cash = 9,

		/// <summary>
		///   債券
		/// </summary>
		Bond = 10,

		/// <summary>
		///   點差交易
		/// </summary>
		Spread = 11,

		/// <summary>
		///   外匯
		/// </summary>
		Forex = 12,
		
		/// <summary>
		///   期貨展期
		/// </summary>
		FutureRolover = 18,

		/// <summary>
		///   其他商品
		/// </summary>
		Commodity = 19,

		/// <summary>
		///   權證
		/// </summary>
		Warrant = 32,

		/// <summary>
		///   無分類
		/// </summary>
		None = 254
	}
}