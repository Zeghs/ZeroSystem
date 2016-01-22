using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Rules;
using Zeghs.Managers;

namespace Zeghs.Products {
	/// <summary>
	///   商品設定資訊
	/// </summary>
	public sealed class InstrumentSettings : IInstrumentSettings {
		private double __dTimeZone = 0;
		private double __dMinValue = 0;
		private double __dPriceScale = 0;
		private double __dStrikePrice = 0;
		private int __iContractIndex = 0;
		private Resolution __cResolution;
		private Product __cProduct = null;
		private string __sSymbolId = null;
		private string __sExchange = null;
		private string __sDataSource = null;
		private AbstractProductProperty __cProperty = null;
		private OptionType __cOptionType = OptionType.None;
		private DateTime __cExpiration = DateTime.MinValue;

		/// <summary>
		///   [取得] 商品資訊(為了相容 PowerLanguage 但內部結構並不相同)
		/// </summary>
		public Product ASymbolInfo2 {
			get {
				return __cProduct;
			}
		}

		/// <summary>
		///   [取得] 每一大點的金額
		/// </summary>
		public double BigPointValue {
			get {
				return __cProperty.BigPointValue;
			}
		}

		/// <summary>
		///   [取得] 商品分類列舉
		/// </summary>
		public ESymbolCategory Category {
			get {
				return __cProduct.Category;
			}
		}

		/// <summary>
		///   [取得] 當日漲跌幅限制
		/// </summary>
		public double DailyLimit {
			get {
				return __cProperty.DailyLimit;
			}
		}

