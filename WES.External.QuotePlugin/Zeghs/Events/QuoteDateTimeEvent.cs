using System;

namespace Zeghs.Events {
	/// <summary>
	///   即時伺服器系統時間事件
	/// </summary>
	public sealed class QuoteDateTimeEvent : EventArgs {
		private string __sDataSource = null;
		private DateTime __cQuoteDateTime = DateTime.Now;

		/// <summary>
		///   [取得] 報價元件名稱
		/// </summary>
		public string DataSource {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   [取得] 報價伺服器系統時間
		/// </summary>
		public DateTime QuoteDateTime {
			get {
				return __cQuoteDateTime;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="dataSource">報價元件名稱</param>
		/// <param name="quoteDateTime">報價伺服器系統時間</param>
		public QuoteDateTimeEvent(string dataSource, DateTime quoteDateTime) {
			__cQuoteDateTime = quoteDateTime;
		}
	}
}