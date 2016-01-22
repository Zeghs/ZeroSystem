using System.Text;
using Mitake.Stock.Data;

namespace Mitake.Stock.Decode {
        /*
         *  即時訊息(盤中即時小訊息)
         *   [data:N] = [0b][03][f0][Length:2][NoticeId:2][等級:1][時間:2][標題長度:2][標題資料:n][內文資料:n][0e]
         */
        /// <summary>
        ///   即時訊息(盤中即時小訊息)
        /// </summary>
        internal sealed class Decode_MF0 {
                internal static MitakeNotice Decode(Mitake.Sockets.Data.PacketBuffer Buffer) {
                        MitakeNotice cNotice = new MitakeNotice();
                        cNotice.NoticeId = (Buffer.Data[5] << 8) + Buffer.Data[6];
                        cNotice.Level = Buffer.Data[7];
                        cNotice.Time = Buffer.Data[8].ToString("0#") + ":" + Buffer.Data[9].ToString("0#");

                        int iHeadSize = (Buffer.Data[10] << 8) + Buffer.Data[11];   //取得標題長度
                        cNotice.Title = Encoding.UTF8.GetString(Buffer.Data, 12, iHeadSize);

                        int iBodySize = Buffer.Length - (iHeadSize + 13); //取得內容長度
                        cNotice.Content = Encoding.UTF8.GetString(Buffer.Data, (iHeadSize + 12), iBodySize);

                        return cNotice;
                }
        }
}