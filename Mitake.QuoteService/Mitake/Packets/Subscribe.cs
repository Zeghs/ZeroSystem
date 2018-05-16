using System.Collections.Generic;  
using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///    股票訂閱封包結構
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class Subscribe : McpStruct {
                private const int SUBSCRIBE_SIZE = 4096;  //目前可以訂閱 4096 * 8 = 32768(支)

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = SUBSCRIBE_SIZE)]
                private byte[] __bSubscribeFlag;  //訂閱旗標
                private byte MCP_End = 0xe;       //封包結尾字元

                /// <summary>
                ///    [取得]使用者訂閱的股票流水號
                /// </summary>
                internal Dictionary<int, int> Serials {
                        get {
                                byte bData = 0;
                                int j = 0, iTmp = 0;
                                int iStockId = 0;
                                Dictionary<int, int> cDict = new Dictionary<int, int>();

                                for (int i = 0; i < SUBSCRIBE_SIZE; i++) {
                                        iTmp = (i << 3);
                                        bData = __bSubscribeFlag[i];
                                        for (j = 0; j < 8; j++) {
                                                if ((bData & 0x1) == 1) {
                                                        iStockId = (iTmp + j);
                                                        cDict.Add(iStockId, iStockId);
                                                }
                                                bData >>= 1;
                                        }
                                }
                                return cDict;
                        }
                }

                internal Subscribe() {
                        this.MCP_Type = 0x01;      //封包類型
                        this.MCP_Command = 0x07;   //封包命令代號
                        
                        //封包長度(高位元)
                        this.MCP_Length_High = ((SUBSCRIBE_SIZE >> 8) & 0xff);

                        //封包長度(低位元)
                        this.MCP_Length_Low = (SUBSCRIBE_SIZE & 0xff);

                        this.__bSubscribeFlag = new byte[SUBSCRIBE_SIZE];  //建立訂閱旗標
                }

                /// <summary>
                ///    加入要訂閱的股票流水號
                /// </summary>
                /// <param name="serial">股票流水號</param>
                internal void Add(int serial) {
                        byte bBit = 0x01;
                        int iIndex = (serial >> 3);
                        int iBit = serial % 8;

                        if (iIndex < SUBSCRIBE_SIZE) {
                                bBit <<= iBit;
                                __bSubscribeFlag[iIndex] |= bBit;
                        }
                }

                internal void Clear() {
                        System.Array.Clear(__bSubscribeFlag, 0, __bSubscribeFlag.Length);
                }

                /// <summary>
                ///    刪除要訂閱的股票流水號
                /// </summary>
                /// <param name="serial">股票流水號</param>
                internal void Remove(int serial) {
                        byte bBit = 0x01;
                        int iIndex = (serial >> 3);
                        int iBit = serial % 8;

                        if (iIndex < SUBSCRIBE_SIZE) {
                                bBit <<= iBit;
                                __bSubscribeFlag[iIndex] &= (byte)(~bBit);
                        }
                }
        }
}