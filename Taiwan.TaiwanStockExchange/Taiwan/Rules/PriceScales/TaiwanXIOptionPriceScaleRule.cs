using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanXIOptionPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanXIOptionPriceScaleRule", "(台灣非金電指數選擇權價格座標規則)\r\n\r\n台灣指數型非金電選擇權最小跳動點數規則(請參考台灣期貨交易所指數型選擇權契約資訊)。", false)]
	public sealed class TaiwanXIOptionPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 2d };

			if (price < 20) {
				dPriceScales[0] = 0.1d;
			} else if (price < 100 && price >= 20) {
				dPriceScales[0] = 0.5d;
			} else if (price < 1000 && price >= 100) {
				dPriceScales[0] = 1d;
			} else if (price < 2000 && price >= 1000) {
				dPriceScales[0] = 5d;
			} else if (price > 2000) {
				dPriceScales[0] = 10d;
			}
			return dPriceScales;
		}
	}
}