		/// <summary>
		///   [取得] 資料來源名稱
		/// </summary>
		public string DataFeed {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   [取得] 商品資訊備註
		/// </summary>
		public string Description {
			get {
				return __cProperty.Description;
			}
		}

		/// <summary>
		///   [取得] 交易所簡稱
		/// </summary>
		public string Exchange {
			get {
				return __sExchange;
			}
		}

		/// <summary>
		///   [取得] 到期日(如果是期貨與選擇權為到期日, 如果是股票則是交割時間截止日期)
		/// </summary>
		public DateTime Expiration {
			get {
				return __cExpiration;
			}
		}

		/// <summary>
		///   [取得] 期貨或選擇權使用之保證金
		/// </summary>
		public double Margin {
			get {
				int iCount = __cProperty.CautionMoneys.Count;
				return (iCount == 0) ? 0 : __cProperty.CautionMoneys[0].InitialMoney;
			}
		}

		/// <summary>
		///   [取得] 最小跳動點數
		/// </summary>
		public double MinMove {
			get {
				return __dMinValue;
			}
		}

		/// <summary>
		///   [取得] 商品名稱
		/// </summary>
		public string Name {
			get {
				return __cProduct.SymbolName;
			}
		}

		/// <summary>
		///   [取得] 選擇權的買賣權類型(None=非選擇權類型, 可能不是選擇權)
		/// </summary>
		public OptionType OptionType {
			get {
				return __cOptionType;
			}
		}

		/// <summary>
		///   [取得] 最小跳動金額
		/// </summary>
		public double PointValue {
			get {
				double dPointValue = __cProperty.BigPointValue;
				return (__dPriceScale == 0) ? dPointValue : dPointValue / __dPriceScale;
			}
		}

		/// <summary>
		///   [取得] 價格座標
		/// </summary>
		public double PriceScale {
			get {
				return __dPriceScale;
			}
		}

		/// <summary>
		///   [取得] 商品屬性
		/// </summary>
		public AbstractProductProperty Property {
			get {
				return __cProperty;
			}
		}

		/// <summary>
		///   [取得] Bars 時間週期
		/// </summary>
		public Resolution Resolution {
			get {
				return __cResolution;
			}
		}

		/// <summary>
		///   [取得] 商品開收盤時間
		/// </summary>
		public List<SessionObject> Sessions {
			get {
				return __cProperty.Sessions;
			}
		}

		/// <summary>
		///   [取得] 選擇權的履約價格(若不是選擇權, 此屬性皆為 0)
		/// </summary>
		public double StrikePrice {
			get {
				return __dStrikePrice;
			}
		}

		/// <summary>
		///   [取得] 交易所與 UTC 的時差
		/// </summary>
		public double TimeZone {
			get {
				return __dTimeZone;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="request">InstrumentDataRequest類別</param>
		public InstrumentSettings(ref InstrumentDataRequest request) {
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(request.Exchange);

			__sSymbolId = request.Symbol;
			__sExchange = request.Exchange;
			__sDataSource = request.DataFeed;
			
			__dTimeZone = cExchange.TimeZone;
			__cProduct = cExchange.GetProduct(__sSymbolId);
			__cProperty = cExchange.GetProperty(__sSymbolId, __sDataSource);

			if (__cProperty != null) {
				__cOptionType = __cProperty.GetCallOrPut(__cProduct);     //取得選擇權 Call or Put 類型
				__dStrikePrice = __cProperty.GetStrikePrice(__cProduct);  //取得選擇權履約價格
				
				//取得合約到期日索引值
				IContractTime cContractTime = __cProperty.ContractRule as IContractTime;
				if (cContractTime != null) {
					__iContractIndex = cContractTime.GetContractIndex(__sSymbolId);
				}
			}

			this.Create(ref request);
			__cResolution = request.Resolution;
		}

		/// <summary>
		///   根據設定資訊調整 InstrumentDataRequest 設定並回傳目前的設定類別
		/// </summary>
		/// <param name="request">InstrumentDataRequest類別</param>
		/// <returns>回傳值: 目前的 InstrumentSettings類別</returns>
		public InstrumentSettings Create(ref InstrumentDataRequest request) {
			SessionObject cSession = GetSessionFromToday();
			DataRequestType cRequestType = request.Range.RequestType;

			request.Resolution.CalculateRate(cSession.GetStartTimeForDaylight(), cSession.GetCloseTimeForDaylight(), __cProperty.Sessions.Count);
			if (cRequestType == DataRequestType.DaysBack || cRequestType == DataRequestType.FromTo) {
				request.Range.Count = request.Resolution.ConvertDaysToBars(request.Range.Count);
			}
			return this;
		}

		/// <summary>
		///   取得今日的開收盤時間
		/// </summary>
		/// <returns>返回值:SessionObject類別</returns>
		public SessionObject GetSessionFromToday() {
			DateTime cToday = DateTime.UtcNow.AddHours(__dTimeZone);
			DayOfWeek cWeek = cToday.DayOfWeek;

			SessionObject cSession = null;
			int iPeriodCount = __cProperty.Sessions.Count;
			for (int i = 0; i < iPeriodCount; i++) {
				cSession = __cProperty.Sessions[i];
				if (cSession.StartDay == cWeek) {
					break;
				}
			}
			return cSession;
		}

		/// <summary>
		///   從傳入的日期來設定合約到期日
		/// </summary>
		/// <param name="time">日期時間結構</param>
		public void SetExpirationFromTime(DateTime time) {
			IContractTime cContractTime = __cProperty.ContractRule as IContractTime;
			if (cContractTime == null) {
				__cExpiration = time.AddSeconds(86400);  //如果沒有設定預設為明天到期(表示沒設定到期日, 永遠都不會到期)
			} else {
				__cExpiration = cContractTime.GetContractTime(time, __iContractIndex).MaturityDate;
			}
		}

		/// <summary>
		///   從目標價格資訊設定價格座標
		/// </summary>
		/// <param name="price">目標價格</param>
		public void SetPriceScaleFromClosePrice(double price) {
			double[] dValues = new double[] { 1, 1 };
			IPriceScale cPriceScale = __cProperty.PriceScaleRule as IPriceScale;
			if (cPriceScale != null) {
				dValues = cPriceScale.GetPriceScale(price);
			}
			
			__dPriceScale = dValues[0];  //價格座標
			__dMinValue = dValues[1];    //最小跳動單位
		}
	}
}  //292行