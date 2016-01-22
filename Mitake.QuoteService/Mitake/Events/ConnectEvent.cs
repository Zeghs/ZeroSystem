using System.Net;
using System.Net.Sockets;

namespace Mitake.Events {
        /// <summary>
        ///   完成連結事件類別
        /// </summary>
        internal class ConnectEvent {
		private int __iRemotePort = 0;
		private string __sRemoteIP = null;

                /// <summary>
                ///    [取得]伺服器IP位址
                /// </summary>
                internal string RemoteIP {
                        get {
                                return __sRemoteIP;
                        }
                }

                /// <summary>
                ///    [取得]伺服器端連接Port
                /// </summary>
                internal int Port {
                        get {
                                return __iRemotePort;
                        }
                }

                /// <summary>
                ///    建構子
                /// </summary>
                /// <param name="socket">Socket類別</param>
                internal ConnectEvent(Socket socket) {
                        try {
                                IPEndPoint cIPPoint = (IPEndPoint)socket.RemoteEndPoint;
                                __sRemoteIP = cIPPoint.Address.ToString();
                                __iRemotePort = cIPPoint.Port;
                        } catch {
                                __sRemoteIP = "???.???.???.???";
                                __iRemotePort = -1;
                        }
                }
        }
}