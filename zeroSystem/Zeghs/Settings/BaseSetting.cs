﻿namespace Zeghs.Settings {
	/// <summary>
	///   基礎設定檔
	/// </summary>
	public sealed class BaseSetting {
		/// <summary>
		///   [取得/設定] 顯示交易檢視頁籤
		/// </summary>
		public bool ShowTradeView {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] ZeroSystem Title
		/// </summary>
		public string Title {
			get;
			set;
		}
	}
}