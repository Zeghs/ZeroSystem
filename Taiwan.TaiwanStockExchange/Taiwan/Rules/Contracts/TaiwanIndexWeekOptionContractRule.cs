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
	///   台灣週選擇權合約規則
	/// </summary>
	[RuleProperty(ERuleType.Contract, "TaiwanIndexWeekOptionContractRule", "Taiwan.Rules.Contracts.TaiwanIndexWeekOptionContractRule", "(台灣指數週選擇權合約到期規則)\r\n\r\n台灣指數週選擇權各契約結算日以第三個星期三為最後交易日(如遇國定假日或其他不可抗力因素則以台灣期交所公告為準)。", true)]
	public sealed class TaiwanIndexWeekOptionContractRule : RuleBase, IContractTarget, IContractTime, IRuleSetting {
		private static Dictionary<string, string> __cOptionsTable = new Dictionary<string, string>() {
			{ "TXW", "TXF" }
		};

		private static string GetOptionsTarget(string commodityId) {
			string sTarget = null;
			if (!__cOptionsTable.TryGetValue(commodityId, out sTarget)) {
				sTarget = "TXF";
			}
			return sTarget;
		}

		private int __iCloseHour = 0, __iCloseMinute = 0, __iCloseSecond = 0;
		private List<ContractTime> __cContractTimes = new List<ContractTime>(8);

		public TaiwanIndexWeekOptionContractRule() {
			if (ShowSetting() == -1) {
				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));
			}
		}

		public TaiwanIndexWeekOptionContractRule(JToken args) {
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

		public string GetContractTarget(string symbolId) {
			string sRet = null;
			string sUSymbolId = symbolId.ToUpper();
			if (sUSymbolId.Length > 3) {
				sRet = string.Format("{0}0.tw", GetOptionsTarget(sUSymbolId));
			}
			return sRet;
		}

		public ContractTime GetContractTime(DateTime date, int index = 0) {
			date = new DateTime(date.Year, date.Month, date.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);

			double dMinValue = double.MaxValue;
			int iIndex = -1, iCount = __cContractTimes.Count;
			for (int i = index; i < iCount; i++) {
				ContractTime cContractTemp = __cContractTimes[i];
				double dTotals = (cContractTemp.MaturityDate - date).TotalSeconds;
				if (dTotals >= 0 && dTotals <= 604800) {
					if (dMinValue > dTotals) {
						dMinValue = dTotals;
						iIndex = i;
					}
				}
			}

			if (iIndex == -1) {
				if (index > 0) {
					date = date.AddSeconds(index * 604800);
				}
				return MaturityDateUtil.GetWeekMaturityDate(date);
			}
			return __cContractTimes[iIndex];
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

				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));

				JObject cObject = new JObject();
				cObject.Add(new JProperty("ExpireTime", cCloseTime));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)
				return 0;  //成功
			}
			return -1;  //取消
		}

		public void UpdateContractTime(DateTime date) {
			__cContractTimes.Clear();

			DateTime cDate = new DateTime(date.Year, date.Month, date.Day, __iCloseHour, __iCloseMinute, __iCloseSecond);
			CalcContractTime(cDate);  //計算合約時間
		}

		private void CalcContractTime(DateTime today) {
			//先計算出目前此週到期日
			ContractTime cCurrentContractTime = MaturityDateUtil.GetWeekMaturityDate(today);
			AddContractTime(0, cCurrentContractTime);

			DateTime cDate = cCurrentContractTime.MaturityDate.AddSeconds(604800);
			for (int i = 1; i < 3; i++) {
				ContractTime cContractTime = MaturityDateUtil.GetWeekMaturityDate(cDate);
				AddContractTime(i, cContractTime);

				cDate = cContractTime.MaturityDate.AddSeconds(604800);
			}
		}

		private void AddContractTime(int id, ContractTime contractTime) {
			contractTime.Id = id;
			__cContractTimes.Add(contractTime);
		}
	}
}