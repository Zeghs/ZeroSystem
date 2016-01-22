using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerLanguage {
	/// <summary>
	///   商品類別
	/// </summary>
	public sealed class Product {
		/// <summary>
		///   [取得/設定] 商品來源Id
		/// </summary>
		public string CommodityId {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品代號
		/// </summary>
		public string SymbolId {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品名稱
		/// </summary>
		public string SymbolName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品類型
		/// </summary>
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public ESymbolCategory Category {
			get;
			set;
		}
	}
}