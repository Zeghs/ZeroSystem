using System.Drawing;

namespace Zeghs.Settings {
	/// <summary>
	///   圖表屬性類別
	/// </summary>
	public sealed class ChartProperty {
		/// <summary>
		///   [取得/設定] 背景顏色
		/// </summary>
		public Color BackgroundColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 預設字體名稱
		/// </summary>
		public string FontName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 預設字體大小
		/// </summary>
		public int FontSize {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 前景顏色
		/// </summary>
		public Color ForeColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 網格顏色
		/// </summary>
		public Color GridColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否顯示網格
		/// </summary>
		public bool IsShowGrid {
			get;
			set;
		}
	}
}