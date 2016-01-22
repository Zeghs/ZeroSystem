using System;
using Zeghs.Products;

namespace Zeghs.Rules {
	/// <summary>
	///   合約規則介面
	/// </summary>
	public interface IContractTime {
		/// <summary>
		///   由商品代號取得合約時間代號索引
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值: 合約時間代號索引</returns>
		int GetContractIndex(string symbolId);

		/// <summary>
		///   取得目標日期的合約時間
		/// </summary>
		/// <param name="date">目標日期</param>
		/// <param name="index">合約時間代號索引,會根據代號索引修正目標日期並取得最後合約時間</param>
		/// <returns>返回值:ContractTime類別</returns>
		ContractTime GetContractTime(DateTime date, int index = 0);

		/// <summary>
		///   更新合約時間
		/// </summary>
		/// <param name="date">基準合約時間(以此時間為基準更新合約到期日)</param>
		void UpdateContractTime(DateTime date);
	}
}