namespace PowerLanguage {
	/// <summary>
	///   圖表座標設定值
	/// </summary>
	public sealed class AxisSetting {
		/// <summary>
		///   [取得/設定] 座標範圍
		/// </summary>
		public EAxisScope AxisScope {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 下邊界%數
		/// </summary>
		public double MarginBottom {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 上邊界%數
		/// </summary>
		public double MarginTop {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 刻度值模式
		/// </summary>
		public EAxisScaleMode ScaleMode {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 刻度設定值(根據 EAxisScaleMode 分別)
		/// </summary>
		public double ScaleValue {
			get;
			set;
		}
	}
}