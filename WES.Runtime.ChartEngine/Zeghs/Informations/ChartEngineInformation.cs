namespace Zeghs.Informations {
	/// <summary>
	///   圖表引擎資訊類別
	/// </summary>
	public sealed class ChartEngineInformation {
		/// <summary>
		///   [取得] 圖表引擎類別
		/// </summary>
		public string ChartEngine {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 開發公司名稱
		/// </summary>
		public string Company {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 圖表引擎元件說明
		/// </summary>
		public string Description {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 圖表引擎檔案版本號碼
		/// </summary>
		public string FileVersion {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 圖表引擎本機位置
		/// </summary>
		public string Location {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 圖表引擎模組名稱
		/// </summary>
		public string ModuleName {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 圖表引擎產品版本號碼
		/// </summary>
		public string ProductVersion {
			get;
			internal set;
		}
	}
}