namespace PowerLanguage {
	/// <summary>
	///   下單傳送者介面
	/// </summary>
	public interface IOrderSender {
		/*
		  isReverse: 是否已反轉倉位 = 假設目前倉位為 Long 下了一口 Sell 單, 會將倉內的全部多單平倉, 並轉成 Short 
					     如果此變數為 true 會處理其他特殊行為(行為由下單 Plugin 自行決定與實作)
		*/

		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="action">下單進出場動作</param>
		/// <param name="category">下單類型</param>
		/// <param name="limitPrice">限價價格(市價=0)</param>
		/// <param name="lots">下單數量</param>
		/// <param name="isReverse">是否已反轉倉位</param>
		/// <param name="touchPrice">觸發或停損價格</param>
		/// <param name="name">下單註解</param>
		/// <param name="openNextBar">是否開倉在下一根 Bars</param>
		/// <returns>返回值: true=傳送成功, false=取消傳送(可能某些條件不符合或是價格相同)</returns>
		bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false);
	}
}