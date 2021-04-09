namespace Zeghs.IO {
	/// <summary>
	///   歷史資料裝置建立者介面
	/// </summary>
	public interface IDeviceCreator {
		/// <summary>
		///   建立歷史資料過檔裝置的執行個體
		/// </summary>
		/// <returns>抽象裝置基底類別</returns>
		AbstractDevice Create();
	}
}