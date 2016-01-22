using System;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /* 漲跌停資訊(上市:9999、上櫃:9998)：[type:1] = ' 6 ' ([36]16)
         *  [data:N] = {[漲跌類別id:1] [家數:n1]}重覆直到封包結尾...
         */
        /// <summary>
        ///    漲跌停資訊(上市:9999、上櫃:9998)
        /// </summary>
        internal sealed class Decode_S36 {
                internal static void Decode(MitakeIndex index, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        int iSize = 0;
                        byte bMode = 0, bType = 0, bFlag = 0;

                        iSize = Buffer.Length - 2;

                        //移動至資料結構(時間欄位)
                        Buffer.Position = 7;

                        do {
                                //取得Format旗標
                                bFlag = Buffer[0];
                                ++Buffer.Position;

                                //取得家數大小(1~4 Byte 不固定)
                                bMode = BitConvert.GetValue(bFlag, 6, 2);

                                //取得家數代號(有持平家數，上漲家數，下跌家數 等......)
                                bType = BitConvert.GetValue(bFlag, 0, 6);

                                switch (bType) {
                                        case 0: //持平家數
                                                index.持平家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 1: //未成交家數
                                                index.未成交家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 2: //上漲家數
                                                index.上漲家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 3: //下跌家數
                                                index.下跌家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 4: //漲停家數
                                                index.漲停家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 5: //跌停家數
                                                index.跌停家數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                }
                        } while (Buffer.Position < iSize);//End While

                        ++index.UpdateCount;
                }
        }
}