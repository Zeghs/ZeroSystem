using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///   MCP封包檔頭結構表
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class McpStruct {
                protected byte MCP_Header      = 0x0b; //MCP封包標頭
                protected byte MCP_Type        = 0;    //MCP類型
                protected byte MCP_Command     = 0;    //MCP命令狀態
                protected byte MCP_Length_High = 0;    //MCP封包長度
                protected byte MCP_Length_Low  = 0;    //MCP封包長度
        }
}