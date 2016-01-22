using System;

namespace Zeghs.Events {
	/// <summary>
	///   當輸入的索引位置超出 Series 範圍時會觸發此事件做資料要求處理
	/// </summary>
	public sealed class SeriesRequestEvent : EventArgs {
		private int __iPosition = 0;

		/// <summary>
		///   [取得] 使用者輸入的索引位置(Current + barsAgo)
		/// </summary>
		public int Position {
			get {
				return __iPosition;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="position">使用者輸入的索引位置</param>
		public SeriesRequestEvent(int position) {
			__iPosition = position;
		}
	}
}