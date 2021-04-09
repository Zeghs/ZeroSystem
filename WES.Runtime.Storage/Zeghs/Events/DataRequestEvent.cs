using System;
using PowerLanguage;

namespace Zeghs.Events {
	/// <summary>
	///   資料請求事件類別
	/// </summary>
	public sealed class DataRequestEvent : EventArgs {
		private int __iRate = 0;
		private int __iTotals = 0;

		/// <summary>
		///   [取得/設定] 要請求的資料個數
		/// </summary>
		public int Count {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否已經請求全部資料
		/// </summary>
		public bool IsAlreadyRequestAllData {
			get;
			set;
		}

		/// <summary>
		///   [取得] 與基礎周期的縮放比率
		/// </summary>
		public int Rate {
			get {
				return __iRate;
			}
		}

		/// <summary>
		///   [取得/設定] 資料的區間日期(0=起始日期, 1=終止日期)
		/// </summary>
		public DateTime[] Ranges {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 返回值
		/// </summary>
		public int Result {
			get;
			set;
		}

		/// <summary>
		///   [取得] 資料總個數(欲請求個數 + 目前已下載完後的資料個數)
		/// </summary>
		public int Totals {
			get {
				return __iTotals;
			}
		}

		internal DataRequestEvent(int count, int totals, int rate) {
			__iRate = rate;
			__iTotals = totals;

			this.Result = -1;
			this.Count = count;
			
			DateTime cToday = DateTime.Today;
			this.Ranges = new DateTime[] { cToday, cToday };
		}

		internal DataRequestEvent(InstrumentDataRequest dataRequest) {
			this.Result = -1;
			__iRate = dataRequest.Resolution.Rate;

			DataRequest cRange = dataRequest.Range;
			this.Count = cRange.Count;
			this.Ranges = new DateTime[] { cRange.From, cRange.To };
		}

		internal bool CheckRequest(DataRequest request) {
			bool bRet = true;
			if (this.Count == 0) {  //0表示請求區間使用起始日期與終止日期
				bRet = request.From > this.Ranges[0];  //如果目前已下載資料的請求起始日期 > 準備請求的起始日期(表示準備請求的起始日期更早, 需要再提交請求更早的日期資料)
			} else {
				this.Count *= this.Rate;
				bRet = !(request.To == this.Ranges[1] && this.Count < request.Count);
			}
			return bRet;
		}

		internal DataRequestEvent Clone() {
			return this.MemberwiseClone() as DataRequestEvent;
		}
	}
}