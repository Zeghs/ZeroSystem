using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanTFOptionPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanTFOptionPriceScaleRule", "(台灣金融指數選擇權價格座標規則)\r\n\r\n台灣指數型金融選擇權最小跳動點數規則(請參考台灣期貨交易所指數型選擇權契約資訊)。", false)]
	public sealed class TaiwanTFOptionPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 2d };

			if (price < 2) {
				dPriceScales[0] = 0.01d;
			} else if (price < 10 && price >= 2) {
				dPriceScales[0] = 0.05d;
			} else if (price < 100 && price >= 10) {
				dPriceScales[0] = 0.1d;
			} else if (price < 200 && price >= 100) {
				dPriceScales[0] = 0.5d;
			} else if (price > 200) {
				dPriceScales[0] = 1d;
			}
			return dPriceScales;
		}
	}
}