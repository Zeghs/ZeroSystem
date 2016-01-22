using PowerLanguage;

namespace Zeghs.Orders {
	/// <summary>
	///   交易佣金介面
	/// </summary>
	public interface ICommission {
		/// <summary>
		///   [取得] 交易佣金類型
		/// </summary>
		ERuleType RuleType {
			get;
		}

		/// <summary>
		///   計算佣金
		/// </summary>
		/// <param name="price">成交價格</param>
		/// <param name="volume">成交數量</param>
		/// <returns>返回值: 佣金</returns>
		double Calculate(double price, int volume);
	}
}