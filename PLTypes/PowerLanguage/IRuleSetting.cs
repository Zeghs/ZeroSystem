namespace PowerLanguage {
	/// <summary>
	///   規則設定介面
	/// </summary>
	public interface IRuleSetting {
		/// <summary>
		///   顯示設定表單視窗
		/// </summary>
		/// <returns>返回值: 0=設定完成, 其他=設計者自訂</returns>
		int ShowSetting();
	}
}