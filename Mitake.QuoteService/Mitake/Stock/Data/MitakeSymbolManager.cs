using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Rules;
using Zeghs.Products;
using Zeghs.Managers;

namespace Mitake.Stock.Data {
	internal sealed class MitakeSymbolManager {
		private static Dictionary<string, int> __cSymbolList = new Dictionary<string, int>(MitakeStorage.BASE_STOCK_CAPACITY);
		private static Dictionary<int, MitakeSymbolInformation> __cMitakeQuoteInformations = new Dictionary<int, MitakeSymbolInformation>(MitakeStorage.BASE_STOCK_CAPACITY);

		private static string __sExchangeName = null;
		private static DateTime __cToday = DateTime.MinValue;
		private static HashSet<string> __cMitakeIndexOptions = null;  //指數選擇權商品源代號表(用來判斷是指數型選擇權還是股票型選擇權)
		private static Dictionary<string, double> __cMitakeOptionDecimalPoints = null;  //選擇權履約價格小數點位數(0.1=小數點一位, 0.01=小數點兩位)

		private static Dictionary<string, int> __cMitakeIndexSerials = new Dictionary<string, int>() {
			{ "otc.tw", 9998 },
			{ "twi.tw", 9999 }
		};

		private static Dictionary<int, Product> __cMitakeIndexInformations = new Dictionary<int, Product>() {
			{ 9998, new Product() { CommodityId ="INDEX_WEIGHT", Category = ESymbolCategory.Index, SymbolId = "OTC.tw", SymbolName = "上櫃指數" }},
			{ 9999, new Product() { CommodityId ="INDEX_WEIGHT", Category = ESymbolCategory.Index, SymbolId = "TWI.tw", SymbolName = "加權指數" }}
		};

