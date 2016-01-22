using Mitake.Sockets.Data;

namespace Mitake.Stock.Util {
        /// <summary>
        ///   價位格式轉換類別
        /// </summary>
	internal sealed class Price {
                /// <summary>
                ///    解碼價格資訊函式
                /// </summary>
                /// <param name="type">價格類型(請參閱資料格式)</param>
                /// <param name="Buffer">ZBuffer類別</param>
                /// <param name="ReferPrice">參考價格</param>
                /// <returns>傳回值：股票價格</returns>
		internal static float GetPrice(int type, PacketBuffer Buffer, ref float ReferPrice) {
                        double dPrice = 0;
                        switch (type) {
                                case 0:
                                        if ((Buffer[0] & 0x80) == 0) {
                                                dPrice = (Buffer[0] * 0.01);
                                        } else {
                                                dPrice = ((Buffer[0] & 0x7f) * 0.01);
                                                dPrice *= -1;
                                        }
                                        dPrice = ReferPrice + dPrice;
                                        ++Buffer.Position;
                                        break;
                                case 1:
                                        dPrice = ((Buffer[0] << 8) + Buffer[1]);
                                        dPrice += (Buffer[2] * 0.01);
                                        Buffer.Position += 3;
                                        break;
                                case 2:
                                        if ((Buffer[0] & 0x80) == 0) {
                                                dPrice = Buffer[0];
                                        } else {
                                                dPrice = (Buffer[0] & 0x7f);
                                                dPrice *= -1;
                                        }
                                        dPrice = ReferPrice + dPrice;
                                        ++Buffer.Position;
                                        break;
                                case 3:
                                        dPrice = Buffer[0];
                                        if ((Buffer[1] & 0x80) == 0) {
                                                dPrice += (Buffer[1] * 0.01);
                                        } else {
                                                dPrice += ((Buffer[1] & 0x7f) * 0.01);
                                                dPrice *= -1;
                                        }
                                        dPrice = ReferPrice + dPrice;
                                        Buffer.Position += 2;
                                        break;
                        }

                        float fPrice = (float)System.Math.Round(dPrice, 2);
                        if (ReferPrice == 0) {
                                ReferPrice = fPrice;
                        }
                        return fPrice;
                }
                
                /// <summary>
                ///     編碼價格資訊函式
                /// </summary>
                /// <param name="Buffer">緩衝區</param>
                /// <param name="Price">價格</param>
                /// <param name="ReferPrice">參考價</param>
                /// <param name="PriceFlag">價格旗標</param>
                /// <param name="BitIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
		internal static void SetPrice(PacketBuffer Buffer, float Price, float ReferPrice, ref byte PriceFlag, int BitIndex) {
                        int iValue = 0;
                        if (ReferPrice == 0 || System.Math.Abs(Price - ReferPrice) > 255) {
                                iValue = decimal.ToInt32(new decimal(Price) * 100);
                        } else {
                                iValue = decimal.ToInt32((new decimal(Price) * 100) - (new decimal(ReferPrice) * 100));
                        }
                        
                        int iNegative = (((iValue & 0x80000000) == 0) ? 0x00 : 0x80);
                        if (iNegative == 0x80) {   //如果是負數
                                iValue = ~iValue;  //做2的補數(變成正數)
                                ++iValue;
                        }
                        
                        int iInteger = iValue / 100;
                        int iDot = iValue % 100;
                        if (iInteger == 0 && iDot > 0) {
                                Buffer.Data[Buffer.Position++] = (byte)(iDot | iNegative);
                                PriceFlag = BitConvert.SetValue(PriceFlag, BitIndex, 0);
                        } else if (iInteger < 128 && iDot == 0) {
                                Buffer.Data[Buffer.Position++] = (byte)(iInteger | iNegative);
                                PriceFlag = BitConvert.SetValue(PriceFlag, BitIndex, 2);
                        } else if (iInteger < 256) {
                                Buffer.Data[Buffer.Position++] = (byte)iInteger;
                                Buffer.Data[Buffer.Position++] = (byte)(iDot | iNegative);
                                PriceFlag = BitConvert.SetValue(PriceFlag, BitIndex, 3);
                        } else {
                                Buffer.Data[Buffer.Position++] = (byte)(iInteger >> 8);
                                Buffer.Data[Buffer.Position++] = (byte)(iInteger & 0xff);
                                Buffer.Data[Buffer.Position++] = (byte)iDot;
                                PriceFlag = BitConvert.SetValue(PriceFlag, BitIndex, 1);
                        }
                }
        }
}