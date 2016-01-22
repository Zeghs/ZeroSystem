using System;
using System.Drawing;
using System.Threading;
using PowerLanguage;

namespace Zeghs.Drawing {
	/// <summary>
	///   文字繪圖物件類別
	/// </summary>
	public sealed class TextObject : ITextObject {
		private static int __iObjectID = 0;

		private int __iID = 0;
		private bool __bExist = true;
		private bool __bLocked = false;
		private FontStyle __cStyle = FontStyle.Regular;

		/// <summary>
		///   [取得/設定] 背景顏色
		/// </summary>
		public Color BGColor {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否要繪製框線
		/// </summary>
		public bool Border {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字顏色
		/// </summary>
		public Color Color {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 資料串流編號
		/// </summary>
		public int DataStream {
			get;
			internal set;
		}
		
		/// <summary>
		///   [取得/設定] 繪製來源旗標(1=Study, 2=Manual, 4=Other)
		/// </summary>
		public int DrawingSourceFlag {
			get;
			set;
		}

		/// <summary>
		///   [取得] 繪圖物件是否存在
		/// </summary>
		public bool Exist {
			get {
				return __bExist;
			}
		}

		/// <summary>
		///   [取得/設定] 繪製文字的字型名稱
		/// </summary>
		public string FontName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字水平對齊樣式
		/// </summary>
		public ETextStyleH HStyle {
			get;
			set;
		}

		/// <summary>
		///   [取得] 繪圖物件 ID
		/// </summary>
		public int ID {
			get {
				return __iID;
			}
		}

		/// <summary>
		///   [取得/設定] 文字座標位置
		/// </summary>
		public ChartPoint Location {
			get;
			set;
		}

		/// <summary>
		///   [設定] 鎖定繪圖物件以防止用戶修改
		/// </summary>
		public bool Locked {
			set {
				__bLocked = value;
			}
		}

		/// <summary>
		///   [取得/設定] 是否在同一個副圖上
		/// </summary>
		public bool OnSameSubchart {
			get;
			internal set;
		}
	
		/// <summary>
		///   [取得/設定] 繪製文字的字型大小
		/// </summary>
		public int Size {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 繪製文字
		/// </summary>
		public string Text {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 文字垂直對齊樣式
		/// </summary>
		public ETextStyleV VStyle {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		public TextObject() {
			__iID = Interlocked.Increment(ref __iObjectID);
		}

		/// <summary>
		///   繪圖物件是否被刪除
		/// </summary>
		/// <returns>回傳值: true=刪除, false=尚未刪除</returns>
		public bool Delete() {
			if (__bExist) {
				__bExist = false;
			}
			return !__bExist;
		}

		/// <summary>
		///   指出目前的物件是否等於另一個具有相同型別的物件。
		/// </summary>
		/// <param name="other">要與這個物件相互比較的物件。</param>
		/// <returns>如果目前的物件等於 other 參數，則為 true，否則為 false。</returns>
		public bool Equals(IDrawObject other) {
			if (other == null) {
				return false;
			}
			return this.ID == other.ID;
		}

		/// <summary>
		///   指出目前的物件是否等於另一個具有相同型別的物件。
		/// </summary>
		/// <param name="other">要與這個物件相互比較的物件。</param>
		/// <returns>如果目前的物件等於 other 參數，則為 true，否則為 false。</returns>
		public bool Equals(ITextObject other) {
			if (other == null) {
				return false;
			}
			return this.ID == other.ID;
		}

		/// <summary>
		///   判斷指定的 System.Object 和目前的 System.Object 是否相等。
		/// </summary>
		/// <param name="obj">System.Object，要與目前的 System.Object 比較。</param>
		/// <returns>如果指定的 System.Object 和目前的 System.Object 相等，則為 true，否則為 false。</returns>
		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			
			TextObject cTextObject = obj as TextObject;
			if (cTextObject == null) {
				return false;
			}
			return this.ID == cTextObject.ID;
		}

		/// <summary>
		///   做為特定型別的雜湊函式。
		/// </summary>
		/// <returns>目前 System.Object 的雜湊程式碼。</returns>
		public override int GetHashCode() {
			return __iID;
		}

		/// <summary>
		///   檢查是否有此種字型樣式
		/// </summary>
		/// <param name="style">字型樣式列舉</param>
		/// <returns>返回值: true=有此樣式, false=無此樣式</returns>
		public bool HaveFont(FontStyle style) {
			return (__cStyle & style) == style;
		}

		/// <summary>
		///   設定字型樣式
		/// </summary>
		/// <param name="style">字型樣式列舉</param>
		/// <param name="val">是否啟用此樣式</param>
		public void SetFont(FontStyle style, bool val) {
			if (val) {
				__cStyle |= style;
			} else {
				__cStyle ^= style;
			}
		}
	}
}