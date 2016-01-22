using System;
using Zeghs.Products;

namespace Taiwan.Rules.Contracts {
	/// <summary>
	///   計算到期日工具
	/// </summary>
	internal sealed class MaturityDateUtil {
		/// <summary>
		///   取得合約到期日資訊(適用於月選擇權與期貨商品)
		/// </summary>
		/// <param name="date">日期</param>
		/// <param name="isAutoNextMonth">是否自動換月(如果不自動換月,不會超過本月)</param>
		/// <returns>返回值: 合約到期日資訊</returns>
		internal static ContractTime GetMonthMaturityDate(DateTime date, bool isAutoNextMonth) {
			int iYear = date.Year;
			int iMonth = date.Month;
			ContractTime cContractTime = new ContractTime();

			for (int i = 15; i <= 21; i++) {
				DateTime cDate = new DateTime(iYear, iMonth, i, date.Hour, date.Minute, date.Second);
				if (cDate.DayOfWeek == DayOfWeek.Wednesday) {
					if (isAutoNextMonth && date > cDate) {
						i = 14;
						++iMonth;
						if (iMonth > 12) {
							iMonth = 1;
							++iYear;
						}
						continue;
					}

					cContractTime.ContractYear = iYear;
					cContractTime.ContractMonth = iMonth;
					cContractTime.MaturityDate = cDate;
					break;
				}
			}
			return cContractTime;
		}

		/// <summary>
		///   取得週合約到期日資訊(適用於周選擇權商品)
		/// </summary>
		/// <param name="date">日期</param>
		/// <returns>返回值: 合約到期日資訊</returns>
		internal static ContractTime GetWeekMaturityDate(DateTime date) {
			ContractTime cContractTime = new ContractTime();
			DateTime cMaturity = GetMonthMaturityDate(date, false).MaturityDate;

			for (int i = 0; i < 7; i++) {
				if (date.DayOfWeek == DayOfWeek.Wednesday) {
					double dTotals = (cMaturity - date).TotalSeconds;  //計算是否在第三個星期三周期內(第三個星期三沒有週選擇權)
					if (dTotals >= 0 && dTotals <= 518400) {  //要扣掉結算日 604800 - 86400 = 518400(星期四才是新的換周日)
						date = cMaturity.AddSeconds(604800);  //一次加上一星期的秒數
					}

					cContractTime.ContractYear = date.Year;
					cContractTime.ContractMonth = date.Month;
					cContractTime.MaturityDate = date;
					break;
				}
				date = date.AddSeconds(86400);  //一次加上一天 86400 秒
			}
			return cContractTime;
		}
	}
}