namespace PowerLanguage {
	/// <summary>
	///   選擇權類型
	/// </summary>
	public enum OptionType {
		/// <summary>
		///   選擇權的買權
		/// </summary>
		Call = 0,
		
		/// <summary>
		///   選擇權的賣權
		/// </summary>
		Put = 1,

		/// <summary>
		///   非選擇權類型
		/// </summary>
		None = 255
	}
}