namespace PowerLanguage {
	/// <summary>
	///   繪製描述來源列舉
	/// </summary>
	public enum EDrawingSource {
		/// <summary>
		///   由當前的腳本所創建繪製物件
		/// </summary>
		CurrentTech = 1,
		
		/// <summary>
		///   不是由當前腳本或手動創建的繪製物件
		/// </summary>
		NotCurrentTechOrManual = 2,

		/// <summary>
		///   腳本或手動創建的所有繪製物件
		/// </summary>
		AnyTechOrManual = 3,

		/// <summary>
		///   由當前腳本或手動創建的繪製物件
		/// </summary>
		CurrentTechOrManual = 4,

		/// <summary>
		///   不是由當前的腳本所創建繪製物件
		/// </summary>
		NotCurrentTech = 5,

		/// <summary>
		///   腳本所創建的所有繪製物件
		/// </summary>
		AnyTech = 6,
		
		/// <summary>
		///   手動所創建的所有繪製物件
		/// </summary>
		Manual = 7
	}
}