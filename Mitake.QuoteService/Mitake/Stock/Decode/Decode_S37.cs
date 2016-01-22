using System;
using System.Collections.Generic;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
        /*
         *  個股計算及補價資訊：(<stockid:2>=1~N)
         *  <data:N> = {<subId:1> <值:n1>}重覆直到封包結尾...
         */
        internal sealed class Decode_S37 {
                /// <summary>
                ///   個股計算及補價資訊
                /// </summary>
                internal static void Decode(int StockId, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        switch (StockId) {
                                case 9998:  //大盤資料
                                case 9999:  //大盤資料
                                case 10000: //大盤漲跌幅資訊
                                case 10002:
                                        Decoder(StockId, MitakeStorage.Storage.GetIndex(9999), Buffer);
                                        break;
                                default:   //個股資料
                                        Decoder(MitakeStorage.Storage.GetQuote(StockId), Buffer);
                                        break;
                        }
                }

                private static void Decoder(MitakeQuote stock, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        float fReferPrice = 0;
                        byte bMode = 0, bModule = 0, bPVFlag = 0, bFlag = 0;

                        int iSize = Buffer.Length - 2;
                        Buffer.Position = 7;

                        do {
                                bFlag = Buffer[0];  //移動至資料結構
                                ++Buffer.Position;

                                //取得(價/量)旗標 0=量  1=價
                                bPVFlag = BitConvert.GetValue(bFlag, 5, 1);

                                //取得參考價或是成交量模式
                                bMode = BitConvert.GetValue(bFlag, 6, 2);
                                bModule = BitConvert.GetValue(bFlag, 0, 6);

                                switch (bModule) {
                                        case 2: //累計買量
                                                stock.累計買量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 3: //累計賣量
                                                stock.累計賣量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 4: //即時單筆買進巨量
                                                stock.單筆買進巨量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 5: //即時單筆賣出巨量
                                                stock.單筆賣出巨量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 6: //(即時)累計買進巨量
                                                stock.累計買進巨量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 7: //(即時)累計賣出巨量
                                                stock.累計賣出巨量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 8: //即時)量幅，小數兩位(每3筆送一次)
                                                stock.即時量幅 = (float)(Volumn.GetVolumn(bMode, Buffer) * 0.01);
                                                break;
                                        case 9: //(即時)買氣，0~100，賣氣=100－買氣(每3筆送一次)
                                                stock.買進量百分比 = (byte)Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 15: //昨日總成交量
                                                stock.昨日總成交量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 17: //買進成交筆數(股票與選擇權共用)
                                                stock.買進成交筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 18: //賣出成交筆數(股票與選擇權共用)
                                                stock.賣出成交筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 19: //總成交合約數(股票與選擇權共用)
                                                stock.總成交合約數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 20: //買進累計委託筆數
                                                stock.買進累計委託筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 21: //買進累計委託合約量
                                                stock.買進累計委託合約量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 22: //賣出累計委託筆數
                                                stock.賣出累計委託筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 23: //賣出累計委託合約量
                                                stock.賣出累計委託合約量 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 24: //未平倉合約數(收盤後)
                                                stock.未平倉合約數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 25: //選擇權:委託買進總口數
                                                stock.委託買進總口數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 26: //選擇權:委託賣出總口數
                                                stock.委託賣出總口數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 27: //選擇權:總成交筆數 
                                                stock.總成交筆數 = Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                        case 32: //(即時)均價(每10筆送一次)
                                                stock.即時均價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 40: //昨日最高價
                                                stock.昨日最高價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 41: //昨日最低價
                                                stock.昨日最低價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 42: //今日開盤價
                                                stock.Open = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 43: //今日最高價
                                                stock.High = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 44: //今日最低價
                                                stock.Low = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 45: //今日收盤價
                                                stock.Close = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        case 54: //結算價(收盤後)
                                                stock.結算價 = Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                        default: //其他屬於盤後資料(已經不計入範圍內)
                                                if (bPVFlag == 0)
                                                        Volumn.GetVolumn(bMode, Buffer);
                                                else
                                                        Price.GetPrice(bMode, Buffer, ref fReferPrice);
                                                break;
                                }//end switch
                        } while (Buffer.Position < iSize);//end while

                        ++stock.UpdateCount;
                }

                /*
                 *  指數計算及補價資訊：(<stockid:2>=999x...)
                 *  <data:N> = {<subId:1> <值:n1>}重覆直到封包結尾...
                 */
                /// <summary>
                ///    大盤指數計算及補價資訊
                /// </summary>
                private static void Decoder(int StockId, MitakeIndex index, Mitake.Sockets.Data.PacketBuffer Buffer) {
                        uint uValue = 0;
                        bool bNegative = false;
                        byte bMode = 0, bFlag = 0, bModule = 0;

                        int iSize = Buffer.Length - 2;
                        Buffer.Position = 7;

                        do {
                                //取得subId旗標
                                bFlag = Buffer[0];
                                ++Buffer.Position;

                                //取得量價格式
                                bMode = BitConvert.GetValue(bFlag, 6, 2);
                                bModule = BitConvert.GetValue(bFlag, 0, 6);

                                switch (StockId) {
                                        case 9999:
                                                uValue = Volumn.GetVolumn(bMode, Buffer);

                                                Decoder_Index_9999(index, bModule, uValue);
                                                break;
                                        case 10000:
                                                if ((Buffer[0] & 0x80) == 0) {
                                                        bNegative = false;                     //正號
                                                } else {
                                                        bNegative = true;                      //負號
                                                        Buffer.Data[Buffer.Position] &= 0x7f;  //取消負號旗標
                                                }

                                                uValue = Volumn.GetVolumn(bMode, Buffer);

                                                Decoder_Index_10000(index, bModule, uValue, bNegative);
                                                break;
                                        default:
                                                Volumn.GetVolumn(bMode, Buffer);
                                                break;
                                }
                        } while (Buffer.Position < iSize);//end while

                        ++index.UpdateCount;
                }

                //大盤指數計算與補價資訊(StockId = 9999)
                private static void Decoder_Index_9999(MitakeIndex index, byte Module, uint value) {
                        switch (Module) {
                                case 0: //大盤綜合買氣(上市股票最近1000筆)
                                        index.大盤綜合買氣 = value;
                                        break;
                                case 30: //大盤買賣比 (委買張/委賣張)
                                        index.大盤買賣比 = (float)(value * 0.01);
                                        break;
                                default: //新編各類股買氣(該類股最近500筆，紡織、電子則為1000筆)
                                        //判斷是否為null 如果是則創建一個Dictionary
					if (index.類股綜合買氣 == null) {
						index.類股綜合買氣 = new Dictionary<int, uint>();
					}

					if (index.類股綜合買氣.ContainsKey(Module)) {
						index.類股綜合買氣[Module] = value;
					} else {
						index.類股綜合買氣.Add(Module, value);
					}
                                        break;
                        }
                }

                //大盤指數計算與補價資訊(StockId = 10000)
                private static void Decoder_Index_10000(MitakeIndex index, byte Module, uint value, bool Negative) {
                        float fValue = (float)(value * 0.01);

                        if (Negative) {
                                fValue *= -1;  //如果是負數
                        }

                        switch (Module) {
                                case 0:
                                        break;
                                case 1:
                                        index.加權指數漲跌幅 = fValue;
                                        break;
                                case 2:
                                        break;
                                case 3:
                                        index.不含金融漲跌幅 = fValue;
                                        break;
                                case 4:
                                        index.OTC指數價差 = fValue;
                                        break;
                                case 5:
                                        index.OTC指數漲跌幅 = fValue;
                                        break;
                        }
                }
        }
}