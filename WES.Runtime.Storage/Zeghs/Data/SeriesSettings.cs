namespace Zeghs.Data {
	/// <summary>
	///   Series 設定資訊
	/// </summary>
	public sealed class SeriesSettings {
		/// <summary>
		///   [取得/設定] Http Server domain
		/// </summary>
		public string HttpDomain {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Request method
		/// </summary>
		public string TargetUrl {
			get;
			set;
		}
	}
}