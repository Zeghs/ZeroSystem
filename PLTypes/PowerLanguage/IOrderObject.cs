namespace PowerLanguage {
	/// <summary>
	///   下單物件介面
	/// </summary>
	public interface IOrderObject {
		/// <summary>
		///   [取得] 下單的代號
		/// </summary>
		int ID {
			get;
		}

		/// <summary>
		///   [取得] 下單資訊
		/// </summary>
		Order Info {
			get;
		}
	}
}