namespace PowerLanguage {
	/// <summary>
	///   座標範圍列舉
	/// </summary>
	public enum EAxisScope {
		/// <summary>
		///   目前資料範圍
		/// </summary>
		CurrentScope = 0,
		
		/// <summary>
		///   全部資料範圍
		/// </summary>
		AllScope = 1,

		/// <summary>
		///   資料變動的範圍
		/// </summary>
		ChangeScope = 2,

		/// <summary>
		///   價格比例範圍(根據商品的價格尺度規則設定座標軸間距)
		/// </summary>
		PriceScaleScope = 3
	}
}