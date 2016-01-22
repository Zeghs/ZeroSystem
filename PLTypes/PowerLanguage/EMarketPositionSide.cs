namespace PowerLanguage {
	/// <summary>
	///   留倉方向列舉
	/// </summary>
	public enum EMarketPositionSide : int {
		/// <summary>
		///   空方
		/// </summary>
		Short = -1,

		/// <summary>
		///   無留倉
		/// </summary>
		Flat = 0,
		
		/// <summary>
		///   多方
		/// </summary>
		Long = 1
	}
}