namespace Zeghs.Informations {
	/// <summary>
	///   下單服務資訊類別
	/// </summary>
	public sealed class OrderServiceInformation {
		/// <summary>
		///   [取得/設定] 開發公司名稱
		/// </summary>
		public string Company {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 下單元件說明
		/// </summary>
		public string Description {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單元件檔案版本號碼
		/// </summary>
		public string FileVersion {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單元件本機位置
		/// </summary>
		public string Location {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單元件模組名稱
		/// </summary>
		public string ModuleName {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單元件產品版本號碼
		/// </summary>
		public string ProductVersion {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下單服務類別陣列
		/// </summary>
		public string[] Services {
			get;
			internal set;
		}
	}
}