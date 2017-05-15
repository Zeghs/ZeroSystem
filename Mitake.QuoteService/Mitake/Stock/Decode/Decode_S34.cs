using System;
using PowerLanguage;
using Zeghs.Data;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *  委託資訊(上市:9999、上櫃:9998、興櫃:9994)：[type:1] = [34]16
         *   [data:N] = [時間:2] {[委託類別id:1] [數值:n1]}重覆直到封包結尾...
         */
        /// <summary>
        ///   大盤委託資訊
        /// </summary>
        internal sealed class Decode_S34 {
                internal static void Decode(MitakeIndex index, Mitake.Sockets.Data.PacketBuffer Buffer) {
			int iSize = 0;
                        byte bMode = 0, bType = 0, bFlag = 0;
                        MitakeIndexTick cTick = null;

                        iSize = Buffer.Length - 2;
                        Buffer.Position = 7;         //移動至資料結構(時間欄位)

			DateTime cTime = Time.GetTime(Buffer); //取得時間
			bool bHave = index.GetMitakeTick(cTime, ref cTick);
			cTick.SetFlag(4);

			if (index.ComplementStatus != ComplementStatus.NotComplement) {
				MitakeIndexTick cPrevTick = index.GetPreviousTick(cTime, 4);
				if (cPrevTick != null) {
					cTick.Clone(cPrevTick);
				}
			}

                        do {
                                //取得Format旗標
                                bFlag = Buffer[0];
                                ++Buffer.Position;

                                //取得委託張數大小(1~4 Byte 不固定)
                                bMode = BitConvert.GetValue(bFlag, 6, 2);

                                //取得委託代號(有委賣合計張數，委買合計張數 等......)
                                bType = BitConvert.GetValue(bFlag, 0, 6);

                                switch (bType) {
                                        case 0: //委買合計張數
						cTick.Bid = new DOMPrice(cTick.Price, Volumn.GetVolumn(bMode, Buffer));
                                                break;
                                        case 1: //委賣合計張數
						cTick.Ask = new DOMPrice(cTick.Price, Volumn.GetVolumn(bMode, Buffer));
                                                break;
                                        case 2: //委買合計筆數
                                                cTick.委買合計筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 3: //委賣合計筆數
                                                cTick.委賣合計筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 4: //委買總漲停張數
                                                cTick.委買總漲停張數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 5: //委賣總漲停張數
                                                cTick.委賣總漲停張數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 6: //委買總漲停筆數
                                                cTick.委買總漲停筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 7: //委賣總漲停筆數
                                                cTick.委賣總漲停筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 8: //委買總跌停張數
                                                cTick.委買總跌停張數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 9: //委賣總跌停張數
                                                cTick.委賣總跌停張數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 10: //委買總跌停筆數
                                                cTick.委買總跌停筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 11: //委賣總跌停筆數
                                                cTick.委賣總跌停筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        default:
                                                Buffer.Position += (bMode + 1);
                                                break;
                                }//End Switch
                        } while (Buffer.Position < iSize);//End While

			++index.UpdateCount;
                }
        }
}