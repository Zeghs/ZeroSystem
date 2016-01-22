namespace Zeghs.Rules {
	/// <summary>
	///   價格縮放規則
	/// </summary>
	public interface IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		double[] GetPriceScale(double price);
	}
}