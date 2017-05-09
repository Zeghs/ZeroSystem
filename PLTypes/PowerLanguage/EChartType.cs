namespace PowerLanguage {
	/// <summary>
	///   圖表類型列舉
	/// </summary>
	public enum EChartType {
		/// <summary>
		///   點型圖表
		/// </summary>
		Point = 0,
		
		/// <summary>
		///   線型圖表
		/// </summary>
		Line = 1,
		
		/// <summary>
		///   長條圖表
		/// </summary>
		Bar = 2,
		
		/// <summary>
		///   美國線圖表
		/// </summary>
		OHLC = 3,
		
		/// <summary>
		///   HLC線圖表
		/// </summary>
		HLC = 4,
		
		/// <summary>
		///   蠟燭線圖表
		/// </summary>
		Candlestick = 5,
		
		/// <summary>
		///   收盤線圖表
		/// </summary>
		ClosingLine = 6,

		/// <summary>
		///   文字物件
		/// </summary>
		TextObject = 128,

		/// <summary>
		///   交易物件
		/// </summary>
		TradeObject = 129
	}
}