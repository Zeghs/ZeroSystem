namespace PowerLanguage {
	/// <summary>
	///   平倉設定類別
	/// </summary>
	public class OrderExit {
		/// <summary>
		///   平倉類型
		/// </summary>
		public enum EExitType {
			/// <summary>
			///   平倉所有單
			/// </summary>
			All = 0,

			/// <summary>
			///   平倉一定數量的所有單(參考 Contract 的設定 lots 單位, 或是由使用者自己指定)
			/// </summary>
			Total = 1,
			
			/// <summary>
			///   平倉使用者指定的 Entry
			/// </summary>
			FromOne = 2
		}

		/// <summary>
		///   [取得] 條目代號
		/// </summary>
		public int EntryID {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 平倉類型
		/// </summary>
		public OrderExit.EExitType ExitType {
			get;
			private set;
		}

		/// <summary>
		///   [取得] All 類型的平倉類別
		/// </summary>
		public static OrderExit FromAll {
			get {
				return new OrderExit(EExitType.All, 0);
			}
		}

		/// <summary>
		///   [取得] 是否為 Total 平倉類型
		/// </summary>
		public bool IsTotal {
			get {
				return this.ExitType == EExitType.Total;
			}
		}

		/// <summary>
		///   [取得] Total 類型的平倉類別
		/// </summary>
		public static OrderExit Total {
			get {
				return new OrderExit(EExitType.Total, 0);
			}
		}

		private OrderExit(EExitType type, int entryId) {
			this.ExitType = type;
			this.EntryID = entryId;
		}

		/// <summary>
		///   從 entry 取得平倉設定類別
		/// </summary>
		/// <param name="entry">IOrderObject 介面</param>
		/// <returns>返回值: OrderExit 類別</returns>
		public static OrderExit FromEntry(IOrderObject entry) {
			return entry.Info.OrderExit;
		}
	}
}