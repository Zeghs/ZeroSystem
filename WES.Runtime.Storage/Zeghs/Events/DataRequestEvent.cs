using System;

namespace Zeghs.Events {
	/// <summary>
	///   資料請求事件類別
	/// </summary>
	internal sealed class DataRequestEvent : EventArgs {
		private int __iRate = 0;
		private int __iPosition = 0;

		/// <summary>
		///   [取得/設定] 要請求的資料個數
		/// </summary>
		internal int Count {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否已經請求全部資料
		/// </summary>
		internal bool IsAlreadyRequestAllData {
			get;
			set;
		}

		/// <summary>
		///   [取得] 使用者移動的索引位置
		/// </summary>
		internal int Position {
			get {
				return __iPosition;
			}
		}

		/// <summary>
		///   [取得] 與基礎周期的縮放比率
		/// </summary>
		internal int Rate {
			get {
				return __iRate;
			}
		}

		/// <summary>
		///   [取得/設定] 資料的區間日期(0=起始日期, 1=終止日期)
		/// </summary>
		internal DateTime[] Ranges {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 返回值
		/// </summary>
		internal int Result {
			get;
			set;
		}

		internal DataRequestEvent(int position, int count, int rate) {
			__iPosition = position;
			__iRate = rate;

			this.Result = -1;
			this.Count = count;
			this.Ranges = new DateTime[2];
		}
	}
}