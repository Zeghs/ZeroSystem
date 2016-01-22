namespace PowerLanguage {
	/// <summary>
	///   資料陣列儲存介面
	/// </summary>
	/// <typeparam name="T">資料型別</typeparam>
	public interface ISeries<T> {
		/// <summary>
		///   [取得] 目前資料
		/// </summary>
		T Value {
			get;
		}

		/// <summary>
		///   [取得] 目前或是之前的資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		T this[int barsAgo] {
			get;
		}
	}
}