﻿namespace PowerLanguage {
	/// <summary>
	///   停損限價單交易介面
	/// </summary>
	public interface IOrderStopLimit : IOrderObject {
		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="stopPrice">停損價格</param>
		/// <param name="limitPrice">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(double stopPrice, double limitPrice);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="stopPrice">停損價格</param>
		/// <param name="limitPrice">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(double stopPrice, double limitPrice, int numLots);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="stopPrice">停損價格</param>
		/// <param name="limitPrice">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name, double stopPrice, double limitPrice);

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="stopPrice">停損價格</param>
		/// <param name="limitPrice">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		bool Send(string new_name, double stopPrice, double limitPrice, int numLots);
	}
}