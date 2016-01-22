namespace Zeghs.Products {
	/// <summary>
	///   保證金資訊類別
	/// </summary>
	public sealed class CautionMoney {
		/// <summary>
		///   [取得/設定] 結算保證金金額(結算保證金) @百分比請轉換成小數格式
		/// </summary>
		public double CloseMoney {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 保證金描述
		/// </summary>
		public string Description {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 初始保證金金額(原始保證金) @百分比請轉換成小數格式
		/// </summary>
		public double InitialMoney {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 維持保證金金額(維持保證金) @百分比請轉換成小數格式
		/// </summary>
		public double KeepMoney {
			get;
			set;
		}

		/// <summary>
		///   建立保證金資訊的淺層複本
		/// </summary>
		/// <returns>返回值: CautionMoney 類別</returns>
		internal CautionMoney Clone() {
			return this.MemberwiseClone() as CautionMoney;
		}
	}
}