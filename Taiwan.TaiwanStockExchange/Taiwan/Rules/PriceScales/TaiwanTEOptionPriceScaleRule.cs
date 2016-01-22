using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanTEOptionPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanTEOptionPriceScaleRule", "(台灣電子指數選擇權價格座標規則)\r\n\r\n台灣指數型電子選擇權最小跳動點數規則(請參考台灣期貨交易所指數型選擇權契約資訊)。", false)]
	public sealed class TaiwanTEOptionPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 5d };

			if (price < 0.5) {
				dPriceScales[0] = 0.001d;
			} else if (price < 2.5 && price >= 0.5) {
				dPriceScales[0] = 0.005d;
			} else if (price < 25 && price >= 2.5) {
				dPriceScales[0] = 0.01d;
			} else if (price < 50 && price >= 25) {
				dPriceScales[0] = 0.05d;
			} else if (price > 50) {
				dPriceScales[0] = 0.1d;
			}
			return dPriceScales;
		}
	}
}