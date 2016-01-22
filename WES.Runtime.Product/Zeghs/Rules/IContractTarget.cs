namespace Zeghs.Rules {
	/// <summary>
	///   合約的履約標的介面(適用選擇權)
	/// </summary>
	public interface IContractTarget {
		/// <summary>
		///    取得(合約)履約標的商品代號
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:履約標的商品代號</returns>
		string GetContractTarget(string symbolId);
	}
}