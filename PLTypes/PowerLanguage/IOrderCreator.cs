namespace PowerLanguage {
	/// <summary>
	///   下單建立者介面
	/// </summary>
	public interface IOrderCreator {
		/// <summary>
		///   建立限價單交易
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced 介面</returns>
		IOrderPriced Limit(SOrderParameters orderParams);

		/// <summary>
		///   建立市場單交易(市場單會在下一根 Bars 下單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket 介面</returns>
		IOrderMarket MarketNextBar(SOrderParameters orderParams);

		/// <summary>
		///   建立市場單交易(市場單會在這一根 Bars 下單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket 介面</returns>
		IOrderMarket MarketThisBar(SOrderParameters orderParams);

		/// <summary>
		///   建立停損單交易
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced 介面</returns>
		IOrderPriced Stop(SOrderParameters orderParams);

		/// <summary>
		///   建立停損限價單交易
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderStopLimit 介面</returns>
		IOrderStopLimit StopLimit(SOrderParameters orderParams);
	}
}