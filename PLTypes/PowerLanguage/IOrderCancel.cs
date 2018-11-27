namespace PowerLanguage {
	/// <summary>
	///   訂單取消介面(訂單委託後取消命令的實作介面)
	/// </summary>
	public interface IOrderCancel {
		/// <summary>
		///   取消委託中訂單
		/// </summary>
		/// <param name="name">下單名稱(如果 name 為 null 則取消全部委託中的訂單)</param>
		/// <returns>回傳值: 取消委託中訂單的總口數</returns>
		int Cancel(string name = null);
	}
}