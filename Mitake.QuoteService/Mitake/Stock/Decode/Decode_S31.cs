using System;
using System.Collections.Generic;
using PowerLanguage;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *  個股成交即時資訊：<type:1> = [31]16 [b1]16 序號=2bytes
         *  個股成交即時資訊：<type:1> = [3e]16 [be]16 序號=3bytes
         *
         *   <data:N> = <序號:2> <時間:2> {<format:1> <價位:n1> <量:n2>}重覆直到封包結尾...
         */
        /// <summary>
        ///   個股成交即時資訊
        /// </summary>
        internal sealed class Decode_S31 {
		internal static void Decode(MitakeQuote stock, Mitake.Sockets.Data.PacketBuffer Buffer) {
			DateTime cTime;
			float fReferPrice = 0;
			double dAskP = 0, dAskV = 0, dBidP = 0, dBidV = 0, dPrice = 0, dVolume = 0, dSingle = 0;
			byte bType = 0, bMode = 0, bPType = 0, bVType = 0, bFlag = 0, b類型 = 0, b買賣盤 = 0, b價格類型 = 0;

			int iSerial = 0, iSize = Buffer.Length - 2;
			byte bSType = Buffer.Data[6];

			Buffer.Position = 7;

			//取得序號
			if ((bSType & 0xf) == 1) {
				iSerial = (Buffer[0] << 8) + Buffer[1];
				Buffer.Position = 9;
			} else {
				iSerial = (Buffer[0] << 16) + (Buffer[1] << 8) + Buffer[2];
				Buffer.Position = 10;
			}

			//取得類型
			bType = BitConvert.GetValue(Buffer[0], 7, 1);

			//取得時間
			if ((bSType & 0x80) == 0) {
				cTime = Time.GetTime(Buffer);
			} else {
				cTime = Time.GetOther(Buffer);
			}

			do {
				//取得價量旗標
				bFlag = Buffer[0];
				++Buffer.Position;

				b類型 = bType;  //類型  0=即時  1=盤後

				//判斷是否是單量(1110B)
				if (BitConvert.GetValue(bFlag, 0, 4) == 0xe) {
					//買賣型態(0=無法區分買賣盤量  1=買量  2=賣量)
					bVType = BitConvert.GetValue(bFlag, 6, 2);
					b買賣盤 = bVType;

					bMode = BitConvert.GetValue(bFlag, 4, 2);
					dSingle = Volumn.GetVolumn(bMode, Buffer);
				} else {
					//買賣型態(0=無法確定　1=買盤　2=賣盤　3=委託買賣)
					bVType = BitConvert.GetValue(bFlag, 2, 2);

					//價位型態(0=一般成交價　1=開　2=高　3=低)
					//如果買賣型態為委託買賣(0=委買　1=委賣)
					bPType = BitConvert.GetValue(bFlag, 0, 2);

					//取得成交價或委託價模式
					bMode = BitConvert.GetValue(bFlag, 6, 2);

					switch (bVType) {
						case 3: //委買委賣格式
							if (bPType == 0) {
								dBidP = Price.GetPrice(bMode, Buffer, ref fReferPrice);

								//取得委買量模式
								bMode = BitConvert.GetValue(bFlag, 4, 2);
								dBidV = Volumn.GetVolumn(bMode, Buffer);
							} else {
								dAskP = Price.GetPrice(bMode, Buffer, ref fReferPrice);

								//取得委賣量模式
								bMode = BitConvert.GetValue(bFlag, 4, 2);
								dAskV = Volumn.GetVolumn(bMode, Buffer);
							}
							break;
						default: //一般成交格式
							b價格類型 = bPType;
							dPrice = Price.GetPrice(bMode, Buffer, ref fReferPrice);

							CalculatePrice(stock, iSerial, dPrice);

							bMode = BitConvert.GetValue(bFlag, 4, 2);
							dVolume = Volumn.GetVolumn(bMode, Buffer);
							break;
					}
				}
			} while (Buffer.Position < iSize); //End While

			MitakeQuoteTick cTick = null;
			bool bHave = stock.GetMitakeTick(iSerial, ref cTick);
			MitakeQuoteTick cPrevTick = stock.GetPreviousTick(iSerial);
			if (bHave) {
				cTick.Time = cTime;
				cTick.Ask = new DOMPrice(dAskP, dAskV);
				cTick.Bid = new DOMPrice(dBidP, dBidV);
			} else {
				cTick.類型 = b類型;
				cTick.買賣盤 = b買賣盤;
				cTick.價格類型 = b價格類型;
				cTick.Time = cTime;
				cTick.Price = dPrice;
				cTick.Volume = dVolume;
				cTick.Single = dSingle;
				cTick.Ask = (dAskP == 0 && dAskV == 0 && cPrevTick != null) ? cPrevTick.Ask : new DOMPrice(dAskP, dAskV);
				cTick.Bid = (dBidP == 0 && dBidV == 0 && cPrevTick != null) ? cPrevTick.Bid : new DOMPrice(dBidP, dBidV);

				if (dPrice > 0 && dVolume > 0) {  //如果有價量才計算成交金額與更換最新的即時資訊訊息
					stock.今日總成交額 += cTick.Price * cTick.Single;
					if (iSerial > stock.即時資訊.Serial) {
						stock.今日總成交量 = cTick.Volume;
						stock.即時資訊 = cTick;
					}
				}
			}

			//填入委託價格第一檔
			CalculateEntrust(stock, cTick);

			//計算委託價格買賣盤
			MitakeEntrust.ComparePrice(cTick, cPrevTick);
			++stock.UpdateCount;
		}

                //計算個股(開盤價 最高價 最低價)
                internal static void CalculatePrice(MitakeQuote quote, int serial, double price) {
                        if (price == 0) return;
                        quote.Open = ((serial == 1) ? price : quote.Open);
                        quote.High = ((quote.High == 0) ? price : ((price > quote.High) ? price : quote.High));
                        quote.Low = ((quote.Low == 0) ? price : ((price < quote.Low) ? price : quote.Low));
			quote.Close = price;
                }

                //計算第一檔委買賣
                internal static void CalculateEntrust(MitakeQuote stock, MitakeQuoteTick tick) {
                        //if (Time.ConvertForTotalSeconds(Turnover.Time) <= 50400) {  //14:00以下的委買委賣量 才算有交割資料
			if (tick.Ask.Price > 0 && tick.Ask.Size > 0) {
				stock.委買委賣資訊.Ask[0] = tick.Ask;
			}

			if (tick.Bid.Price > 0 && tick.Bid.Size > 0) {
				stock.委買委賣資訊.Bid[0] = tick.Bid;
			}
                        //}
                }
        }
}  //160行