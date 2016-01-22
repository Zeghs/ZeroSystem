using System.Runtime.InteropServices;
using Mitake.Sockets;

namespace Mitake.Packets {
        /// <summary>
        ///   用戶端註冊Session服務引擎封包結構表
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class SessionRegister : McpStruct {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
                private byte[] __bUserId;                            //使用者名稱
                private byte ClientType;                             //使用者端類型(0=專業版  1=精簡版)
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
                private byte[] __bSessionKey;                        //Session Key
                private byte MCP_End = 0xe;

                /// <summary>
                ///    [取得]SessionKey的值
                /// </summary>
                internal string SessionKey {
                        get {
                                return MitakePacket.ToString(__bSessionKey);
                        }
                }

                /// <summary>
                ///    [取得]UserId
                /// </summary>
                internal string UserId {
                        get {
                                return MitakePacket.ToString(__bUserId);
                        }
                }

                internal SessionRegister() { 
		}

                internal SessionRegister(byte command, string userId, byte clientType, string sessionKey) {
                        this.MCP_Type = 0x01;
                        this.MCP_Command = command;
                        this.MCP_Length_Low = 31;
                        this.__bUserId = MitakePacket.ToBytes(userId, 10);
                        this.__bSessionKey = MitakePacket.ToBytes(sessionKey, 20);
                        this.ClientType = clientType;
                }
        }
}