using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanIndexWeightPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanIndexWeightPriceScaleRule", "(台灣加權型指數價格座標規則)\r\n\r\n台灣加權型指數最小跳動點數規則，因加權型指數趨近至小數點兩位，所以適用於加權或上櫃兩指數或其他加權型商品。", false)]
	public sealed class TaiwanIndexWeightPriceScaleRule : RuleBase, IPriceScale {
		private static double[] __dPriceScales = new double[] { 0.01d, 1d };

		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			return __dPriceScales;
		}
	}
}