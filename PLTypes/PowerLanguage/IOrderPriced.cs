﻿namespace PowerLanguage {
	/// <summary>
	///   價格單交易介面
	/// </summary>
	public interface IOrderPriced : IOrderCancel, IOrderObject {
		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(double price);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(double price, int numLots);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name, double price);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name, double price, int numLots);
	}
}