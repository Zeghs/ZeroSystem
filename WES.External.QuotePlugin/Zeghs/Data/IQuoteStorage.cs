namespace Zeghs.Data {
	/// <summary>
	///   即時報價儲存介面
	/// </summary>
	public interface IQuoteStorage {
		/// <summary>
		///   商品代號是否存在
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:true=存在, false=不存在</returns>
		bool IsSymbolExist(string symbolId);
		
		/// <summary>
		///   取得即時報價資訊
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:IQuote報價資訊介面</returns>
		IQuote GetQuote(string symbolId);
	}
}