using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    公告資料類別
        /// </summary>
        public sealed class MitakeNotice : INotice {
		/// <summary>
		///   公告內容
		/// </summary>
		public string Content {
			get;
			internal set;
		}

		/// <summary>
		///    [取得/設定]公告等級
		/// </summary>
		public int Level {
			get;
			internal set;
		}
		
		/// <summary>
		///   [取得] 公告ID 10000=證交所緊急公告, 9999=上市公告, 9998=上櫃公告, 9997=期交所公告, 9995=選擇權公告, 9994=興櫃公告, 98=時報資訊新聞, 99=中央社新聞, 100~199=投顧訊息(100:股市導航), 200~299=券商訊息, 
		/// </summary>
                public int NoticeId {
                        get;
                        internal set;
                }

                /// <summary>
		///   [取得] 公告發出的時間
                /// </summary>
                public string Time {
                        get;
                        internal set;
                }

                /// <summary>
		///   [取得] 公告標題
                /// </summary>
                public string Title {
                        get;
                        internal set;
                }

                /// <summary>
		///   [取得/設定] 公告總封包個數(不包含標題，標題要算進去還要加一個封包)
                /// </summary>
                internal byte Count {
                        get;
                        set;
                }
        }
}