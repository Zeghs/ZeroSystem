namespace PowerLanguage {
	/// <summary>
	///   下單類型列舉
	/// </summary>
	public enum OrderCategory {
		/// <summary>
		///   市價單
		/// </summary>
		Market = 0,

		/// <summary>
		///   限價單
		/// </summary>
		Limit = 1,

		/// <summary>
		///   停損單
		/// </summary>
		Stop = 2,

		/// <summary>
		///   停損限價單
		/// </summary>
		StopLimit = 3
	}
}