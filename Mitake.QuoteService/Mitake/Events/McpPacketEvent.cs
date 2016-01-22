using System.Net.Sockets;
using Mitake.Sockets.Data;

namespace Mitake.Events {
        /// <summary>
        ///   MCP封包事件類別
        /// </summary>
        internal class McpPacketEvent {
                /// <summary>
                ///    [取得]作用中的Socket
                /// </summary>
                internal Socket ActiveSocket {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]MCP緩衝區內的資料
                /// </summary>
                internal PacketBuffer Source {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]MCP封包的類型
                /// </summary>
                internal byte Type {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]MCP封包的命令
                /// </summary>
                internal byte Command {
                        get;
                        set;
                }

                internal McpPacketEvent() { }

                internal McpPacketEvent(Socket socket, PacketBuffer source, byte type, byte command) {
                        this.ActiveSocket = socket;
                        this.Source = source;
                        this.Type = type;
                        this.Command = command;
                }
        }
}