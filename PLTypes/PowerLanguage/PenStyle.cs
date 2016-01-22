using System;
using System.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   圖表畫筆樣式
	/// </summary>
	public sealed class PenStyle {
		/// <summary>
		///   [取得/設定] 畫筆顏色
		/// </summary>
		public Color Color {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 畫筆線條格式
		/// </summary>
		public int Pattern {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 畫筆寬度
		/// </summary>
		public int Width {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="color">畫筆顏色</param>
		/// <param name="width">畫筆寬度</param>
		public PenStyle(Color color, int width) {
			this.Color = color;
			this.Width = width;
			this.Pattern = -1;
		}
	}
}
