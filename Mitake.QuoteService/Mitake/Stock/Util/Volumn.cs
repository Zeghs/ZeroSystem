using Mitake.Sockets.Data;

namespace Mitake.Stock.Util {
        /// <summary>
        ///   成交量格式轉換類別
        /// </summary>
        internal sealed class Volumn {
                /// <summary>
                ///    取得成交量資訊函式
                /// </summary>
                /// <param name="type">成交量類型(請參考資料格式)</param>
                /// <param name="Buffer">ZBuffer類別</param>
                /// <returns>傳回值：成交量(或是其他型態)</returns>
		internal static uint GetVolumn(int type, PacketBuffer Buffer) {
                        uint uVolumn = 0;
                        switch (type) {
                                case 0:
                                        uVolumn = (uint)Buffer[0];
                                        break;
                                case 1:
                                        uVolumn = (uint)(Buffer[0] << 8);
                                        uVolumn += (uint)Buffer[1];
                                        break;
                                case 2:
                                        uVolumn = (uint)(Buffer[0] << 16);
                                        uVolumn += (uint)(Buffer[1] << 8);
                                        uVolumn += Buffer[2];
                                        break;
                                case 3:
                                        uVolumn = (uint)(Buffer[0] << 24);
                                        uVolumn += (uint)(Buffer[1] << 16);
                                        uVolumn += (uint)(Buffer[2] << 8);
                                        uVolumn += Buffer[3];
                                        break;
                        }

                        Buffer.Position += (type + 1);
                        return uVolumn;
                }

                /// <summary>
                ///     設定成交量資訊函式
                /// </summary>
                /// <param name="Buffer">封包陣列</param>
                /// <param name="Volumn">成交量</param>
                /// <param name="VolumnFlag">成交量旗標</param>
                /// <param name="BitIndex">起始位元索引(0=最右邊起始位元 依序向左遞增)</param>
		internal static void SetVolumn(PacketBuffer Buffer, uint Volumn, ref byte VolumnFlag, int BitIndex) {
			byte[] bTmpData = new byte[4];   //建立一個暫存的陣列
			
			byte i = 0;
                        for (i = 0; i <= 3; i++) {
                                bTmpData[i] = (byte)(Volumn & 0xff);
                                if ((Volumn >>= 8) == 0) break;
                        }

                        VolumnFlag = BitConvert.SetValue(VolumnFlag, BitIndex, i);

                        int iCount = i + 1;
                        for (byte j = 0; j < iCount; j++) {
                                Buffer.Data[Buffer.Position++] = bTmpData[i - j];
                        }
                }
        }
}