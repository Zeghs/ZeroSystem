using Zeghs.Services;
using PowerLanguage;

namespace Zeghs.Informations {
	/// <summary>
	///   即時報價元件來源資訊
	/// </summary>
	public sealed class DataSourceInformation {
		private string __sExchangeName = null;

		private Product __cProduct = null;
		private AbstractQuoteService __cQuoteService = null;

		/// <summary>
		///   [取得] 交易所簡稱
		/// </summary>
		public string ExchangeName {
			get {
				return __sExchangeName;
			}

		}
		
		/// <summary>
		///   [取得] 資料來源名稱
		/// </summary>
		public string DataSource {
			get {
				return __cQuoteService.DataSource;
			}
		}

		/// <summary>
		///   [取得] 商品資訊
		/// </summary>
		public Product Product {
			get {
				return __cProduct;
			}
		}

		/// <summary>
		///   [取得] 即時報價服務
		/// </summary>
		public AbstractQuoteService QuoteService {
			get {
				return __cQuoteService;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="exchangeName">交易所簡稱</param>
		/// <param name="quoteService">即時報價服務</param>
		/// <param name="product">商品資訊</param>
		public DataSourceInformation(string exchangeName, AbstractQuoteService quoteService, Product product) {
			__sExchangeName = exchangeName;
			__cQuoteService = quoteService;
			__cProduct = product;
		}
	}
}