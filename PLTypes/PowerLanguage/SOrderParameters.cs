namespace PowerLanguage {
	/// <summary>
	///   下單參數結構
	/// </summary>
	public struct SOrderParameters {
		/// <summary>
		///   下單進出場動作
		/// </summary>
		public readonly EOrderAction Action;

		/// <summary>
		///   平倉設定類別
		/// </summary>
		public readonly OrderExit ExitTypeInfo;

		/// <summary>
		///   下單合約數量結構
		/// </summary>
		public readonly Contracts Lots;

		/// <summary>
		///   下單名稱
		/// </summary>
		public readonly string Name;

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="action">下單進出場動作</param>
		public SOrderParameters(EOrderAction action) 
			: this(Contracts.Default, string.Empty, action, OrderExit.FromAll) {
		}
		
		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="lots">下單合約數量結構</param>
		/// <param name="action">下單進出場動作</param>
		public SOrderParameters(Contracts lots, EOrderAction action)
			: this(lots, string.Empty, action, OrderExit.FromAll) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="action">下單進出場動作</param>
		/// <param name="name">下單名稱</param>
		public SOrderParameters(EOrderAction action, string name)
			: this(Contracts.Default, name, action, OrderExit.FromAll) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="lots">下單合約數量結構</param>
		/// <param name="action">下單進出場動作</param>
		/// <param name="exitInfo">平倉設定類別</param>
		public SOrderParameters(Contracts lots, EOrderAction action, OrderExit exitInfo)
			: this(lots, string.Empty, action, exitInfo) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="lots">下單合約數量結構</param>
		/// <param name="name">下單名稱</param>
		/// <param name="action">下單進出場動作</param>
		public SOrderParameters(Contracts lots, string name, EOrderAction action)
			: this(lots, name, action, OrderExit.FromAll) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="lots">下單合約數量結構</param>
		/// <param name="name">下單名稱</param>
		/// <param name="action">下單進出場動作</param>
		/// <param name="exitInfo">平倉設定類別</param>
		public SOrderParameters(Contracts lots, string name, EOrderAction action, OrderExit exitInfo) {
			this.Lots = lots;
			this.Name = name;
			this.Action = action;
			this.ExitTypeInfo = exitInfo;
		}
	}
}