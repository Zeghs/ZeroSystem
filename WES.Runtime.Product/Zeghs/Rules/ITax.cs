namespace Zeghs.Rules {
	/// <summary>
	///   交易稅率規則介面
	/// </summary>
	public interface ITax {
		/// <summary>
		///   取得交易稅金額
		/// </summary>
		/// <param name="tradeTotals">交易總金額</param>
		/// <returns>返回值:交易稅金</returns>
		double GetTax(double tradeTotals);
	}
}