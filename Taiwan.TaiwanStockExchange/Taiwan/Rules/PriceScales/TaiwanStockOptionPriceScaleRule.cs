using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanStockOptionPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanStockOptionPriceScaleRule", "(台灣股票型選擇權價格座標規則)\r\n\r\n台灣股票型選擇權最小跳動點數規則(請參考台灣期貨交易所股票型選擇權契約資訊)。", false)]
	public sealed class TaiwanStockOptionPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 1d };

			if (price < 5) {
				dPriceScales[0] = 0.01d;
			} else if (price < 15 && price >= 5) {
				dPriceScales[0] = 0.05d;
			} else if (price < 50 && price >= 15) {
				dPriceScales[0] = 0.1d;
			} else if (price < 150 && price >= 50) {
				dPriceScales[0] = 0.5d;
			} else if (price < 1000 && price >= 150) {
				dPriceScales[0] = 1d;
			} else if (price > 1000) {
				dPriceScales[0] = 5d;
			}
			return dPriceScales;
		}
	}
}