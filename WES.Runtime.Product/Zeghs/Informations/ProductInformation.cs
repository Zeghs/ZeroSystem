using Zeghs.Products;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Informations {
	/// <summary>
	///   產品資訊類別
	/// </summary>
	public sealed class ProductInformation {
		private List<Product> __cProducts = null;
		private AbstractExchange __cExchange = null;


		/// <summary>
		///   [取得] 交易所類別
		/// </summary>
		public AbstractExchange Exchange {
			get {
				return __cExchange;
			}
		}

		/// <summary>
		///   [取得] 商品資訊列表
		/// </summary>
		public List<Product> Products {
			get {
				return __cProducts;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="exchange">交易所類別</param>
		/// <param name="products">商品資訊列表</param>
		public ProductInformation(AbstractExchange exchange, List<Product> products) {
			__cExchange = exchange;
			__cProducts = products;
		}
	}
}