using System;
using Zeghs.Data;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *   解碼類股成交量與成交金額
         * 
         *    [data:N] = [時間:2]{<Format:1><類股代號:1><成交金額:n><成交量:n>}
         */
        internal sealed class Decode_SB3 {
                internal static void Decode(MitakeIndex index, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        int iSize = 0;
                        uint uValue = 0;
                        byte bMode = 0, bIType = 0, bFlag = 0;
			MitakeIndexTick cTick = null;

                        iSize = Buffer.Length - 2;
                        Buffer.Position = 7;          //移動至資料結構(時間欄位)

			DateTime cTime = Time.GetTime(Buffer); //取得時間
			bool bHave = index.GetMitakeTick(cTime, ref cTick);
			cTick.SetFlag(8);

			if (index.ComplementStatus != ComplementStatus.NotComplement) {
				MitakeIndexTick cPrevTick = index.GetPreviousTick(cTime, 8);
				if (cPrevTick != null) {
					cTick.Clone(cPrevTick);
				}
			}

                        do {
                                //取得價量旗標
                                bFlag = Buffer[0];
                                ++Buffer.Position;

                                //取得指數代號(有加權指數，不含金融 等......)
                                bIType = Buffer[0];
                                ++Buffer.Position;

                                //取得類股成交金額
                                bMode = BitConvert.GetValue(bFlag, 6, 2);
                                uValue = Volumn.GetVolumn(bMode, Buffer);

				cTick.Classifys[bIType].Amount = uValue;

                                //取得類股成交量
                                bMode = BitConvert.GetValue(bFlag, 4, 2);
                                
                                uValue = Volumn.GetVolumn(bMode, Buffer);
                                cTick.Classifys[bIType].Totals = uValue;
                       } while (Buffer.Position < iSize);//End While

                        ++index.UpdateCount;
		}
        }
}