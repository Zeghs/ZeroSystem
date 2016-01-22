namespace Zeghs.Scripts {
	/// <summary>
	///   腳本設定類別
	/// </summary>
	public sealed class ScriptSettings {
		/// <summary>
		///   [取得/設定] 編譯後的輸出路徑
		/// </summary>
		public string OutputPath {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本來源路徑
		/// </summary>
		public string SourcePath {
			get;
			set;
		}
	}
}