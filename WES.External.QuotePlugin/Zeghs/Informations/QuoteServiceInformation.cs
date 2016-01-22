namespace Zeghs.Informations {
	/// <summary>
	///   報價服務資訊類別
	/// </summary>
	public sealed class QuoteServiceInformation {
		/// <summary>
		///   [取得/設定] 開發公司名稱
		/// </summary>
		public string Company {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 報價元件名稱
		/// </summary>
		public string DataSource {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 報價元件說明
		/// </summary>
		public string Description {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 是否啟用
		/// </summary>
		public bool Enabled {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價元件檔案版本號碼
		/// </summary>
		public string FileVersion {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價元件本機位置
		/// </summary>
		public string Location {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價元件模組名稱
		/// </summary>
		public string Name {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價元件產品版本號碼
		/// </summary>
		public string ProductVersion {
			get;
			internal set;
		}
	}
}