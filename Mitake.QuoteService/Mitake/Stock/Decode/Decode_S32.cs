using System;
using PowerLanguage;
using Zeghs.Data;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *   大盤即時封包 (<stockid:2> = 9999：上市，9998：上櫃)
         *   指數資訊：[type:1] = [32]16
         *   [data:N] = [時間:2] {[指數id:1] [指數值:n1]}重覆直到封包結尾...
         */
        /// <summary>
        ///   大盤即時封包
        /// </summary>
        internal sealed class Decode_S32 {
		internal static void Decode(MitakeIndex index, Mitake.Sockets.Data.PacketBuffer Buffer) {
			float fIndex = 0;
                        byte bMode = 0, bIType = 0, bFlag = 0;
			MitakeIndexTick cTick = null;

                        int iSize = Buffer.Length - 2;
                        Buffer.Position = 7;          //移動至資料結構(時間欄位)

			DateTime cTime = Time.GetTime(Buffer); //取得時間
			bool bHave = index.GetMitakeTick(cTime, ref cTick);
			cTick.SetFlag(1);

			if (index.ComplementStatus != ComplementStatus.NotComplement) {
				MitakeIndexTick cPrevTick = index.GetPreviousTick(cTime, 1);
				if (cPrevTick != null) {
					cTick.Clone(cPrevTick);
				}
			}

			do {
				//取得價量旗標
				bFlag = Buffer[0];
				++Buffer.Position;

				//取得指數大小(1~4 Byte 不固定)
				bMode = BitConvert.GetValue(bFlag, 6, 2);

				//取得指數代號(有加權指數，不含金融 等......)
				bIType = BitConvert.GetValue(bFlag, 0, 6);
				fIndex = (float) (Volumn.GetVolumn(bMode, Buffer) * 0.01);

				if (Time.ConvertForTotalSeconds(cTime) == 32400) { //09:00 開盤會送出昨日收盤價
					index.ReferPrices[bIType] = fIndex;  //昨天收盤指數
				} else {
					if (fIndex > 0) {
						cTick.Classifys[bIType].IndexValue = fIndex;
						
						switch (bIType) {
							case 0: //加權指數
								cTick.Ask = new DOMPrice(fIndex, cTick.Ask.Size);
								cTick.Bid = new DOMPrice(fIndex, cTick.Bid.Size);
								CalculatePrice(index, cTime, fIndex);

								if (cTime >= index.即時資訊.Time) {
									index.Close = fIndex;

									if (index.Serial == 9999) {
										index.加權指數價差 = fIndex - index.ReferPrices[0];
									} else if (index.Serial == 9998) {
										index.OTC指數價差 = fIndex - index.ReferPrices[0];
									}
								}
								break;
							case 9: //不含金融
								if (index.Serial == 9999) {
									if (cTime >= index.即時資訊.Time) {
										index.不含金融價差 = fIndex - index.ReferPrices[9];
									}
								}
								break;
							default:
								break;
						}
					}
				}
			} while (Buffer.Position < iSize);//End While

			if (cTime > index.即時資訊.Time) {
			        index.即時資訊 = cTick;
			}
			++index.UpdateCount;
                }
		
		//計算個股(開盤價 最高價 最低價)
		private static void CalculatePrice(MitakeIndex index, DateTime time, double price) {
			if (price > 0) {
				if (index.Open == 0) {
					if (Time.ConvertForTotalSeconds(time) == 32405) {  //9:00:05 才是真的開盤價格
						index.Open = price;
					}
				}
				
				index.High = ((index.High == 0) ? price : ((price > index.High) ? price : index.High));
				index.Low = ((index.Low == 0) ? price : ((price < index.Low) ? price : index.Low));
			}
		}
	}
}