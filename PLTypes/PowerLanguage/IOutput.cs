namespace PowerLanguage {
	/// <summary>
	///   輸出訊息介面
	/// </summary>
	public interface IOutput {
		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">bool型別</param>
		void Write(bool value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">char型別</param>
		void Write(char value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		void Write(char[] buffer);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">decimal型別</param>
		void Write(decimal value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">double型別</param>
		void Write(double value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">float型別</param>
		void Write(float value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">int型別</param>
		void Write(int value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">long型別</param>
		void Write(long value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">object型別</param>
		void Write(object value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">string型別</param>
		void Write(string value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">uint型別</param>
		void Write(uint value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">ulong型別</param>
		void Write(ulong value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="format">格式化表示字串</param>
		/// <param name="arg">參數陣列</param>
		void Write(string format, params object[] arg);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		/// <param name="index">起始位置</param>
		/// <param name="count">個數</param>
		void Write(char[] buffer, int index, int count);

		/// <summary>
		///   寫入資料
		/// </summary>
		void WriteLine();

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">bool型別</param>
		void WriteLine(bool value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">char型別</param>
		void WriteLine(char value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		void WriteLine(char[] buffer);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">decimal型別</param>
		void WriteLine(decimal value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">double型別</param>
		void WriteLine(double value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">float型別</param>
		void WriteLine(float value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">int型別</param>
		void WriteLine(int value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">long型別</param>
		void WriteLine(long value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">object型別</param>
		void WriteLine(object value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">string型別</param>
		void WriteLine(string value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">uint型別</param>
		void WriteLine(uint value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="value">ulong型別</param>
		void WriteLine(ulong value);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="format">格式化表示字串</param>
		/// <param name="arg">參數陣列</param>
		void WriteLine(string format, params object[] arg);

		/// <summary>
		///   寫入資料
		/// </summary>
		/// <param name="buffer">char陣列</param>
		/// <param name="index">起始位置</param>
		/// <param name="count">個數</param>
		void WriteLine(char[] buffer, int index, int count);
	}
}