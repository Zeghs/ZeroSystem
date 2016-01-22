namespace PowerLanguage {
	/// <summary>
	///   資料請求類型
	/// </summary>
	public enum DataRequestType {
		/// <summary>
		///   從目前的日期往前幾天的讀取模式
		/// </summary>
		DaysBack = 274,

		/// <summary>
		///   從目前的日期往前幾個Bars的讀取模式
		/// </summary>
		BarsBack = 275,

		/// <summary>
		///   從兩個日期區間內為範圍的讀取模式
		/// </summary>
		FromTo = 276
	}
}