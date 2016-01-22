namespace PowerLanguage {
	/// <summary>
	///   委託報價資訊集合
	/// </summary>
	public interface IDOMData {
		/// <summary>
		///   叫價資訊陣列
		/// </summary>
		DOMPrice[] Ask {
			get;
		}

		/// <summary>
		///   詢價資訊陣列
		/// </summary>
		DOMPrice[] Bid {
			get;
		}
	}
}