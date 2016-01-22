using System;
using Zeghs.Data;

namespace Zeghs.Events {
	/// <summary>
	///   即時公告事件類別(當有即時公告時，會觸發此事件通知)
	/// </summary>
	public sealed class QuoteNoticeEvent : EventArgs {
		private INotice __cNotice = null;
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
		///   [取得] 公告內容
		/// </summary>
		public INotice Notice {
			get {
				return __cNotice;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="dataSource">報價元件名稱</param>
		/// <param name="notice">公告內容</param>
		public QuoteNoticeEvent(string dataSource, INotice notice) {
			__sDataSource = dataSource;
			__cNotice = notice;
		}
	}
}