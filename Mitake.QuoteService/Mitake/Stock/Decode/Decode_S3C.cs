using System;
using PowerLanguage;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *  個股委買委賣(2~5檔)價量封包([type:1] = ' < '，[3C]16)
         *
         *     <data:N> = {<format:1> <價位:n1> <量:n2>}重覆直到封包結尾... 
         */
        /// <summary>
        ///    個股委買委賣(2~5檔)價量封包
        /// </summary>
        internal sealed class Decode_S3C {
                internal static void Decode(MitakeQuote stock, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        int iSize = 0;
                        
                        float fPeferPrice = 0;
                        byte bNumber = 0, bMode = 0, bFlag = 0, bBuyFlag = 0;

                        iSize = Buffer.Length - 2;
                        Buffer.Position = 7;  //移動至資料結構

			int iCount = MitakeEntrust.MAX_DOM_COUNT;
			for (int i = 1; i < iCount; i++) {
                                stock.委買委賣資訊.Ask[i] = DOMPrice.EMPTY;
				stock.委買委賣資訊.Bid[i] = DOMPrice.EMPTY;
			}

                        do {
                                //取得價量旗標
                                bFlag = Buffer[0];
                                ++Buffer.Position;

                                bNumber = BitConvert.GetValue(bFlag, 0, 3);
                                ++bNumber;

                                //買賣型態(0=委買量  1=委賣量)
                                bBuyFlag = BitConvert.GetValue(bFlag, 3, 1);

				double dPrice = 0, dVolume = 0;
				if (bBuyFlag == 0) {
                                        //取得委買價模式
                                        bMode = BitConvert.GetValue(bFlag, 6, 2);
                                        dPrice = Price.GetPrice(bMode, Buffer, ref fPeferPrice);

                                        //取得委買量模式
                                        bMode = BitConvert.GetValue(bFlag, 4, 2);
                                        dVolume = Volumn.GetVolumn(bMode, Buffer);

					if (bNumber < MitakeEntrust.MAX_DOM_COUNT) {
						stock.委買委賣資訊.Bid[bNumber] = new DOMPrice(dPrice, dVolume);
					}
                                } else {
                                        //取得委賣價模式
                                        bMode = BitConvert.GetValue(bFlag, 6, 2);
                                        dPrice = Price.GetPrice(bMode, Buffer, ref fPeferPrice);

                                        //取得委賣量模式
                                        bMode = BitConvert.GetValue(bFlag, 4, 2);
                                        dVolume = Volumn.GetVolumn(bMode, Buffer);

					if (bNumber < MitakeEntrust.MAX_DOM_COUNT) {
						stock.委買委賣資訊.Ask[bNumber] = new DOMPrice(dPrice, dVolume);
					}
				}
                        } while (Buffer.Position < iSize); //End While
                        
                        ++stock.UpdateCount;
                }
        }
}