using System.Runtime.InteropServices;

namespace Mitake.Packets {
        /// <summary>
        ///     使用者登出命令封包結購類別
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class Logout : McpStruct {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public byte[] EXIT = { 0x45, 0x58, 0x49, 0x54 }; //'EXIT'
                private byte MCP_End = 0xe;

                internal Logout() {
                        this.MCP_Type = 0x01;
                        this.MCP_Command = 0x7f;
                        this.MCP_Length_Low = 4;
                }
        }
}