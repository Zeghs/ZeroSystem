using System;

namespace Zeghs.Events {
	/// <summary>
	///   輸出資料事件
	/// </summary>
	public sealed class OutputDataEvent : EventArgs {
		private bool __bNewLine = false;
		private string __sOutput = null;

		/// <summary>
		///   [取得] 輸出字串資料
		/// </summary>
		public string Data {
			get {
				return __sOutput;
			}
		}

		/// <summary>
		///   [取得] 是否需要添加換行字元
		/// </summary>
		public bool IsNewLine {
			get {
				return __bNewLine;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="output">輸出字串</param>
		/// <param name="isNewLine">是否需要添加換行字元</param>
		public OutputDataEvent(string output, bool isNewLine) {
			__sOutput = output;
			__bNewLine = isNewLine;
		}
	}
}