using System;

namespace PowerLanguage {
	/// <summary>
	///   基礎繪圖物件介面
	/// </summary>
	public interface IDrawObject : IEquatable<IDrawObject> {
		/// <summary>
		///   [取得] 繪圖物件是否存在
		/// </summary>
		bool Exist {
			get;
		}

		/// <summary>
		///   [取得] 繪圖物件 ID
		/// </summary>
		int ID {
			get;
		}

		/// <summary>
		///   [設定] 鎖定繪圖物件以防止用戶修改
		/// </summary>
		bool Locked {
			set;
		}

		/// <summary>
		///   繪圖物件是否被刪除
		/// </summary>
		/// <returns>回傳值: true=刪除, false=尚未刪除</returns>
		bool Delete();
	}
}