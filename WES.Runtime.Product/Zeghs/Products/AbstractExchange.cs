using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Rules;

namespace Zeghs.Products {
	/// <summary>
	///   交易所類別(所有創建的交易所都需要繼承這個類別)
	/// </summary>
	public abstract class AbstractExchange {
		private ProductClassify __cProductClassify = null;

		/// <summary>
		///   [取得/設定] 完整交易所的名稱
		/// </summary>
		public string FullName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易所簡稱
		/// </summary>
		public string ShortName {
			get;
			set;
		}

		/// <summary>
		///   [取得] 交易所時差(以UTC時間為準) 
		/// </summary>
		public abstract double TimeZone {
			get;
		}

		/// <summary>
		///   [取得] 是否需要更新 
		/// </summary>
		public bool IsUpdate {
			get {
				DateTime cDate = DateTime.Now;
				int iNowDate = cDate.Year * 10000 + cDate.Month * 100 + cDate.Day;
				int iUpdateDate = this.UpdateTime.Year * 10000 + this.UpdateTime.Month * 100 + this.UpdateTime.Day;
				return (iNowDate > iUpdateDate);
			}
		}

		/// <summary>
		///   [取得/設定] 更新時間
		/// </summary>
		public DateTime UpdateTime {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 產品資訊列表
		/// </summary>
		protected Dictionary<string, Product> Products {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 基礎產品屬性列表
		/// </summary>
		protected ProductPropertyList BasePropertys {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 使用者自訂產品屬性列表
		/// </summary>
		protected Dictionary<string, ProductPropertyList> CustomPropertys {
			get;
			set;
		}
		
		/// <summary>
		///   建構子
		/// </summary>
		public AbstractExchange() {
			__cProductClassify = new ProductClassify();
		}

		/// <summary>
		///   取得可用的規則項目列表
		/// </summary>
		/// <param name="ruleType">規則型態(由型態區分規則項目)</param>
		/// <returns>返回值:規則項目列表</returns>
		public abstract List<RulePropertyAttribute> GetRuleItems(ERuleType ruleType);

		/// <summary>
		///   讀取交易所設定資訊
		/// </summary>
		public abstract void Load();

		/// <summary>
		///   寫入交易所設定資訊
		/// </summary>
		public abstract void Save();

		/// <summary>
		///   新增商品(如果商品已經存在會覆寫之前商品)
		/// </summary>
		/// <param name="product">商品類別</param>
		public void AddProduct(Product product) {
			Product cProduct = null;
			string sSymbolId = product.SymbolId.ToLower();
			lock (this.Products) {
				if (this.Products.TryGetValue(sSymbolId, out cProduct)) {
					cProduct.CommodityId = product.CommodityId;
					cProduct.Category = product.Category;
					cProduct.SymbolName = product.SymbolName;
				} else {
					this.Products.Add(sSymbolId, product);
					__cProductClassify.Add(product);
				}
			}
		}

		/// <summary>
		///   新增商品屬性(如果商品已經存在會覆寫之前屬性設定)
		/// </summary>
		/// <param name="property">商品屬性類別</param>
		/// <param name="dataSource">資料來源名稱</param>
		public void AddProperty(AbstractProductProperty property, string dataSource = null) {
			if (dataSource == null) {
				BasePropertys.AddProperty(property);
			} else {
				ProductPropertyList cPropertyList = null;
				
				if (!CustomPropertys.TryGetValue(dataSource, out cPropertyList)) {
					cPropertyList = new ProductPropertyList();
					CustomPropertys.Add(dataSource, cPropertyList);
				}
				cPropertyList.AddProperty(property);
			}
		}

		/// <summary>
		///   新增商品屬性(如果商品已經存在會覆寫之前屬性設定)
		/// </summary>
		/// <param name="commodityId">商品源名稱(也可以是商品代號)</param>
		/// <param name="dataSource">資料來源名稱</param>
		public void AddProperty(string commodityId, string dataSource) {
			this.AddProperty(CreateProperty(commodityId), dataSource);
		}

		/// <summary>
		///   清除所有商品
		/// </summary>
		public void Clear() {
			lock (this.Products) {
				this.Products.Clear();
				__cProductClassify.Clear();
			}
		}

		/// <summary>
		///   取得商品
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:Product類別</returns>
		public Product GetProduct(string symbolId) {
			Product cProduct = null;
			string sSymbolId = symbolId.ToLower();

			lock (this.Products) {
				this.Products.TryGetValue(sSymbolId, out cProduct);
			}
			return cProduct;
		}

		/// <summary>
		///   取得商品
		/// </summary>
		/// <param name="productType">商品類型</param>
		/// <returns>返回值:此分類的商品代號列表</returns>
		public List<string> GetProductClassify(ESymbolCategory productType) {
			return　__cProductClassify.GetClassify(productType);
		}

		/// <summary>
		///   取得商品屬性
		/// </summary>
		/// <param name="symbol">商品來源代號或商品代號</param>
		/// <param name="dataSource">資料來源名稱</param>
		public AbstractProductProperty GetProperty(string symbol, string dataSource = null) {
			AbstractProductProperty cProperty = null;

			Product cProduct = GetProduct(symbol);
			if (dataSource != null) {
				ProductPropertyList cPropertyList = null;
				if (CustomPropertys.TryGetValue(dataSource, out cPropertyList)) {
					if (cProduct == null) {
						cProperty = cPropertyList.GetProperty(symbol);
					} else {
						cProperty = cPropertyList.GetProperty(cProduct);
					}
				}
			}

			if (cProperty == null) {
				if (cProduct == null) {
					cProperty = this.BasePropertys.GetProperty(symbol);
				} else {
					cProperty = this.BasePropertys.GetProperty(cProduct);
				}
			}
			return cProperty;
		}

		/// <summary>
		///   移除商品
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public void RemoveProduct(string symbolId) {
			string sSymbolId = symbolId.ToLower();
			lock (this.Products) {
				Product cProduct = null;
				if (this.Products.TryGetValue(sSymbolId, out cProduct)) {
					this.Products.Remove(sSymbolId);
					__cProductClassify.Remove(cProduct);
				}
			}
		}

		/// <summary>
		///   移除商品屬性
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <param name="dataSource">資料來源名稱</param>
		public void RemoveProperty(string symbolId, string dataSource = null) {
			Product cProduct = GetProduct(symbolId);
			if (dataSource == null) {
				BasePropertys.RemoveProperty(cProduct);
			} else {
				ProductPropertyList cPropertyList = null;
				if (CustomPropertys.TryGetValue(dataSource, out cPropertyList)) {
					cPropertyList.RemoveProperty(cProduct);
					if (cPropertyList.Count == 0) {
						CustomPropertys.Remove(dataSource);
					}
				}
			}
		}

		/// <summary>
		///   搜尋商品資訊
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <param name="isMatch">是否需要吻合(true=商品代號需要完全吻合, false=商品代號不需要完全吻合)</param>
		/// <returns>返回值:商品資訊列表</returns>
		public List<Product> SearchProducts(string symbolId, bool isMatch) {
			List<Product> cProducts = new List<Product>(128);

			string sLSymbolId = symbolId.ToLower();
			lock (this.Products) {
				if (isMatch) {
					Product cProduct = null;
					if (this.Products.TryGetValue(sLSymbolId, out cProduct)) {
						cProducts.Add(cProduct);
					}
				} else {
					foreach (string sSymbolId in this.Products.Keys) {
						if (sSymbolId.IndexOf(sLSymbolId) > -1) {
							cProducts.Add(this.Products[sSymbolId]);
						}
					}
				}
			}
			return cProducts;
		}

		/// <summary>
		///   更新交易所所有商品屬性資訊
		/// </summary>
		/// <param name="date">欲更新的合約基準時間(通常都以今日為主, 也可以傳入基準時間來更新合約到期日)</param>
		public void Update(DateTime date) {
			this.BasePropertys.UpdateProperty(date);
			foreach (ProductPropertyList cPropertyList in this.CustomPropertys.Values) {
				cPropertyList.UpdateProperty(date);
			}
		}

		internal void Initialize() {
			int iCount = Products.Count;
			if (iCount > 0) {
				foreach (Product cProduct in Products.Values) {
					__cProductClassify.Add(cProduct);
				}
			}
		}

		/// <summary>
		///   建立新的商品屬性類別
		/// </summary>
		/// <param name="commodityId">商品源名稱(也可以是商品代號)</param>
		/// <returns>返回值: AbstractProductProperty 類別</returns>
		protected abstract AbstractProductProperty CreateProperty(string commodityId);
	}
}