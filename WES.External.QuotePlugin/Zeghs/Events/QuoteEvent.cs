using System;
using Zeghs.Data;

namespace Zeghs.Events {
	/// <summary>
	///   即時報價事件類別(當有即時報價時，會觸發此事件通知)
	/// </summary>
	public sealed class QuoteEvent : EventArgs {
		private IQuote __cQuote = null;
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
		///   [取得] 報價資訊
		/// </summary>
		public IQuote Quote {
			get {
				return __cQuote;
			}
		}

		/// <summary>
		///   [取得] 最新成交的Tick
		/// </summary>
		public ITick Tick {
			get {
				return __cQuote.RealTick;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="exchangeName">交易所簡稱</param>
		/// <param name="dataSource">報價元件名稱</param>
		/// <param name="quote">報價資訊</param>
		public QuoteEvent(string exchangeName, string dataSource, IQuote quote) {
			__sExchangeName = exchangeName;
			__sDataSource = dataSource;
			__cQuote = quote;
		}
	}
}