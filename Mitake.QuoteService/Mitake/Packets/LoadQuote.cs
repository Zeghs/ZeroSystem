using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///    讀取檔案封包結構
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class LoadQuote : McpStruct {
                /// <summary>
                ///    讀取檔案資料
                /// </summary>
                public byte InfoName = 0x0;
                private byte MCP_End = 0xe;

                internal LoadQuote() {
                        this.MCP_Type = 0x01;     //封包類型 
                        this.MCP_Command = 0x9;   //封包命令代號
                        this.MCP_Length_Low = 1;  //封包長度
                }
        }
}