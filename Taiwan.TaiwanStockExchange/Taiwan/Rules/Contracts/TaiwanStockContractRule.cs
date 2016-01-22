using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PowerLanguage;
using Zeghs.Rules;
using Zeghs.Products;
using Taiwan.Forms;

namespace Taiwan.Rules.Contracts {
	/// <summary>
	///   台灣股票交割規則(T+2)
	/// </summary>
	[RuleProperty(ERuleType.Contract, "TaiwanStockContractRule", "Taiwan.Rules.Contracts.TaiwanStockContractRule", "(台灣股票交割到期日規則)\r\n\r\n台灣證券各契約交割到期日為交割當天後兩天營業日為最後交割到期日，如未交割則視為違約交割(詳情請見台灣證券交易所之交割規則)。", true)]
	public sealed class TaiwanStockContractRule : RuleBase, IContractTime, IContractParameters, IRuleSetting {
		private const int TRADE_LIMIT_DAY = 2;
		private HashSet<DayOfWeek> __cTradeWeeks = null;
		private int __iCloseHour = 0, __iCloseMinute = 0, __iCloseSecond = 0;

		public TaiwanStockContractRule() {
			ShowSetting();
		}

		public TaiwanStockContractRule(JToken args) {
			this.args = args;
			TimeSpan cContractCloseTime = args["ExpireTime"].ToObject<TimeSpan>();

			//算出時分秒
			__iCloseHour = cContractCloseTime.Hours;
			__iCloseMinute = cContractCloseTime.Minutes;
			__iCloseSecond = cContractCloseTime.Seconds;
		}

		public int GetContractIndex(string symbolId) {
			return 0;
		}

		public ContractTime GetContractTime(DateTime date, int index = 0) {
			return CalcMaturityDate(date);
		}

		public void SetParameters(List<SessionObject> sessions) {
			__cTradeWeeks = new HashSet<DayOfWeek>();
			
			foreach (SessionObject cSession in sessions) {
				__cTradeWeeks.Add(cSession.StartDay);
			}
		}

		public int ShowSetting() {
			frmTaiwanContractSetting cSetting = new frmTaiwanContractSetting();
			cSetting.CloseTime = new TimeSpan(__iCloseHour, __iCloseMinute, __iCloseSecond);
			DialogResult cResult = cSetting.ShowDialog();
			if (cResult == DialogResult.OK) {
				TimeSpan cCloseTime = cSetting.CloseTime;
				__iCloseHour = cCloseTime.Hours;
				__iCloseMinute = cCloseTime.Minutes;
				__iCloseSecond = cCloseTime.Seconds;

				JObject cObject = new JObject();
				cObject.Add(new JProperty("ExpireTime", cCloseTime));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)

				return 0;  //成功
			}
			return -1;  //取消
		}

		public void UpdateContractTime(DateTime date) {
			//股票不需要更新合約時間
		}

		private ContractTime CalcMaturityDate(DateTime date) {
			int iYear = date.Year;
			int iMonth = date.Month;
			
			ContractTime cContractTime = new ContractTime();
			cContractTime.ContractYear = iYear;
			cContractTime.ContractMonth = iMonth;

			DateTime cDate = new DateTime(iYear, iMonth, date.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);
			for (int i = 0; i < TRADE_LIMIT_DAY; i++) {
				cDate = cDate.AddSeconds(86400);  //一次增加一天
				if (__cTradeWeeks != null && !__cTradeWeeks.Contains(cDate.DayOfWeek)) {
					--i;
					continue;
				}
			}
			cContractTime.MaturityDate = cDate;

			return cContractTime;
		}
	}
}