namespace PowerLanguage {
	/// <summary>
	///   下單進出場動作列舉
	/// </summary>
	public enum EOrderAction {
		/// <summary>
		///   多單進場
		/// </summary>
		Buy = 0,

		/// <summary>
		///   多單出場
		/// </summary>
		Sell = 1,

		/// <summary>
		///   空單進場
		/// </summary>
		SellShort = 2,

		/// <summary>
		///   空單出場
		/// </summary>
		BuyToCover = 3
	}
}