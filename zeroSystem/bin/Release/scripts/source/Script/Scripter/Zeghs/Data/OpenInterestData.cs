using System;
using System.Collections.Generic;
using Zeghs.Utils;

namespace Zeghs.Data {
	/// <summary>
	///   台灣指數選擇權未平倉資料
	/// </summary>
	public sealed class OpenInterestData {
		private DateTime __cDate = DateTime.MinValue;
		private Dictionary<string, int> __cInterest = null;

		/// <summary>
		///   [取得] 當日未平倉日期
		/// </summary>
		public DateTime Date {
			get {
				return __cDate;
			}
		}

		public OpenInterestData(DateTime date, ZBuffer buffer) {
			__cDate = date.Date;
			__cInterest = new Dictionary<string, int>(128);

			while (buffer.Position < buffer.Length) {
				__cInterest.Add(buffer.GetString(), buffer.GetInt32());
			}
		}

		/// <summary>
		///   取得台灣指數選擇權未平倉數量
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值: 未平倉數量</returns>
		public int GetOpenInterest(string symbolId) {
			int iInterest = 0;
			__cInterest.TryGetValue(symbolId, out iInterest);

			return iInterest;
		}
	}
}