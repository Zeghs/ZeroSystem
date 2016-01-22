using System;
using Mitake.Events;
using Mitake.Stock.Util;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /* 股票時間封包
         *
         *   時間封包資料格式   
         *    [01] <id:1> [02] <len:1> <time:2> <date:2> <trade:1> {<trade-date:2>}可能無 <checksum:1> [04]
         *
         */
        /// <summary>
        ///    股票時間封包解碼類別
        /// </summary>
        internal sealed class DecodeTime {
                internal static TimerEvent Decode(PacketBuffer buffer, bool isDecode) {
                        if (isDecode) {
				buffer.Position = 0;  //從第0索引開始
				int iLength = buffer[3];  //取得封包長度

				TimerEvent cTimer = new TimerEvent();
                                cTimer.QuoteDateTime = Time.GetDateTime(buffer);
                                
				cTimer.Trade = (byte)(buffer[0] & 0x03);
				++buffer.Position;

				if (iLength == 9) {  //表示有最後交易日期
					cTimer.TradeDate = Time.GetDate(buffer);
				} else {
					cTimer.TradeDate = cTimer.QuoteDateTime.Date;
				}
                                return cTimer;
                        } else {
                                return null;
                        }
                }
        }
}