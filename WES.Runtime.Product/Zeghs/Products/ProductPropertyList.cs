using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;

namespace Zeghs.Products {
	/// <summary>
	///   商品屬性列表類別
	/// </summary>
	public sealed class ProductPropertyList {
		private Dictionary<string, AbstractProductProperty> __cPropertys = null;

		/// <summary>
		///   [取得] 商品屬性個數
		/// </summary>
		public int Count {
			get {
				return __cPropertys.Count;
			}
		}

		/// <summary>
		///   [取得] 所有商品屬性設定
		/// </summary>
		public Dictionary<string, AbstractProductProperty> Propertys {
			get {
				return __cPropertys;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public ProductPropertyList() {
			__cPropertys = new Dictionary<string, AbstractProductProperty>(32);
		}

		/// <summary>
		///   新增或修改商品屬性(如果商品已經存在會覆寫原本的設定值)
		/// </summary>
		/// <param name="property">商品屬性類別</param>
		public void AddProperty(AbstractProductProperty property) {
			AbstractProductProperty cProperty = null;
			string sCommodityId = property.CommodityId;
			
			lock (__cPropertys) {
				if (__cPropertys.TryGetValue(sCommodityId, out cProperty)) {
					cProperty.BigPointValue = property.BigPointValue;
					cProperty.CautionMoneys = property.CautionMoneys;
					cProperty.CommodityId = property.CommodityId;
					cProperty.ContractRule = property.ContractRule;
					cProperty.DailyLimit = property.DailyLimit;
					cProperty.Description = property.Description;
					cProperty.PriceScaleRule = property.PriceScaleRule;
					cProperty.Sessions = property.Sessions;
					cProperty.TaxRule = property.TaxRule;
				} else {
					__cPropertys.Add(sCommodityId, property);
				}
			}
		}

		/// <summary>
		///   取得商品屬性
		/// </summary>
		/// <param name="propertyName">屬性名稱</param>
		/// <returns>返回值:AbstractProductProperty類別</returns>
		public AbstractProductProperty GetProperty(string propertyName) {
			AbstractProductProperty cProperty = null;

			if (propertyName != null) {
				lock (__cPropertys) {
					__cPropertys.TryGetValue(propertyName, out cProperty);
				}
			}
			return cProperty;
		}

		/// <summary>
		///   取得商品屬性
		/// </summary>
		/// <param name="product">商品類別</param>
		/// <returns>返回值:AbstractProductProperty類別</returns>
		public AbstractProductProperty GetProperty(Product product) {
			AbstractProductProperty cProperty = null;
			
			if (product != null) {
				lock (__cPropertys) {
					__cPropertys.TryGetValue(product.SymbolId, out cProperty);
					if (cProperty == null) {
						string sCommodity = product.CommodityId;
						if (sCommodity != null) {
							__cPropertys.TryGetValue(sCommodity, out cProperty);
						}
					}
				}
			}
			return cProperty;
		}

		/// <summary>
		///   移除商品屬性
		/// </summary>
		/// <param name="product">商品類別</param>
		public void RemoveProperty(Product product) {
			if (product != null) {
				lock (__cPropertys) {
					string sKey = product.SymbolId;
					if (__cPropertys.ContainsKey(sKey)) {
						__cPropertys.Remove(sKey);
					} else {
						sKey = product.CommodityId;
						if (sKey != null && __cPropertys.ContainsKey(sKey)) {
							__cPropertys.Remove(sKey);
						}
					}
				}
			}
		}

		/// <summary>
		///   更新商品屬性
		/// </summary>
		/// <param name="date">欲更新的合約基準時間(通常都以今日為主, 也可以傳入基準時間來更新合約到期日)</param>
		internal void UpdateProperty(DateTime date) {
			lock (__cPropertys) {
				foreach (AbstractProductProperty cProperty in __cPropertys.Values) {
					cProperty.UpdateContractTime(date);
				}
			}
		}
	}
}