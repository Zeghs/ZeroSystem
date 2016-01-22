using System;

namespace Zeghs.Events {
	/// <summary>
	///   報價服務啟動或關閉狀態改變所觸發的事件
	/// </summary>
	public sealed class QuoteServiceSwitchChangedEvent : EventArgs {
		private bool __bRunning = false;
		private string __sDataSource = null;

		/// <summary>
		///   [取得] 報價資料來源名稱
		/// </summary>
		public string DataSource {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   [取得] 是否在運作中
		/// </summary>
		public bool IsRunning {
			get {
				return __bRunning;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="dataSource">報價資料來源名稱</param>
		/// <param name="isRunning">是否在運作中</param>
		internal QuoteServiceSwitchChangedEvent(string dataSource, bool isRunning) {
			__bRunning = isRunning;
			__sDataSource = dataSource;
		}
	}
}