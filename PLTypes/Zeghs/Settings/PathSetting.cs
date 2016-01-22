namespace Zeghs.Settings {
	/// <summary>
	///   預設系統目錄路徑設定類別
	/// </summary>
	public sealed class PathSetting {
		/// <summary>
		///   [取得/設定] Cache 路徑
		/// </summary>
		public string CachePath {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Database 路徑
		/// </summary>
		public string DatabasePath {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Log 路徑
		/// </summary>
		public string LogPath {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Database 路徑
		/// </summary>
		public string ProfilePath {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Order 與 Position 的追蹤路徑
		/// </summary>
		public string TrackerPath {
			get;
			set;
		}
	}
}