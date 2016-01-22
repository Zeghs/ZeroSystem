using System;
using PowerLanguage;
using Zeghs.Rules;
using Zeghs.Products;
using Taiwan.Rules;
using Taiwan.Rules.Contracts;

namespace Taiwan.Products {
	public class TaiwanProductProperty : AbstractProductProperty {
		private RuleBase __cTaxRule = null;
		private RuleBase __cPriceScaleRule = null;
		private RuleBase __cContractTimeRule = null;

		/// <summary>
		///   [取得/設定] 合約(交割)時間規則
		/// </summary>
		public override RuleBase ContractRule {
			get {
				return __cContractTimeRule;
			}
			
			set {
				if (value != null) {
					RuleBase cRule = RuleCreater.CreateRule(value);
					if (cRule == null) {
						__cContractTimeRule = value;
					} else {
						__cContractTimeRule = cRule;
					}
				}
			}
		}

		/// <summary>
		///   [取得/設定] 價格縮放規則
		/// </summary>
		public override RuleBase PriceScaleRule {
			get {
				return __cPriceScaleRule;
			}

			set {
				if (value != null) {
					RuleBase cRule = RuleCreater.CreateRule(value);
					if (cRule == null) {
						__cPriceScaleRule = value;
					} else {
						__cPriceScaleRule = cRule;
					}
				}
			}
		}

		/// <summary>
		///   [取得/設定] 交易稅金規則
		/// </summary>
		public override RuleBase TaxRule {
			get {
				return __cTaxRule;
			}

			set {
				if (value != null) {
					RuleBase cRule = RuleCreater.CreateRule(value);
					if (cRule == null) {
						__cTaxRule = value;
					} else {
						__cTaxRule = cRule;
					}
				}
			}
		}

		protected override OptionType GetCallOrPut(Product product) {
			OptionType cOptionType = OptionType.None;
			if (product.Category == ESymbolCategory.IndexOption || product.Category == ESymbolCategory.StockOption) {
				string sSymbolId = product.SymbolId;
				int iEndIndex = sSymbolId.LastIndexOf(".");
				int iLength = sSymbolId.Length;
				for (int i = 4; i < iLength; i++) {
					if (sSymbolId[i] > 'A') {
						cOptionType = ((sSymbolId[i] == 'C') ? OptionType.Call : (sSymbolId[i] == 'P') ? OptionType.Put : OptionType.None);
						break;
					}
				}
			}
			return cOptionType;
		}

		protected override double GetStrikePrice(Product product) {
			double dValue = 0;
			if (product.Category == ESymbolCategory.IndexOption || product.Category == ESymbolCategory.StockOption) {
				string sSymbolId = product.SymbolId;
				int iEndIndex = sSymbolId.LastIndexOf(".");
				int iLength = sSymbolId.Length;
				for (int i = 4; i < iLength; i++) {
					if (sSymbolId[i] > 'A') {
						++i;
						dValue = double.Parse(sSymbolId.Substring(i, iEndIndex - i));
						break;
					}
				}
			}
			return dValue;
		}

		protected override void UpdateContractTime(DateTime date) {
			if (this.ContractRule != null) {
				IContractTime cContractTime = this.ContractRule as IContractTime;
				cContractTime.UpdateContractTime(date);
			}
		}
	}
}