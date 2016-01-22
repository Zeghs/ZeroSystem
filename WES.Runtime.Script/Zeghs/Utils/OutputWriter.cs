using System;
using System.IO;
using System.Text;
using PowerLanguage;
using Zeghs.Events;

namespace Zeghs.Utils {
	/// <summary>
	///   輸出訊息寫入者類別
	/// </summary>
	public sealed class OutputWriter : TextWriter, IOutput {
		/// <summary>
		///   輸出資料時所觸發的事件
		/// </summary>
		public event EventHandler<OutputDataEvent> onOutputData = null;

		private bool __bDisposed = false;  //Dispose旗標

		/// <summary>
		///   [取得] 字串編碼規則(Default)
		/// </summary>
		public override Encoding Encoding {
			get {
				return Encoding.Default;
			}
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">bool型別</param>
		public override void Write(bool value) {
			OnOutputData(value.ToString());
		}
		
		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">char型別</param>
		public override void Write(char value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		public override void Write(char[] buffer) {
			OnOutputData(new string(buffer));
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">decimal型別</param>
		public override void Write(decimal value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">double型別</param>
		public override void Write(double value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">float型別</param>
		public override void Write(float value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">int型別</param>
		public override void Write(int value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">long型別</param>
		public override void Write(long value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">object型別</param>
		public override void Write(object value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">string型別</param>
		public override void Write(string value) {
			OnOutputData(value);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">uint型別</param>
		public override void Write(uint value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">ulong型別</param>
		public override void Write(ulong value) {
			OnOutputData(value.ToString());
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="format">格式化表示字串</param>
		/// <param name="arg">參數陣列</param>
		public override void Write(string format, params object[] arg) {
			OnOutputData(string.Format(format, arg));
		}
		
		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		/// <param name="index">起始位置</param>
		/// <param name="count">個數</param>
		public override void Write(char[] buffer, int index, int count) {
			OnOutputData(new string(buffer, index, count));
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		public override void WriteLine() {
			OnOutputData(string.Empty, true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">bool型別</param>
		public override void WriteLine(bool value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">char型別</param>
		public override void WriteLine(char value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		public override void WriteLine(char[] buffer) {
			OnOutputData(new string(buffer), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">decimal型別</param>
		public override void WriteLine(decimal value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">double型別</param>
		public override void WriteLine(double value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">float型別</param>
		public override void WriteLine(float value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">int型別</param>
		public override void WriteLine(int value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">long型別</param>
		public override void WriteLine(long value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">object型別</param>
		public override void WriteLine(object value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">string型別</param>
		public override void WriteLine(string value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">uint型別</param>
		public override void WriteLine(uint value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">ulong型別</param>
		public override void WriteLine(ulong value) {
			OnOutputData(value.ToString(), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="format">格式化表示字串</param>
		/// <param name="arg">參數陣列</param>
		public override void WriteLine(string format, params object[] arg) {
			OnOutputData(string.Format(format, arg), true);
		}

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		/// <param name="index">起始位置</param>
		/// <param name="count">個數</param>
		public override void WriteLine(char[] buffer, int index, int count) {
			OnOutputData(new string(buffer, index, count), true);
		}
		
		/// <summary>
		///   釋放 System.IO.TextWriter 使用的 Unmanaged 資源，並選擇性釋放 Managed 資源。
		/// </summary>
		/// <param name="disposing">true，表示釋放 Managed 和 Unmanaged 資源；false，表示只釋放 Unmanaged 資源。</param>
		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					onOutputData = null;

					base.Dispose(disposing);
				}
			}
		}

		private void OnOutputData(string output, bool isNewLine = false) {
			if (onOutputData != null) {
				onOutputData(this, new OutputDataEvent(output, isNewLine));
			}
		}
	}
}