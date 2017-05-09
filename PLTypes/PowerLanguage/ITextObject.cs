using System;
using System.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   文字繪圖物件介面
	/// </summary>
	public interface ITextObject : IDrawObject, IEquatable<IDrawObject>, IEquatable<ITextObject> {
		/// <summary>
		///   [取得/設定] 背景顏色
		/// </summary>
		Color BGColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否要繪製框線
		/// </summary>
		bool Border {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字顏色
		/// </summary>
		Color Color {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 繪製文字的字型名稱
		/// </summary>
		string FontName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字水平對齊樣式
		/// </summary>
		ETextStyleH HStyle {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字座標位置
		/// </summary>
		ChartPoint Location {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 繪製文字的字型大小
		/// </summary>
		int Size {
			get;
			set;
		}

		/// <summary>
		///   [取得] 文字樣式資訊
		/// </summary>
		FontStyle Style {
			get;
		}

		/// <summary>
		///   [取得/設定] 繪製文字
		/// </summary>
		string Text {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字垂直對齊樣式
		/// </summary>
		ETextStyleV VStyle {
			get;
			set;
		}

		/// <summary>
		///   檢查是否有此種字型樣式
		/// </summary>
		/// <param name="style">字型樣式列舉</param>
		/// <returns>返回值: true=有此樣式, false=無此樣式</returns>
		bool HaveFont(FontStyle style);

		/// <summary>
		///   設定字型樣式
		/// </summary>
		/// <param name="style">字型樣式列舉</param>
		/// <param name="val">是否啟用此樣式</param>
		void SetFont(FontStyle style, bool val);
	}
}