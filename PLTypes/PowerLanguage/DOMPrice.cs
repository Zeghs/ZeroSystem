namespace PowerLanguage {
	/// <summary>
	///   委託價量結構
	/// </summary>
	public struct DOMPrice {
		/// <summary>
		///   空的委託價量資訊
		/// </summary>
		public static readonly DOMPrice EMPTY = new DOMPrice(0, 0);

		/// <summary>
		///   委託價格
		/// </summary>
		public readonly double Price;
		
		/// <summary>
		///   委託量
		/// </summary>
		public readonly double Size;
		
		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="price">委託價格</param>
		/// <param name="size">委託量</param>
		public DOMPrice(double price, double size) {
			Price = price;
			Size = size;
		}
	}
}