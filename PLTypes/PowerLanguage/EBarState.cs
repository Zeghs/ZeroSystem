namespace PowerLanguage {
	/// <summary>
	///   Bars 狀態列舉
	/// </summary>
	public enum EBarState {
		/// <summary>
		///   無狀態
		/// </summary>
		None = -1,
		
		/// <summary>
		///   開盤狀態
		/// </summary>
		Open = 0,
		
		/// <summary>
		///   Bars 尚未收線
		/// </summary>
		Inside = 1,
		
		/// <summary>
		///   Bars 已經收線
		/// </summary>
		Close = 2
	}
}