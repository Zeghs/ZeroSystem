﻿using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PowerLanguage;
using Zeghs.Rules;
using Taiwan.Forms;

namespace Taiwan.Rules.Taxs {
	[RuleProperty(ERuleType.Tax, "TaiwanStockFuturesTaxRule", "Taiwan.Rules.Taxs.TaiwanStockFuturesTaxRule", "(台灣股票型期貨交易稅規則)\r\n\r\n台灣股票型期貨交易稅率係由台灣期交所與金管會所制定，其稅率由台灣期貨交易所公告為準。", true)]
	public sealed class TaiwanStockFuturesTaxRule : RuleBase, ITax, IRuleSetting {
		private double __dTaxRate = 0;

		public TaiwanStockFuturesTaxRule() {
			ShowSetting();
		}

		public TaiwanStockFuturesTaxRule(JToken args) {
			this.args = args;
			__dTaxRate = args["TaxRate"].Value<double>();
		}

		public double GetTax(EOrderAction action, double tradeTotals) {
			return tradeTotals * __dTaxRate;
		}

		public int ShowSetting() {
			frmTaiwanTaxSetting cSetting = new frmTaiwanTaxSetting(9, 0.00000001d);
			cSetting.TaxRate = __dTaxRate;
			DialogResult cResult = cSetting.ShowDialog();
			if (cResult == DialogResult.OK) {
				__dTaxRate = cSetting.TaxRate;

				JObject cObject = new JObject();
				cObject.Add(new JProperty("TaxRate", __dTaxRate));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)

				return 0;  //成功
			}
			return -1;  //取消
		}
	}
}