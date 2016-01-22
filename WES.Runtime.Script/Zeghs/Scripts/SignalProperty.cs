namespace Zeghs.Scripts {
	/// <summary>
	///   信號設定屬性類別
	/// </summary>
	public sealed class SignalProperty {
		/// <summary>
		///   [取得/設定] 預設的合約數量
		/// </summary>
		public int DefaultContracts {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 初始本金
		/// </summary>
		public double InitialCapital {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 信號使用的最大 Bars count 參考值
		/// </summary>
		public int MaximumBarsReference {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 下單來源名稱
		/// </summary>
		public string OrderSource {
			get;
			set;
		}
	}
}