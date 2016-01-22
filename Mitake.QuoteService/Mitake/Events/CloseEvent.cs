using System.Net;
using System.Net.Sockets;
  
namespace Mitake.Events {
        /// <summary>
        ///    SocketClose事件類別
        /// </summary>
        internal class CloseEvent {
		private int __iHandle = 0;          //Socket Handle
		private int __iRemotePort = 0;      //遠端Port
                private string __sRemoteIP = null;  //遠端IP位址

                /// <summary>
                ///    [取得]Socket Handle
                /// </summary>
                internal int Handle {
                        get {
                                return __iHandle;
                        }
                }

                /// <summary>
                ///   [取得/設定]中斷連線的訊息
                /// </summary>
                internal string Message {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]遠端主機Port
                /// </summary>
                internal int Port {
                        get {
                                return __iRemotePort;
                        }
                }

                /// <summary>
                ///   [取得/設定]遠端主機位址
                /// </summary>
                internal string RemoteIP {
                        get {
                                return __sRemoteIP;
                        }
                }

                /// <summary>
                ///    [取得/設定]Socket斷線型態
                /// </summary>
                internal int Type {
                        get;
                        set;
                }

                internal CloseEvent(Socket socket) {
                        try {
                                __iHandle = socket.Handle.ToInt32();
                                IPEndPoint cIPPoint = (IPEndPoint)socket.RemoteEndPoint;
                                __sRemoteIP = cIPPoint.Address.ToString();
                                __iRemotePort = cIPPoint.Port;
                        } catch {
                                __sRemoteIP = "???.???.???.???";  //如果自己關閉,則傳回???
                                __iRemotePort = -1;
                        }
                }
        }
}