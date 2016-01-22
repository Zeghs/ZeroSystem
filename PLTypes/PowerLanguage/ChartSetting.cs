using System.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   圖表設定類別
	/// </summary>
	public sealed class ChartSetting {
		/// <summary>
		///   [取得/設定] 座標設定值
		/// </summary>
		public AxisSetting Axis {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 圖表類型
		/// </summary>
		public EChartType ChartType {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否顯示最新價格
		/// </summary>
		public bool IsShowNewPrice {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否為副圖(如果不是副圖則就是隱藏)
		/// </summary>
		public bool IsSubChart {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 畫筆樣式
		/// </summary>
		public PenStyle[] PenStyles {
			get;
			set;
		}
	}
}