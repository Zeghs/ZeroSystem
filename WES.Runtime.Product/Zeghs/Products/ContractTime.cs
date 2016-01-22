using System;

namespace Zeghs.Products {
	/// <summary>
	///   合約時間
	/// </summary>
	public sealed class ContractTime {
		/// <summary>
		///   [取得/設定] 合約月份
		/// </summary>
		public int ContractMonth {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 合約年份
		/// </summary>
		public int ContractYear {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Id值
		/// </summary>
		public int Id {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 最後交割日(到期日)
		/// </summary>
		public DateTime MaturityDate {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		public ContractTime() {
			this.Id = -1;
		}
	}
}