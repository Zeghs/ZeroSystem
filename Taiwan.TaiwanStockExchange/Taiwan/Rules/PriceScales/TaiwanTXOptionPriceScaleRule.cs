using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanTXOptionPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanTXOptionPriceScaleRule", "(台灣台指選擇權價格座標規則)\r\n\r\n台灣指數型台指選擇權最小跳動點數規則(請參考台灣期貨交易所指數型選擇權契約資訊)。", false)]
	public sealed class TaiwanTXOptionPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 1d };
			
			if (price < 10) {
				dPriceScales[0] = 0.1d;
			} else if (price < 50 && price >= 10) {
				dPriceScales[0] = 0.5d;
			} else if (price < 500 && price >= 50) {
				dPriceScales[0] = 1d;
			} else if (price < 1000 && price >= 500) {
				dPriceScales[0] = 5d;
			} else if (price > 1000) {
				dPriceScales[0] = 10d;
			}
			return dPriceScales;
		}
	}
}