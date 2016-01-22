namespace PowerLanguage {
	/// <summary>
	///   下單合約數量結構
	/// </summary>
	public struct Contracts {
		/// <summary>
		///   下單合約型態
		/// </summary>
		public enum EContractsType {
			/// <summary>
			///   預設 lots 單位
			/// </summary>
			Default = 0,

			/// <summary>
			///   使用者指定 lots 單位
			/// </summary>
			UserSpecified = 1
		}

		/// <summary>
		///   [取得] 預設合約數量
		/// </summary>
		public static Contracts Default {
			get {
				return new Contracts(EContractsType.Default, 1);
			}
		}

		/// <summary>
		///   [取得] 使用者指定的合約數量
		/// </summary>
		public static Contracts UserSpecified {
			get {
				return new Contracts(EContractsType.UserSpecified, 0);
			}
		}

		/// <summary>
		///   建立使用者指定的合約數量(使用指定的合約數量在預設模式)
		/// </summary>
		/// <param name="num">下單數量</param>
		/// <returns>返回值: Contracts 結構</returns>
		public static Contracts CreateUserSpecified(int num) {
			return new Contracts(EContractsType.Default, num);
		}

		/// <summary>
		///   下單合約預設口數
		/// </summary>
		public readonly int Contract;

		/// <summary>
		///   下單合約型態
		/// </summary>
		public readonly Contracts.EContractsType Type;

		/// <summary>
		///   [取得] 是否為預設合約類型
		/// </summary>
		public bool IsDefault {
			get {
				return Type == EContractsType.Default;
			}
		}

		/// <summary>
		///   [取得] 是否為使用者指定類型
		/// </summary>
		public bool IsUserSpecified {
			get {
				return Type == EContractsType.UserSpecified;
			}
		}

		private Contracts(EContractsType type, int contract) {
			Type = type;
			Contract = contract;
		}
	}
}