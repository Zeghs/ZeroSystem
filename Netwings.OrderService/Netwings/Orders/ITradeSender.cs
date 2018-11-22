using PowerLanguage;

namespace Netwings.Orders {
	/// <summary>
	///   交易傳送者介面
	/// </summary>
	internal interface ITradeSender : IOrderSender {
		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="trust">交易訂單資訊</param>
		/// <param name="isCancel">是否要取消此交易訂單(成功委託才能取消訂單)</param>
		/// <returns>返回值: true=成功, false=失敗</returns>
		bool Send(TradeOrder trust, bool isCancel);
	}
}