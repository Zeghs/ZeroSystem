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
	///   台灣期貨合約規則
	/// </summary>
	[RuleProperty(ERuleType.Contract, "TaiwanIndexFuturesContractRule", "Taiwan.Rules.Contracts.TaiwanIndexFuturesContractRule", "(台灣指數期貨合約到期規則)\r\n\r\n台灣指數期貨各契約結算月份以第三個星期三為最後交易日(如遇國定假日或其他不可抗力因素則以台灣期交所公告為準)。", true)]
	public sealed class TaiwanIndexFuturesContractRule : RuleBase, IContractTime, IRuleSetting {
		private static int CalcBaseMonth(int month) {
			int iBaseMonth = month + 1;
			for (int i = iBaseMonth; i < 16; i++) {
				if (i > iBaseMonth && i % 3 == 0) {
					iBaseMonth = i;
					break;
				}
			}
			return iBaseMonth;
		}

		private int __iCloseHour = 0, __iCloseMinute = 0, __iCloseSecond = 0;
		private Dictionary<int, int> __cIndexs = new Dictionary<int, int>(8);
		private List<ContractTime> __cContractTimes = new List<ContractTime>(8);

		public TaiwanIndexFuturesContractRule() {
			if (ShowSetting() == -1) {
				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));
			}
		}

		public TaiwanIndexFuturesContractRule(JToken args) {
			this.args = args;
			TimeSpan cContractCloseTime = args["ExpireTime"].ToObject<TimeSpan>();

			//算出時分秒
			__iCloseHour = cContractCloseTime.Hours;
			__iCloseMinute = cContractCloseTime.Minutes;
			__iCloseSecond = cContractCloseTime.Seconds;

			DateTime cToday = DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE);
			cToday = new DateTime(cToday.Year, cToday.Month, cToday.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);
			CalcContractTime(cToday);  //計算合約時間
		}

		public int GetContractIndex(string symbolId) {
			int iIndex = 0, iLength = symbolId.Length;
			for (int i = 3; i < iLength; i++) {
				char chValue = symbolId[i];
				if (char.IsDigit(chValue)) {
					iIndex *= 10;
					iIndex += (chValue - '0');
				} else {
					break;
				}
			}
			return iIndex;
		}

		public ContractTime GetContractTime(DateTime date, int index = 0) {
			ContractTime cContractTime = null;

			int iYear = date.Year, iMonth = date.Month;
			int iIndex = 0, iKey = iYear * 100 + iMonth;
			if (__cIndexs.TryGetValue(iKey, out iIndex)) {
				cContractTime = __cContractTimes[(index > 0) ? index : iIndex];
			} else {
				date = new DateTime(iYear, iMonth, date.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);
				cContractTime = MaturityDateUtil.GetMonthMaturityDate(date, true);

				if (index > 0) {
					int iCMonth = cContractTime.ContractMonth;
					int iBaseMonth = CalcBaseMonth(iMonth);
					int iMonthPeriod = (index > 1) ? (iBaseMonth + (3 * (index - 2))) - iCMonth : 1;
					cContractTime = AddContractMonths(cContractTime, iMonthPeriod);
				}
			}
			return cContractTime;
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

				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));  //更新

				JObject cObject = new JObject();
				cObject.Add(new JProperty("ExpireTime", cCloseTime));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)

				return 0;  //成功
			}
			return -1;  //取消
		}

		public void UpdateContractTime(DateTime date) {
			__cIndexs.Clear();
			__cContractTimes.Clear();

			DateTime cDate = new DateTime(date.Year, date.Month, date.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);
			CalcContractTime(cDate);  //計算合約時間
		}

		private void CalcContractTime(DateTime today) {
			//先計算出現月
			ContractTime cCurrentContractTime = MaturityDateUtil.GetMonthMaturityDate(today, true);
			AddContractTime(0, cCurrentContractTime);

			//由現月計算出其他月份
			int iMonth = cCurrentContractTime.ContractMonth;
			int iBaseMonth = CalcBaseMonth(iMonth);

			int[] iMonthPeriods = new int[5];
			iMonthPeriods[0] = 1;
			for (int i = 0; i < 4; i++) {
				iMonthPeriods[i + 1] = (iBaseMonth + (3 * i)) - iMonth;
			}

			//計算其他月份的交易日與到期日
			int iLength = iMonthPeriods.Length;
			for (int i = 0; i < iLength; i++) {
				ContractTime cContractTime = AddContractMonths(cCurrentContractTime, iMonthPeriods[i]);
				AddContractTime(i + 1, cContractTime);
			}
		}

		private ContractTime AddContractMonths(ContractTime contract, int month) {
			int iYear = contract.ContractYear;
			int iMonth = contract.ContractMonth + month;
			if (iMonth > 12) {
				--iMonth;
				iMonth %= 12;
				++iMonth;
				++iYear;
			}
			return MaturityDateUtil.GetMonthMaturityDate(new DateTime(iYear, iMonth, 1, __iCloseHour, __iCloseMinute, __iCloseSecond), false);
		}

		private void AddContractTime(int id, ContractTime contractTime) {
			int iKey = contractTime.ContractYear * 100 + contractTime.ContractMonth;
			if (!__cIndexs.ContainsKey(iKey)) {
				contractTime.Id = id;
				
				__cIndexs.Add(iKey, id);
				__cContractTimes.Add(contractTime);
			}
		}
	}
}