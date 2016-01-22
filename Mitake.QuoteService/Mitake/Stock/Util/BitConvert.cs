namespace Mitake.Stock.Util {
        /// <summary>
        ///   位元轉換類別
        /// </summary>
        internal sealed class BitConvert {
                /// <summary>
                ///    取得資料內某依區段位元資料
                /// </summary>
                /// <param name="Value">來源資料</param>
                /// <param name="StartIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
                /// <param name="Length">位元長度</param>
                /// <returns>返回值：取得的位元值</returns>
                internal static byte GetValue(byte Value, int StartIndex, byte Length) {
                        byte bMask = 0xff;

                        bMask >>= (8 - Length);
                        Value >>= StartIndex;
                        Value &= bMask;

                        return Value;
                }

                /// <summary>
                ///    取得資料內某依區段位元資料
                /// </summary>
                /// <param name="Value">來源資料</param>
                /// <param name="StartIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
                /// <param name="Length">位元長度</param>
                /// <returns>返回值：取得的位元值</returns>
                internal static byte GetValue(ushort Value, int StartIndex, byte Length) {
                        ushort uMask = 0xffff;

                        uMask >>= (16 - Length);
                        Value >>= StartIndex;
                        Value &= uMask;

                        return ((byte)Value);
                }

                /// <summary>
                ///    設定位元
                /// </summary>
                /// <param name="Src">來源資料</param>
                /// <param name="StartIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
                /// <param name="Value">設定值</param>
                /// <returns>返回值：重設後的數值</returns>
                internal static byte SetValue(byte Src, int StartIndex, byte Value) {
                        Value <<= StartIndex;
                        Src |= Value;
                        
                        return Src;
                }

                /// <summary>
                ///    設定位元
                /// </summary>
                /// <param name="Src">來源資料</param>
                /// <param name="StartIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
                /// <param name="Value">設定值</param>
                /// <returns>返回值：重設後的數值</returns>
                internal static ushort SetValue(ushort Src, int StartIndex, ushort Value) {
                        Value <<= StartIndex;
                        Src |= Value;
                        
                        return Src;
                }
        }
}