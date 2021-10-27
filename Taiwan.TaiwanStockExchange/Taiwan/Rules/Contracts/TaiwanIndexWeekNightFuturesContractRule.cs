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
	///   台灣夜盤週期貨合約規則
	/// </summary>
	[RuleProperty(ERuleType.Contract, "TaiwanIndexWeekNightFuturesContractRule", "Taiwan.Rules.Contracts.TaiwanIndexWeekNightFuturesContractRule", "(台灣指數夜盤週小台期貨合約到期規則)\r\n\r\n台灣指數夜盤週小台期貨各契約結算月份以星期三為最後交易日(如遇國定假日或其他不可抗力因素則以台灣期交所公告為準)。", true)]
	public sealed class TaiwanIndexWeekNightFuturesContractRule : RuleBase, IContractTime, IRuleSetting {
		private TimeSpan __cCloseTime;
		private List<ContractTime> __cContractTimes = new List<ContractTime>(8);

		public TaiwanIndexWeekNightFuturesContractRule() {
			if (ShowSetting() == -1) {
				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));
			}
		}

		public TaiwanIndexWeekNightFuturesContractRule(JToken args) {
			this.args = args;
			__cCloseTime = args["ExpireTime"].ToObject<TimeSpan>();

			this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));
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
			date = new DateTime(date.Year, date.Month, date.Day, __cCloseTime.Hours, __cCloseTime.Minutes, __cCloseTime.Seconds);

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
			cSetting.CloseTime = __cCloseTime;
			DialogResult cResult = cSetting.ShowDialog();
			if (cResult == DialogResult.OK) {
				__cCloseTime = cSetting.CloseTime;
				this.UpdateContractTime(DateTime.UtcNow.AddHours(TaiwanStockExchange.TIME_ZONE));  //更新

				JObject cObject = new JObject();
				cObject.Add(new JProperty("ExpireTime", __cCloseTime));
				this.args = cObject;  //將參數設定至屬性上(方便寫入至設定檔內)

				return 0;  //成功
			}
			return -1;  //取消
		}

		public void UpdateContractTime(DateTime date) {
			__cContractTimes.Clear();

			ContractTime cContractTime = MaturityDateUtil.GetWeekMaturityDate(date);
			if (date.Date == cContractTime.MaturityDate.Date) {
				date = new DateTime(date.Year, date.Month, date.Day, __cCloseTime.Hours, __cCloseTime.Minutes, __cCloseTime.Seconds);
			} else {
				date = date.Date.AddSeconds(__cCloseTime.TotalSeconds);
			}
			CalcContractTime(date);  //計算合約時間
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