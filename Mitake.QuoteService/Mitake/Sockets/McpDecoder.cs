using System.Net.Sockets;
using log4net;
using Mitake.Events;
using Mitake.Stock.Data;
using Mitake.Stock.Decode;
using Mitake.Sockets.Data;

namespace Mitake.Sockets {
        /// <summary>
        ///   MCP命令封包解碼類別
        /// </summary>
	internal class McpDecoder {
                private static readonly ILog logger = LogManager.GetLogger(typeof(McpDecoder));

                /// <summary>
                ///    當收到MCP封包後所產生的事件
                /// </summary>
		internal static event McpPacketHandler McpPacketProc;       //MCP封包事件

                /// <summary>
                ///   MCP命令封包解碼(解碼整個封包)
                /// </summary>
                /// <param name="socket">作用中的Socket類別</param>
                /// <param name="token">SocketToken類別</param>
		internal static void Decode(Socket socket, SocketToken token) {
                        PacketDecode(socket, token);

                        if (token.IsDataToTemp) {
                                token.Move();
                                PacketDecode(socket, token);
                                token.Reduction(4096, 512);
                        }
                }

                /// <summary>
                ///   MCP封包解碼(一次解讀一條MCP命令)
                /// </summary>
                /// <param name="socket">作用中的Socket類別</param>
                /// <param name="Token">SocketToken類別</param>
                /// <returns>返回值：true=解碼成功  false=解碼失敗</returns>
		internal static bool McpDecode(Socket socket, SocketToken Token) {
                        PacketBuffer cPacket = Token.ReceiveBuffer;
                        
                        if ((cPacket.Position + 4) >= cPacket.Length) { //如果封包沒有長度表示封包不完整(需要合併)
                                return false;
                        }

                        int iSize = ((cPacket[3] << 8) + cPacket[4]) + 6;
                        if ((cPacket.Position + iSize) > cPacket.Length) {
                                if (logger.IsWarnEnabled) logger.WarnFormat("[McpDecoder.McpDecode] MCP封包內Length值過大... CurrentIndex={0}, MCP_Length={1}, packetSize={2}", cPacket.Position, iSize, cPacket.Length);
                                return false;
                        }

                        //判斷是否是結束字元
                        if (cPacket[iSize - 1] == 0xe) {
                                byte bType = cPacket[1];
                                byte bCommand = cPacket[2];

                                if (bType == 0x03 && bCommand == 0x01) {  //是否為伺服器傳來的心跳包
					if (logger.IsDebugEnabled) logger.DebugFormat("[McpDecoder.McpDecode] Socket({0}) 收到伺服器送來的心跳包...", socket.Handle.ToInt32());
                                } else {
                                        if (McpPacketProc != null) {
                                                Token.SetPackage(iSize);
						if (logger.IsDebugEnabled) logger.DebugFormat("[McpDecoder.McpDecode] Socket({0}) Type={1}, Command={2}", socket.Handle.ToInt32(), bType, bCommand);
						if (logger.IsDebugEnabled) logger.DebugFormat("[McpDecoder.McpDecode] Packet Dump:{0}", Token.Package.ToString());

                                                McpPacketEvent cEvent = Token.McpPacketEvent;
                                                cEvent.ActiveSocket = socket;

                                                McpPacketProc(cEvent);
                                        }
                                }

                                cPacket.Position += (iSize - 1);
                                return true;
                        }
                        return false;
                }

                private static void PacketDecode(Socket socket, SocketToken Token) {
                        PacketBuffer cPacket = Token.ReceiveBuffer;

                        cPacket.Position = -1;
                        while (++cPacket.Position < cPacket.Length) {
                                //判斷是否是MCP標頭格式
                                if (cPacket[0] == 0x0b) {
					if (logger.IsDebugEnabled) logger.Debug("[McpDecoder.PacketDecode] Start decode MCP packet...");
                                        if (McpDecode(socket, Token)) {
						continue;
                                        }
					if (logger.IsDebugEnabled) logger.Debug("[McpDecoder.PacketDecode] End decode MCP packet...");
                                }
			
				Token.AddTemp();
			}
                }
        }
}