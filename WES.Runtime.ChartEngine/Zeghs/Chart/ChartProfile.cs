namespace Zeghs.Chart {
	/// <summary>
	///   圖表設定檔類別
	/// </summary>
	public sealed class ChartProfile {
		/// <summary>
		///   [取得/設定] 圖表引擎資訊索引
		/// </summary>
		public int ChartEngineIndex {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 基礎圖表屬性設定
		/// </summary>
		public ChartProperty DefaultProperty {
			get;
			set;
		}
	}
}