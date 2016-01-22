namespace Zeghs.Data {
	/// <summary>
	///   資料回補狀態
	/// </summary>
	public enum ComplementStatus {
		/// <summary>
		///   尚未回補資料
		/// </summary>
		NotComplement = 0,

		/// <summary>
		///   等待資料回補中
		/// </summary>
		Complementing = 1,

		/// <summary>
		///   資料回補完畢
		/// </summary>
		Complemented = 2
	}
}