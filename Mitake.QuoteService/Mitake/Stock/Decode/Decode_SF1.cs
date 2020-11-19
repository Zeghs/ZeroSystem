using PowerLanguage;
using Mitake.Stock.Data;
using Mitake.Stock.Util;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /*
         *   個股最新交易資料(0xf1)
         *   [data:N] = <Format:8><參考價:n><成交價:n><委買:n><委賣:n><總量:n><單量:n><委買量:n><委賣量:n><開盤:n><最高:n><最低:n><昨高:n><昨低:n><昨量:n><未平倉量:n><委買2:n><委買3:n><委買4:n><委買5:n><委賣2:n><委賣3:n><委賣4:n><委賣5:n><委買量2:n><委買量3:n><委買量4:n><委買量5:n><委賣量2:n><委賣量3:n><委賣量4:n><委賣量5:n><即時均價:n>
         */
        internal class Decode_SF1 {
                internal static void Decode(MitakeQuote quote, PacketBuffer Buffer) {
                        byte bFlag = 0;
                        int iFormatIndex = 7;
                        float fReferPrice = 0;
			double[,] dPrices = new double[2,5];
			double[,] dVolumes = new double[2,5];

                        int iSize = Buffer.Length - 1;
                        if (iSize == 0) { //如果長度為0表示此回補已經回補完畢
                                return;
                        }
                        
                        Buffer.Position = 15; //移動至資料結構

			quote.即時資訊.Time = Time.ConvertForDateTime((Buffer[0] << 16) + (Buffer[1] << 8) + Buffer[2]);
			Buffer.Position += 3;

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.ReferPrice = Price.GetPrice(BitConvert.GetValue(bFlag, 6, 2), Buffer, ref fReferPrice);
                        quote.即時資訊.Price = Price.GetPrice(BitConvert.GetValue(bFlag, 4, 2), Buffer, ref fReferPrice);
                        dPrices[0, 0] = Price.GetPrice(BitConvert.GetValue(bFlag, 2, 2), Buffer, ref fReferPrice);
			dPrices[1, 0] = Price.GetPrice(BitConvert.GetValue(bFlag, 0, 2), Buffer, ref fReferPrice);

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.即時資訊.Volume = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 6, 2), Buffer);
                        quote.今日總成交量 = quote.即時資訊.Volume;
                        quote.即時資訊.Single = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
                        dVolumes[0, 0] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer);
			dVolumes[1, 0] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 0, 2), Buffer);

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.Open = Price.GetPrice(BitConvert.GetValue(bFlag, 6, 2), Buffer, ref fReferPrice);
                        quote.High = Price.GetPrice(BitConvert.GetValue(bFlag, 4, 2), Buffer, ref fReferPrice);
                        quote.Low = Price.GetPrice(BitConvert.GetValue(bFlag, 2, 2), Buffer, ref fReferPrice);
                        quote.昨日最高價 = Price.GetPrice(BitConvert.GetValue(bFlag, 0, 2), Buffer, ref fReferPrice);

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.昨日最低價 = Price.GetPrice(BitConvert.GetValue(bFlag, 6, 2), Buffer, ref fReferPrice);
                        quote.昨日總成交量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
                        quote.未平倉合約數 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer);
                        dPrices[0, 1] = Price.GetPrice(BitConvert.GetValue(bFlag, 0, 2), Buffer, ref fReferPrice);

                        bFlag = Buffer.Data[iFormatIndex++];
			dPrices[0, 2] = Price.GetPrice(BitConvert.GetValue(bFlag, 6, 2), Buffer, ref fReferPrice);
                        dPrices[0, 3] = Price.GetPrice(BitConvert.GetValue(bFlag, 4, 2), Buffer, ref fReferPrice);
			dPrices[0, 4] = Price.GetPrice(BitConvert.GetValue(bFlag, 2, 2), Buffer, ref fReferPrice);
			dPrices[1, 1] = Price.GetPrice(BitConvert.GetValue(bFlag, 0, 2), Buffer, ref fReferPrice);

                        bFlag = Buffer.Data[iFormatIndex++];
			dPrices[1, 2] = Price.GetPrice(BitConvert.GetValue(bFlag, 6, 2), Buffer, ref fReferPrice);
			dPrices[1, 3] = Price.GetPrice(BitConvert.GetValue(bFlag, 4, 2), Buffer, ref fReferPrice);
			dPrices[1, 4] = Price.GetPrice(BitConvert.GetValue(bFlag, 2, 2), Buffer, ref fReferPrice);
                        dVolumes[0, 1] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 0, 2), Buffer);

                        bFlag = Buffer.Data[iFormatIndex++];
			dVolumes[0, 2] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 6, 2), Buffer);
			dVolumes[0, 3] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
			dVolumes[0, 4] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer);
			dVolumes[1, 1] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 0, 2), Buffer);

                        bFlag = Buffer.Data[iFormatIndex++];
			dVolumes[1, 2] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 6, 2), Buffer);
			dVolumes[1, 3] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
			dVolumes[1, 4] = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer);
                        quote.即時均價 = Price.GetPrice(BitConvert.GetValue(bFlag, 0, 2), Buffer, ref fReferPrice);

                        iFormatIndex = Buffer.Position;
                        Buffer.Position += 2;

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.累計買量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 6, 2), Buffer);
                        quote.累計賣量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
                        quote.單筆買進巨量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer);
                        quote.單筆賣出巨量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 0, 2), Buffer);

                        bFlag = Buffer.Data[iFormatIndex++];
                        quote.累計買進巨量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 6, 2), Buffer);
                        quote.累計賣出巨量 = Volumn.GetVolumn(BitConvert.GetValue(bFlag, 4, 2), Buffer);
                        quote.即時量幅 = (float)(Volumn.GetVolumn(BitConvert.GetValue(bFlag, 2, 2), Buffer) * 0.01);
                        quote.買進量百分比 = (byte)Volumn.GetVolumn(BitConvert.GetValue(bFlag, 0, 2), Buffer);

			for (int i = 0; i < 5; i++) {
				quote.委買委賣資訊.Bid[i] = new DOMPrice(dPrices[0, i], dVolumes[0, i]);
				quote.委買委賣資訊.Ask[i] = new DOMPrice(dPrices[1, i], dVolumes[1, i]);
			}
			quote.即時資訊.Bid = quote.委買委賣資訊.Bid[0];
			quote.即時資訊.Ask = quote.委買委賣資訊.Ask[0];

			++quote.UpdateCount;
		}
        }
}