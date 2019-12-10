using System;
using PowerLanguage;
using Mitake.Stock.Data;
using Mitake.Stock.Util;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /*
         *   補個股每筆成交價量封包(<type:1> = [3B]16 [3F]16 [序號:3])
         *   補個股每筆成交價量封包(<type:1> = [BB]16 [BF]16 [序號:3])
         *   
         *   [data:N] = [序號:2] {[時間:2] [format:1] [成交價:n1] [單量:n2] [委買價:n3] [委賣價:n4]}重覆直到封包結尾...
         */
        /// <summary>
        ///   補個股每筆成交價量
        /// </summary>
        internal sealed class Decode_S3B {
                internal static void Decode(MitakeQuote stock, PacketBuffer buffer) {
			DateTime cTime;
                        bool isHave = false;
			int iSerial = 0;
			float fReferPrice = 0;
                        byte bType = 0, bMode = 0, bVType = 0, bFlag = 0;
                        MitakeQuoteTick cTick = null;

                        int iSize = buffer.Length - 2;
                        byte bSType = buffer.Data[6];

			buffer.Position = 7;

                        if ((bSType & 0xf) == 11) {
                                iSerial = ((buffer[0] << 8) + buffer[1]);
                                buffer.Position = 9;
                        } else {
                                iSerial = (buffer[0] << 16) + (buffer[1] << 8) + buffer[2];
                                buffer.Position = 10;
                        }

                        do {
                                //取得類型
                                bType = BitConvert.GetValue(buffer[0], 7, 1);

                                //取得時間
                                if ((bSType & 0x80) == 0) {
                                        cTime = Time.GetTime(buffer);
                                } else {
                                        cTime = Time.GetOther(buffer);
                                }

                                isHave = stock.GetMitakeTick(iSerial, ref cTick);
                                cTick.Time = cTime;

                                bFlag = buffer[0]; //取得價量旗標
                                ++buffer.Position;

                                cTick.類型 = bType;  //類型  0=即時  1=盤後(此封包為盤後封包)

                                //買賣型態(0=無法區分  1=買盤量  2=賣盤量)
                                bVType = BitConvert.GetValue(bFlag, 2, 2);
                                cTick.買賣盤 = bVType;

                                //取得成交價模式
                                bMode = BitConvert.GetValue(bFlag, 6, 2);
                                cTick.Price = Price.GetPrice(bMode, buffer, ref fReferPrice);
                                Decode_S31.CalculatePrice(stock, iSerial, cTick.Price); //計算 開盤 最高 最低

                                //取得單量模式
                                bMode = BitConvert.GetValue(bFlag, 4, 2);
                                cTick.Single = Volumn.GetVolumn(bMode, buffer);

                                //取得委買價
				cTick.Bid = new DOMPrice(cTick.Price - EntrustPrice(BitConvert.GetValue(bFlag, 1, 1), buffer), cTick.Bid.Size);

                                //取得委賣價
				cTick.Ask = new DOMPrice(cTick.Price + EntrustPrice(BitConvert.GetValue(bFlag, 0, 1), buffer), cTick.Ask.Size);

				//修正總成交量(伺服器會傳輸回補修正封包, 需要重新修正總成交量)
				MitakeQuoteTick cPrevTick = stock.GetPreviousTick(iSerial);
				cTick.Volume = ((cPrevTick == null) ? 0d : cPrevTick.Volume) + cTick.Single;
				
				if (!isHave) {
                                        stock.今日總成交額 += cTick.Price * cTick.Single;
                                }
                                ++iSerial;
                        } while (buffer.Position < iSize);//End While
		}

                /// <summary>
                ///   取得委託價函式
                /// </summary>
                /// <param name="type">委託價格式(0=1Byte  1=2Byte)</param>
                /// <param name="buffer">即時資料Buffer</param>
                /// <returns>返回值:委託價格</returns>
                private static float EntrustPrice(byte type, PacketBuffer buffer) {
                        double dPrice = 0;
                        if (type == 0) {
                                if (buffer[0] < 0xff) {     //0xff=無價位 
                                        dPrice = buffer[0] * 0.01;
                                }
                                ++buffer.Position;
                        } else {
                                dPrice = ((buffer[0] << 8) + buffer[1]) * 0.01;
                                buffer.Position += 2;
                        }
                        return (float)dPrice; 
                }
        }
}