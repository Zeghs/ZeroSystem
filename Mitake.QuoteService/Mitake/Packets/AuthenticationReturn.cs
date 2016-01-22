using System.Runtime.InteropServices;
using Mitake.Sockets;

namespace Mitake.Packets {
        /// <summary>
        ///   Authentication登入回應封包結構表
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal sealed class AuthenticationReturn : McpStruct {
                public byte __bHostId;                               //所分配服務主機Host值
                private byte __bPort_H;                              //Port 低8位元
                private byte __bPort_L;                              //Port 高8位元
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
                private byte[] __bSessionKey;                        //Session Key
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                private byte[] __bServiceIP;                         //Service IP Address
                private byte MCP_End = 0xe;

                /// <summary>
                ///   [取得] 遠端認證伺服器的Port
                /// </summary>
                internal int RemotePort {
                        get {
                                return ((__bPort_H << 8) + __bPort_L);
                        }
                }

                /// <summary>
                ///   [取得] SessionKey的值
                /// </summary>
                internal string SessionKey {
                        get {
                                return MitakePacket.ToString(__bSessionKey);
                        }
                }

                /// <summary>
                ///   [取得] 遠端認證伺服器的IP位址
                /// </summary>
                internal string RemoteIP {
                        get {
                                return MitakePacket.ToString(__bServiceIP);
                        }
                }

                internal AuthenticationReturn() {
                }

                internal AuthenticationReturn(byte hostId, string sessionKey, string remoteIP, int remotePort) {
                        this.MCP_Type = 0x01;
                        this.MCP_Command = 0x04;
                        this.MCP_Length_Low = 39;
                        this.__bHostId = hostId;
                        this.__bPort_H = (byte)(remotePort >> 8);
                        this.__bPort_L = (byte)(remotePort & 0xff);
                        this.__bServiceIP = MitakePacket.ToBytes(remoteIP, 16);
                        this.__bSessionKey = MitakePacket.ToBytes(sessionKey, 20);
                }
        }
}