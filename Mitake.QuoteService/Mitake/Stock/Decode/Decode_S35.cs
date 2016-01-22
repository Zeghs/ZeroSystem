using System;
using System.Text;
using Mitake.Stock.Data;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /// <summary>
        ///    證交所公告資訊
        /// </summary>
        internal sealed class Decode_S35 {
		private static MitakeNoticeUtil __cNoticeUtil = new MitakeNoticeUtil();
        
		internal static MitakeNotice Decode(int serial, PacketBuffer buffer) {
                        int iBodySize = 0;
                        string sTime = null, sText = null;

                        int iSerial = 0;
                        byte bNumber = 0, bCount = 0;

                        iBodySize = buffer.Data[3] - 5;

                        buffer.Position = 7; //移動至資料結構(時間欄位)

                        //取得序號
                        iSerial = (buffer[0] << 8) + buffer[1];
                        buffer.Position += 2;

                        bNumber = buffer[0]; //取得封包編號(0 = 標題)
                        ++buffer.Position;

                        if (bNumber == 0) {
                                //取得封包個數
                                bCount = buffer[0];
                                ++buffer.Position; 
                                
                                //取得公告時間
				sTime = string.Format("{0}:{1}", buffer[0].ToString("0#"), buffer[1].ToString("0#"));
                                buffer.Position += 2; 

                                iBodySize -= 6;
                        } else {
                                iBodySize -= 3;
                        }

                        if (iBodySize > 0) {
                                sText = Encoding.GetEncoding("big5").GetString(buffer.Data, buffer.Position, iBodySize);
                        }

			return __cNoticeUtil.Merge(serial, iSerial, bNumber, bCount, sTime, sText); //封包合併(如果合併完成會傳出公告資訊類別, 否則回傳null)
                }
        }
}