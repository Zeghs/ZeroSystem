using System.Runtime.InteropServices;
using Mitake.Sockets; 

namespace Mitake.Packets {
        /// <summary>
        ///   Authentication登入封包結構表
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class AuthenticationLogin : McpStruct {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                private byte[] __bUserId;                            //使用者名稱
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                private byte[] __bPassword;                          //使用者密碼
                public byte ClientType;                              //使用者端類型(0=專業版  1=精簡版)
                public byte ISP;                                     //使用者端所屬ISP代碼(未知則為0)
                private byte MCP_End = 0xe;

                public int Index = 0;                                //伺服器索引位置

                /// <summary>
                ///    [取得]使用者帳號
                /// </summary>
                public string UserId {
                        get {
                                return MitakePacket.ToString(__bUserId);
                        }
                }

                /// <summary>
                ///    [取得]使用者密碼
                /// </summary>
                public string Password {
                        get {
                                return MitakePacket.ToString(__bPassword);
                        }
                }

                public AuthenticationLogin() { }

                public AuthenticationLogin(string UserID, string Password, byte ClientType, byte ISP) {
                        this.MCP_Type = 0x01;
                        this.MCP_Command = 0x03;
                        this.MCP_Length_Low = 22;
                        this.__bUserId = MitakePacket.ToBytes(UserID, 10);
                        this.__bPassword = MitakePacket.ToBytes(Password, 10);
                        this.ClientType = ClientType;
                        this.ISP = ISP;
                }
        }
}