		/// <summary>
		///   [取得/設定] 資料來源名稱
		/// </summary>
		internal static string DataSource {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易所簡稱
		/// </summary>
		internal static string ExchangeName {
			get {
				return __sExchangeName;
			}
			
			set {
				__sExchangeName = value;  //填入交易所名稱

				if (__cToday == DateTime.MinValue) {  //檢查是否尚未更新, 如果尚未更新就更新為目前本機時間(請注意本機時間需要準確, 避免有換月換週的商品計算錯誤)
					AbstractExchange cExchange = ProductManager.Manager.GetExchange(__sExchangeName);
					__cToday = DateTime.UtcNow.AddHours(cExchange.TimeZone).Date;
				}
			}
		}

		internal static void AddQuoteSymbolInformation(int serial, MitakeSymbolInformation symbolInformation) {
			string sLSymbolId = symbolInformation.SymbolId.ToLower();
			if (__cSymbolList.ContainsKey(sLSymbolId)) {
				__cSymbolList[sLSymbolId] = serial;
			} else {
				__cSymbolList.Add(sLSymbolId, serial);
			}

			if (__cMitakeQuoteInformations.ContainsKey(serial)) {
				__cMitakeQuoteInformations[serial] = symbolInformation;
			} else {
				__cMitakeQuoteInformations.Add(serial, symbolInformation);
			}
		}

		/// <summary>
		///   轉換Mitake資訊源內部商品代號為標準台股商品代號
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <param name="marketType">市場別  1=期貨, 2=選擇權</param>
		/// <returns>返回值:轉換後的商品代號</returns>
		internal static string Convert(string symbolId, int marketType) {
			int iContractIndex = 0;
			string sSymbolId = null, sCommodityId = null;

			int iLength = symbolId.Length;
			switch (marketType) {
				case 1:  //期貨
					if (iLength == 5) {
						int iFYear = (symbolId[4] - '0');
						int iFMonth = (symbolId[3] - 'A') + 1;
						if (char.IsDigit(symbolId, 2)) {  //判斷是否為週期貨(周期貨都以數字表示第幾周)
							sCommodityId = symbolId.Substring(0, 2) + "W";  //將週期貨統一更名 例: MX1 => MXW
						} else {
							sCommodityId = symbolId.Substring(0, 3);
						}
						
						iContractIndex = GetContractTimeIndex(sCommodityId, iFYear, iFMonth);
						if (iContractIndex > -1) {
							sSymbolId = string.Format("{0}{1}.tw", sCommodityId, iContractIndex);
						}
					}
					break;
				case 2:  //選擇權
					if (iLength == 10) {
						char chCallOrPut = ((symbolId[8] < 'M') ? 'C' : 'P');
						int iOWeek = 0;
						int iOYear = (symbolId[9] - '0');
						int iOMonth = (symbolId[8] - ((chCallOrPut == 'C') ? 'A' : 'M')) + 1;
						if (char.IsDigit(symbolId, 2)) {  //判斷是否為週選擇權(周選擇權都以數字表示第幾周)
							iOWeek = symbolId[2] - '0';
							sCommodityId = symbolId.Substring(0, 2) + "W";  //將週選擇權統一更名 例: TX1 => TXW
						} else {
							sCommodityId = symbolId.Substring(0, 3);
						}
						
						iContractIndex = GetContractTimeIndex(sCommodityId, iOYear, iOMonth, iOWeek);
						if (iContractIndex > -1) {
							double dDecimalPoint = 0, dValue = double.Parse(symbolId.Substring(3, 5));
							if (__cMitakeOptionDecimalPoints.TryGetValue(sCommodityId, out dDecimalPoint)) {
								dValue *= dDecimalPoint;  //如果有小數點就調整履約價格數值(三竹資料的履約價格沒有小數點總共五位數字, 需要自己轉換小數位)
							}
							
							sSymbolId = string.Format("{0}{1}{2}{3}.tw", sCommodityId, iContractIndex, chCallOrPut, dValue);
						}
					}
					break;
			}

			if (sSymbolId == null) {
				sSymbolId = string.Format("{0}.tw", symbolId);
			}
			return sSymbolId;
		}

		/// <summary>
		///   轉換商品代號成Mitake資訊源的內部商品序號
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:轉換後的商品序號</returns>
		internal static int ConvertToSerial(string symbolId) {
			int iSerial = -1;
			
			symbolId = symbolId.ToLower();
			if (!__cSymbolList.TryGetValue(symbolId, out iSerial)) {  //檢查股票對照表內是否有商品序號
				__cMitakeIndexSerials.TryGetValue(symbolId, out iSerial);  //檢查指數對照表內是否有商品序號
			}
			return iSerial;
		}

		internal static void Deserialize(string[] settings) {
			__cMitakeIndexOptions = JsonConvert.DeserializeObject<HashSet<string>>(settings[1]);
			__cMitakeOptionDecimalPoints = JsonConvert.DeserializeObject<Dictionary<string, double>>(settings[2]);
			__cMitakeQuoteInformations = JsonConvert.DeserializeObject<Dictionary<int, MitakeSymbolInformation>>(settings[3]);

			foreach (int iSerial in __cMitakeQuoteInformations.Keys) {
				MitakeSymbolInformation cInformation = __cMitakeQuoteInformations[iSerial];

				string sLSymbolId = cInformation.SymbolId.ToLower();
				if (!__cSymbolList.ContainsKey(sLSymbolId)) {
					__cSymbolList.Add(sLSymbolId, iSerial);
				}
			}
		}

		internal static Product GetIndexSymbolInformation(int serial) {
			Product cProduct = null;
			__cMitakeIndexInformations.TryGetValue(serial, out cProduct);
			return cProduct;
		}

		internal static MitakeSymbolInformation GetQuoteSymbolInformation(int serial) {
			MitakeSymbolInformation cSymbolInfo = null;
			__cMitakeQuoteInformations.TryGetValue(serial, out cSymbolInfo);
			return cSymbolInfo;
		}

		internal static bool IsExist(string symbolId) {
			string sSymbolId = symbolId.ToLower();
			return __cSymbolList.ContainsKey(sSymbolId);
		}

		internal static bool IsIndexOption(string commodityId) {
			return __cMitakeIndexOptions.Contains(commodityId);
		}

		internal static string Serialize() {
			string sSetting1 = JsonConvert.SerializeObject(__cMitakeIndexOptions);
			string sSetting2 = JsonConvert.SerializeObject(__cMitakeOptionDecimalPoints);
			string sSetting3 = JsonConvert.SerializeObject(__cMitakeQuoteInformations);
			
			StringBuilder cBuilder = new StringBuilder(sSetting1.Length + sSetting2.Length + sSetting3.Length);
			cBuilder.AppendLine(sSetting1);
			cBuilder.AppendLine(sSetting2);
			cBuilder.AppendLine(sSetting3);
			
			return cBuilder.ToString();
		}

		internal static void Update(DateTime date, bool isReBuild = true) {
			__cToday = date.Date;
			__cSymbolList.Clear();
			__cMitakeQuoteInformations.Clear();

			AbstractExchange cExchange = ProductManager.Manager.GetExchange(__sExchangeName);
			cExchange.Update(date); //更新交易所商品屬性資訊

			if (isReBuild) { //是否要重新建立股票代號表資訊
				cExchange.UpdateTime = DateTime.Today.AddDays(-2);  //設定為兩天前(強迫更新)
				cExchange.Clear();  //如果要更新則全部清除
				
				//新增上櫃指數(Mitake股票清單內沒有這支指數需要自行新增)
				cExchange.AddProduct(MitakeSymbolManager.GetIndexSymbolInformation(9998));

				//新增上市指數(Mitake股票清單內沒有這支指數需要自行新增)
				cExchange.AddProduct(MitakeSymbolManager.GetIndexSymbolInformation(9999));
			}
		}

		private static int GetContractTimeIndex(string commodityId, int year, int month, int week = 0) {
			int iRet = -1;
			
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(__sExchangeName);
			AbstractProductProperty cProperty = cExchange.GetProperty(commodityId, DataSource);
			if (cProperty != null) {
				int iYear = __cToday.Year;
				iYear = (iYear - (iYear % 10)) + year;
				int iDay = __cToday.Day;
				int iDays = DateTime.DaysInMonth(iYear, month);  //取得該月份的最大天數
			
				if (commodityId[2] == 'W') {  //如果是週商品(期權)
					int iFirstWeek = (int) new DateTime(iYear, month, 1).DayOfWeek;
					int iFirstWedDay = (7 - iFirstWeek > 3) ? 4 - iFirstWeek : 11 - iFirstWeek;
					iDay = iFirstWedDay + (week - 1) * 7;
					if (iDay > iDays) {
					        iDay = iDays;
					}
				} else {
					iDay = ((iDay > iDays) ? iDays : iDay);  //如果傳進來的月份為商品代號上的月份代碼轉換的, 就必須注意是否超過此月份的最大天數, 如果超過就需要修正
				}

				IContractTime cRule = cProperty.ContractRule as IContractTime;
				if (cRule != null) {
					ContractTime cContractTimes = cRule.GetContractTime(new DateTime(iYear, month, iDay));
					iRet = cContractTimes.Id;
				}
			}
			return iRet;
		}
	}
}  //247行