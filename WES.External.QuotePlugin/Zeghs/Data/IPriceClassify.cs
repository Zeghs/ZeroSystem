namespace Zeghs.Data {
	/// <summary>
	///   分價表資訊類別
	/// </summary>
	public interface IPriceClassify {
		/// <summary>
		///   [取得] 分價價格
		/// </summary>
		double Price {
			get;
		}
		
		/// <summary>
		///   [取得] 分價價格內的統計總量
		/// </summary>
		double Volume {
			get;
		}
	}
}