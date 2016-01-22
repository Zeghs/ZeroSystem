namespace PowerLanguage {
	/// <summary>
	///   下單參數類別
	/// </summary>
	public sealed class Order {
		/// <summary>
		///   [取得] 下單進出場動作列舉
		/// </summary>
		public EOrderAction Action {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 下單類型
		/// </summary>
		public OrderCategory Category {
			get;
			private set;
		}
	
		/// <summary>
		///   [取得] 下單合約數量
		/// </summary>
		public Contracts Contracts {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 平倉是否為 Entry 類型
		/// </summary>
		public bool IsEntry {
			get {
				return this.OrderExit.EntryID > 0;
			}
		}

		/// <summary>
		///   [取得] 是否為平倉類型
		/// </summary>
		public bool IsExit {
			get {
				return this.Action == EOrderAction.Sell || this.Action == EOrderAction.BuyToCover;
			}
		}
		
		/// <summary>
		///   [取得] 是否為多單類型
		/// </summary>
		public bool IsLong {
			get {
				return this.Action == EOrderAction.Buy;
			}
		}

		/// <summary>
		///   [取得] 是否為空單類型
		/// </summary>
		public bool IsShort {
			get {
				return this.Action == EOrderAction.SellShort;
			}
		}
		
		/// <summary>
		///   [取得] 下單名稱
		/// </summary>
		public string Name {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 是否以市價下單在下一根 Bars 上
		/// </summary>
		public bool OnClose {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 平倉設定類別
		/// </summary>
		public OrderExit OrderExit {
			get;
			private set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">下單名稱</param>
		/// <param name="action">下單進出場動作列舉</param>
		/// <param name="category">下單類型</param>
		/// <param name="lots">下單合約數量</param>
		/// <param name="openNextBar">是否開倉在下一根 Bars</param>
		/// <param name="exitInfo">平倉設定類別</param>
		public Order(string name, EOrderAction action, OrderCategory category, Contracts lots, bool openNextBar, OrderExit exitInfo) {
			this.Name = name;
			this.Action = action;
			this.Category = category;
			this.Contracts = lots;
			this.OnClose = openNextBar;
			this.OrderExit = exitInfo;
		}
	}
}