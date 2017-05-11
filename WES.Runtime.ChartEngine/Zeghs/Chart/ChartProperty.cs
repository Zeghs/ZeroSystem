using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表屬性類別
	/// </summary>
	public sealed class ChartProperty {
		/// <summary>
		///   [取得/設定] Axis 顏色
		/// </summary>
		public Color AxisColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 座標軸預設字型
		/// </summary>
		public Font AxisFont {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 背景顏色
		/// </summary>
		public Color BackgroundColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Chart 設定值
		/// </summary>
		public List<ChartSetting> ChartSettings {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 繪製輔助線顏色
		/// </summary>
		public Color DrawAideLineColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 繪製來源狀態
		/// </summary>
		public EDrawingSource DrawingSource {
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

		/// <summary>
		///   [取得/設定] 圖例標籤預設字型
		/// </summary>
		public Font LegendFont {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 使用者輸入文字預設字型
		/// </summary>
		public Font TextFont {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Title 預設字型(標頭顯示商品名稱資訊)
		/// </summary>
		public Font TitleFont {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易標註線顏色
		/// </summary>
		public Color TradeLineColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易標註箭頭符號顏色
		/// </summary>
		public Color TradeSymbolColor {
			get;
			set;
		}
	}
}