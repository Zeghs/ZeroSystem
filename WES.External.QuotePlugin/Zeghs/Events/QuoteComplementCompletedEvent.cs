using System;

namespace Zeghs.Events {
	/// <summary>
	///   回補事件類別(當回補Tick完畢後，會觸發此事件)
	/// </summary>
	public sealed class QuoteComplementCompletedEvent : EventArgs {
		private string __sSymbolId = null;
		private string __sDataSource = null;
		private string __sExchangeName = null;

		/// <summary>
		///   [取得] 報價元件名稱
		/// </summary>
		public string DataSource {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   [取得] 交易所簡稱
		/// </summary>
		public string ExchangeName {
			get {
				return __sExchangeName;
			}
		}

		/// <summary>
		///   [取得] 回補資料的商品代號
		/// </summary>
		public string SymbolId {
			get {
				return __sSymbolId;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="exchangeName">交易所簡稱</param>
		/// <param name="dataSource">報價元件名稱</param>
		/// <param name="symbolId">回補資料的商品代號</param>
		public QuoteComplementCompletedEvent(string exchangeName, string dataSource, string symbolId) {
			__sExchangeName = exchangeName;
			__sDataSource = dataSource;
			__sSymbolId = symbolId;
		}
	}
}