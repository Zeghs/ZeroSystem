using PowerLanguage;

namespace Zeghs.Rules {
	/// <summary>
	///   交易稅率規則介面
	/// </summary>
	public interface ITax {
		/// <summary>
		///   取得交易稅金額
		/// </summary>
		/// <param name="action">下單進出場動作列舉</param>
		/// <param name="tradeTotals">交易總金額</param>
		/// <returns>返回值:交易稅金</returns>
		double GetTax(EOrderAction action, double tradeTotals);
	}
}