namespace PowerLanguage {
	/// <summary>
	///   資料請求結構
	/// </summary>
	public struct InstrumentDataRequest {
		/// <summary>
		///   即時資料報價來源
		/// </summary>
		public string DataFeed;
		
		/// <summary>
		///   證交所簡稱
		/// </summary>
		public string Exchange;
	
		/// <summary>
		///   資料請求結構
		/// </summary>
		public DataRequest Range;
		
		/// <summary>
		///   週期定義結構
		/// </summary>
		public Resolution Resolution;

		/// <summary>
		///   商品代號
		/// </summary>
		public string Symbol;
	}
}