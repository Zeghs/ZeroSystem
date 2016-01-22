using System;
using PowerLanguage;

namespace Zeghs.Events {
	/// <summary>
	///   SeriesSymbolData 的 Current 改變時所觸發的事件
	/// </summary>
	public sealed class SeriesPositionChangeEvent : EventArgs {
		private int __iCurrent = 0;
		private EBarState __cStatus = EBarState.None;

		/// <summary>
		///   [取得] 目前索引位置
		/// </summary>
		public int Current {
			get {
				return __iCurrent;
			}
		}

		/// <summary>
		///   [取得] Bars 狀態
		/// </summary>
		public EBarState Status {
			get {
				return __cStatus;
			}
		}

		internal SeriesPositionChangeEvent(int current, EBarState status) {
			__iCurrent = current;
			__cStatus = status;
		}
	}
}