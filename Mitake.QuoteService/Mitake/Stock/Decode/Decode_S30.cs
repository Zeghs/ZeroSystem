using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *  今日開盤前資訊：[type:1] = '0' (數字，[30])
         *   [data:N] = [format:1][參考價:n1][漲停價:n2][跌停價:n3]
         */
        /// <summary>
        ///   今日開盤前資訊類別
        /// </summary>
        internal sealed class Decode_S30 {
                internal static void Decode(MitakeQuote stock, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        float fReferPrice = 0;
                        byte bMode = 0, bFlag = 0;

                        Buffer.Position = 7;                   //移動至資料結構 
                        bFlag = Buffer.Data[Buffer.Position];
                        ++Buffer.Position;

                        //取得參考價模式
                        bMode = BitConvert.GetValue(bFlag, 6, 2);
                        stock.ReferPrice = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                
                        //取得參考價模式
                        bMode = BitConvert.GetValue(bFlag, 4, 2);
                        stock.漲停價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                
                        //取得參考價模式
                        bMode = BitConvert.GetValue(bFlag, 2, 2);
                        stock.跌停價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);

                        ++stock.UpdateCount;
                }
        }
}