using PowerLanguage;
using Zeghs.Rules;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanStockFuturesPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanStockFuturesPriceScaleRule", "(台灣股票期貨價格座標規則)\r\n\r\n台灣股票期貨最小跳動點數規則(請參考台灣期貨交易所期貨契約資訊)。", false)]
	public sealed class TaiwanStockFuturesPriceScaleRule : RuleBase, IPriceScale {
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			double[] dPriceScales = new double[] { 0, 1d };

			if (price < 10) {
				dPriceScales[0] = 0.01d;
			} else if (price < 50 && price >= 10) {
				dPriceScales[0] = 0.05d;
			} else if (price < 100 && price >= 50) {
				dPriceScales[0] = 0.1d;
			} else if (price < 500 && price >= 100) {
				dPriceScales[0] = 0.5d;
			} else if (price < 1000 && price >= 500) {
				dPriceScales[0] = 1d;
			} else if (price > 1000) {
				dPriceScales[0] = 5d;
			}
			return dPriceScales;
		}
	}
}