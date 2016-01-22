using System;

namespace Mitake.Events {
        /// <summary>
        ///    收到時間封包解碼後所觸發的事件
        /// </summary>
        public class TimerEvent {
		private static string[] tradeData = { "尚未開盤", "盤中資料", "盤後資料", "無法辨識" };

		/// <summary>
		///   [取得] 即時資訊報價日
		/// </summary>
		public DateTime QuoteDateTime {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 交易資訊 0=尚未開盤  1=盤中資料  2=盤後資料  3=無法辨識
		/// </summary>
		public byte Trade {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 最後交易日期
		/// </summary>
		public DateTime TradeDate {
			get;
			set;
		}
		
		/// <summary>
                ///     取得證交所時間封包的字串完整格式
                /// </summary>
                /// <returns>返回值：證交所時間封包字串格式</returns>
                public override string ToString() {
			return string.Format("{0}({1})", this.QuoteDateTime.ToString("yyyy/MM/dd HH:mm:ss"), tradeData[Trade]);
                }
        }
}