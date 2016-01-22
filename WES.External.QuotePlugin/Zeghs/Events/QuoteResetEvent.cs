using System;

namespace Zeghs.Events {
	/// <summary>
	///   報價服務清盤重置事件
	/// </summary>
	public sealed class QuoteResetEvent : EventArgs {
		private string __sDataSource = null;

		/// <summary>
		///   [取得] 報價元件名稱
		/// </summary>
		public string DataSource {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="dataSource">報價元件名稱</param>
		public QuoteResetEvent(string dataSource) {
			__sDataSource = dataSource;
		}
	}
}