using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PowerLanguage;
using Zeghs.Rules;

namespace Zeghs.Products {
	/// <summary>
	///   商品屬性類別(所有創建的交易所都需要繼承此類別)
	/// </summary>
	public abstract class AbstractProductProperty {
		/// <summary>
		///   [取得/設定] 每1大點金額
		/// </summary>
		public double BigPointValue {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 保證金資訊列表
		/// </summary>
		public List<CautionMoney> CautionMoneys {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品源代號或是商品代號
		/// </summary>
		public string CommodityId {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品註解
		/// </summary>
		public string Description {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 合約規則(規則類別完整名稱)
		/// </summary>
		public abstract RuleBase ContractRule {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 當日漲跌幅限制
		/// </summary>
		public double DailyLimit {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 價格縮放規則
		/// </summary>
		public abstract RuleBase PriceScaleRule {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易稅規則(規則類別完整名稱)
		/// </summary>
		public abstract RuleBase TaxRule {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品交易時段
		/// </summary>
		public List<SessionObject> Sessions {
			get;
			set;
		}

		/// <summary>
		///   建立 AbstractProductProperty 淺層覆本
		/// </summary>
		/// <returns>返回值: AbstractProductProperty 類別</returns>
		public AbstractProductProperty Clone() {
			AbstractProductProperty cProperty = this.MemberwiseClone() as AbstractProductProperty;

			int iCautionCount = this.CautionMoneys.Count;
			List<CautionMoney> cCautions = new List<CautionMoney>(iCautionCount + 1);
			cProperty.CautionMoneys = cCautions;

			if (iCautionCount > 0) {
				for (int i = 0; i < iCautionCount; i++) {
					cCautions.Add(this.CautionMoneys[i].Clone());
				}
			}

			int iSessionCount = this.Sessions.Count;
			List<SessionObject> cSessions = new List<SessionObject>(iSessionCount + 1);
			cProperty.Sessions = cSessions;

			if (iSessionCount > 0) {
				for (int i = 0; i < iSessionCount; i++) {
					cSessions.Add(this.Sessions[i].Clone());
				}
			}
			return cProperty;
		}

		/// <summary>
		///   取得選擇權為 Call 或 Put(如果是其他商品則為 None)
		/// </summary>
		/// <param name="product">商品資訊類別</param>
		/// <returns>返回值: OptionType列舉</returns>
		internal protected abstract OptionType GetCallOrPut(Product product);

		/// <summary>
		///   取得選擇權履約價格(如果是其他商品則為 0 )
		/// </summary>
		/// <param name="product">商品資訊類別</param>
		/// <returns>返回值: 履約價格</returns>
		internal protected abstract double GetStrikePrice(Product product);

		/// <summary>
		///   更新合約規則(每天需要更新一次新的合約到期規則)
		/// </summary>
		/// <param name="date">欲更新的合約基準時間(通常都以今日為主, 也可以傳入基準時間來更新合約到期日)</param>
		internal protected abstract void UpdateContractTime(DateTime date);
	}
}