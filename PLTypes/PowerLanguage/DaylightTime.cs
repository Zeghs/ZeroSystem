using System;

namespace PowerLanguage {
	/// <summary>
	///   夏令時間設定類別
	/// </summary>
	public sealed class DaylightTime {
		/// <summary>
		///   [取得/設定] 夏令時間結束日期(請將當地夏令時間自行轉換成UTC時間)
		/// </summary>
		public DateTime EndDate {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 夏令時間開始日期(請將當地夏令時間自行轉換成UTC時間)
		/// </summary>
		public DateTime StartDate {
			get;
			set;
		}

		/// <summary>
		///   建立 DaylightTime 淺層複本
		/// </summary>
		/// <returns>返回值: DaylightTime 類別</returns>
		public DaylightTime Clone() {
			return this.MemberwiseClone() as DaylightTime;
		}
	}
}