using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///    取得股票排行榜
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class QuoteRanking : McpStruct {
                public byte Type = 0; //型態 0=命令  1=排行資料
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                private byte[] __bHandle;     //Socket Handle
                private byte MCP_End = 0xe;

                internal QuoteRanking(byte command) {
                        this.MCP_Type = 0x03;        //封包類型 
                        this.MCP_Command = command;  //封包命令代號
                        this.MCP_Length_Low = 5;     //封包長度
                       
                        __bHandle = new byte[4];     //建立Handle
                }
        }
}