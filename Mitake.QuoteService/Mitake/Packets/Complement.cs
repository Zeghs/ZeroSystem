using System;
using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///     個股回補MCP封包(目前一次最多可補10支股票)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class Complement : McpStruct {
                public int Serial = 0;      //要回補的股票流水號
                private byte MCP_End = 0xe;  //封包結尾字元

                internal Complement() {
                        this.MCP_Type = 0x01;     //封包類型 
                        this.MCP_Command = 0x08;  //封包命令代號
                        this.MCP_Length_Low = 7;  //封包長度
                }
        }
}