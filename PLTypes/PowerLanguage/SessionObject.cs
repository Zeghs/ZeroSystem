using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerLanguage {
	/// <summary>
	///   商品交易時段類別
	/// </summary>
	public sealed class SessionObject {
		/// <summary>
		///   [取得/設定] 收盤星期名稱
		/// </summary>
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public DayOfWeek EndDay {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 收盤時間(@如有日光節約時間則以夏令時間為主要設定)
		/// </summary>
		public TimeSpan EndTime {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 夏令時間設定
		/// </summary>
		public DaylightTime Daylight {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 開盤星期名稱
		/// </summary>
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public DayOfWeek StartDay {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 開盤時間(@如有日光節約時間則以夏令時間為主要設定)
		/// </summary>
		public TimeSpan StartTime {
			get;
			set;
		}

		/// <summary>
		///   建立 SessionObject 淺層複本
		/// </summary>
		/// <returns>返回值: SessionObject 類別</returns>
		public SessionObject Clone() {
			SessionObject cSession = this.MemberwiseClone() as SessionObject;
			
			if (this.Daylight != null) {
				cSession.Daylight = this.Daylight.Clone();
			}
			return cSession;
		}

		/// <summary>
		///   取得收盤時間為日光節約時間(如果沒有設定日光節約時間則回傳原設定值)
		/// </summary>
		/// <returns>返回值:調整後的日光節約時間</returns>
		public TimeSpan GetCloseTimeForDaylight() {
			TimeSpan cTime = this.EndTime;
			
			if (this.Daylight != null) {
				DateTime cDate = DateTime.UtcNow;
				DaylightTime cDaylight = this.Daylight;
				cTime = cTime.Add(TimeSpan.FromHours((cDate >= cDaylight.StartDate && cDate <= cDaylight.EndDate) ? 0 : 1)); //冬令時間多加上一小時
			}
			return cTime;
		}

		/// <summary>
		///   取得開盤時間為日光節約時間(如果沒有設定日光節約時間則回傳原設定值)
		/// </summary>
		/// <returns>返回值:調整後的日光節約時間</returns>
		public TimeSpan GetStartTimeForDaylight() {
			TimeSpan cTime = this.StartTime;
			
			if (this.Daylight != null) {
				DateTime cDate = DateTime.UtcNow;
				DaylightTime cDaylight = this.Daylight;
				cTime = cTime.Add(TimeSpan.FromHours((cDate >= cDaylight.StartDate && cDate <= cDaylight.EndDate) ? 0 : 1)); //冬令時間多加上一小時
			}
			return cTime;
		}
	}
}