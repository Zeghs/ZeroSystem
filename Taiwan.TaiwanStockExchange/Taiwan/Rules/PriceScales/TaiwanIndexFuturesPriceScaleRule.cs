using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PowerLanguage;
using Zeghs.Rules;
using Taiwan.Forms;

namespace Taiwan.Rules.PriceScales {
	[RuleProperty(ERuleType.PriceScale, "TaiwanIndexFuturesPriceScaleRule", "Taiwan.Rules.PriceScales.TaiwanIndexFuturesPriceScaleRule", "(台灣指數型期貨價格座標規則)\r\n\r\n台灣指數期貨最小跳動點數規則(請參考台灣期貨交易所指數型期貨契約資訊)。", true)]
	public sealed class TaiwanIndexFuturesPriceScaleRule : RuleBase, IPriceScale, IRuleSetting {
		private double[] __dPriceScales = null;

		public TaiwanIndexFuturesPriceScaleRule() {
			if (ShowSetting() == -1) {
				__dPriceScales = new double[] { 1.0d, 1.0d };
			}
		}

		public TaiwanIndexFuturesPriceScaleRule(JToken args) {
			this.args = args;
			__dPriceScales = args["PriceScales"].ToObject<double[]>();
		}
		
		/// <summary>
		///   取得價格座標與最小跳動點數
		/// </summary>
		/// <param name="price">價格</param>
		/// <returns>返回值:double陣列(0=價格座標, 1=最小跳動點數)</returns>
		public double[] GetPriceScale(double price) {
			return __dPriceScales;
		}
		
		public int ShowSetting() {
			frmTaiwanIndexFuturePriceScaleSetting cSetting = new frmTaiwanIndexFuturePriceScaleSetting();
			if (__dPriceScales != null) {
				cSetting.PriceScales = __dPriceScales;
			}
			
			DialogResult cResult = cSetting.ShowDialog();
			if (cResult == DialogResult.OK) {
				__dPriceScales = cSetting.PriceScales;

				JObject cObject = new JObject();
				cObject.Add(new JProperty("PriceScales", __dPriceScales));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)

				return 0;  //成功
			}
			return -1;  //取消
		}
	}
}