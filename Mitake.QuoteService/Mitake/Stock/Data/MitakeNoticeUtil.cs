using System.Text;
using System.Collections.Generic;  

namespace Mitake.Stock.Data {
        /// <summary>
        ///    股市公告存放類別
        /// </summary>
        internal sealed class MitakeNoticeUtil {
		private Dictionary<int, MitakeNotice> __cDict = null;   //公告暫存區

                internal MitakeNoticeUtil() {
                        __cDict = new Dictionary<int, MitakeNotice>(256);
                }

		/// <summary>
		///    加入公告資訊
		/// </summary>
		/// <param name="StockId">公告代號</param>
		/// <param name="SerialNo">公告序號</param>
		/// <param name="Number">公告編號(從0開始編號 0=標題....)</param>
		/// <param name="Count">公告個數</param>
		/// <param name="Time">公告時間</param>
		/// <param name="Text">公告內容</param>
		/// <returns>返回值:null=公告尚未接收完成, 否則返回公告內容</returns>
		internal MitakeNotice Merge(int StockId, int SerialNo, byte Number, byte Count, string Time, string Text) {
			MitakeNotice cNoticeRET = null;
			lock (__cDict) {
				MitakeNotice cNotice = null;
				if (__cDict.ContainsKey(SerialNo)) {
                                        cNotice = __cDict[SerialNo];
                                        if (MergeNotice(cNotice, StockId, Number, Count, Time, Text)) {
                                                __cDict.Remove(SerialNo);
						cNoticeRET = cNotice;
                                        }
                                } else {
                                        cNotice = new MitakeNotice();
                                        if (MergeNotice(cNotice, StockId, Number, Count, Time, Text)) {
						cNoticeRET = cNotice;
					} else {
                                                __cDict.Add(SerialNo, cNotice);
                                        }
                                }
                        }
			return cNoticeRET;
                }

                /// <summary>
                ///    清除公告訊息
                /// </summary>
                internal void Clear() {
                        lock (__cDict) {
				__cDict.Clear();
                        }
                }

                /// <summary>
                ///    合併公告資訊(組合公告資訊封包)
                /// </summary>
                /// <param name="notice">公告類別</param>
                /// <param name="StockId">公告代號</param>
                /// <param name="Number">公告編號(從0開始編號 0=標題....)</param>
                /// <param name="Count">公告總個數</param>
                /// <param name="Time">公告時間</param>
                /// <param name="Text">公告內容</param>
                /// <returns>返回值：true=封包已經接收完畢  false=封包尚未接收完畢</returns>
                private bool MergeNotice(MitakeNotice notice, int StockId, byte Number, byte Count, string Time, string Text) {
                        if (Number == 0) {
                                notice.NoticeId = StockId;
                                notice.Time = Time;
                                notice.Title = Text;
                                notice.Count = Count;
                        } else {
                                notice.Content += Text;
                        }

                        if (Number == notice.Count) {
                                return true;
                        } else {
                                return false;
                        }
                }
        }
}