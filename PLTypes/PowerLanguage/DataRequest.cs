using System;

namespace PowerLanguage {
	/// <summary>
	///   資料請求結構
	/// </summary>
	public struct DataRequest {
		/// <summary>
		///   資料請求個數
		/// </summary>
		public int Count;
		
		/// <summary>
		///   日期起始日
		/// </summary>
		public DateTime From;

		/// <summary>
		///   是否已經請求全部資料
		/// </summary>
		public bool IsAlreadyRequestAllData;

		/// <summary>
		///   資料請求類型
		/// </summary>
		public DataRequestType RequestType;

		/// <summary>
		///   日期終止日
		/// </summary>
		public DateTime To;

		/// <summary>
		///   初始化
		/// </summary>
		/// <param name="to">日期終止日</param>
		/// <param name="count">資料個數</param>
		/// <param name="requestType">資料請求類型</param>
		/// <param name="from">日期起始日</param>
		public DataRequest(DateTime to, int count, DataRequestType requestType, DateTime from) {
			this.To = to;
			this.From = from;
			this.Count = count;
			this.RequestType = requestType;
			this.IsAlreadyRequestAllData = false;
		}

		/// <summary>
		///   建立資料請求結構使用以 Bars 數決定資料個數
		/// </summary>
		/// <param name="_to">日期終止日</param>
		/// <param name="_bars">從日期終止日往前的 Bars 個數</param>
		/// <returns>返回值:資料請求結構</returns>
		public static DataRequest CreateBarsBack(DateTime _to, int _bars) {
			return new DataRequest(_to, _bars, DataRequestType.BarsBack, _to);
		}

		/// <summary>
		///   建立資料請求結構使用以天數決定資料個數
		/// </summary>
		/// <param name="_to">日期終止日</param>
		/// <param name="_days">從日期終止日往前的天數</param>
		/// <returns>返回值:資料請求結構</returns>
		public static DataRequest CreateDaysBack(DateTime _to, int _days) {
			DateTime _from = _to.AddDays(-_days);
			return new DataRequest(_to, 0, DataRequestType.DaysBack, _from);
		}

		/// <summary>
		///   建立資料請求結構使用起始日期與終止日期決定資料個數
		/// </summary>
		/// <param name="_from">日期起始日</param>
		/// <param name="_to">日期終止日</param>
		/// <returns>返回值:資料請求結構</returns>
		public static DataRequest CreateFromTo(DateTime _from, DateTime _to) {
			return new DataRequest(_to, 0, DataRequestType.FromTo, _from);
		}
	}
}