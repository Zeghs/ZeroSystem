using System;
using System.Net.Sockets;
using Mitake.Sockets.Data;

namespace Mitake.Events {
        /// <summary>
        ///    ZSocket完成接收之後所發出的事件類別
        /// </summary>
        internal class ReceiveEvent {
                private Socket __cSocket = null;
                private SocketToken __cToken = null;

                /// <summary>
                ///    [取得]作用中的Socket
                /// </summary>
                internal Socket ActiveSocket {
                        get {
                                return __cSocket;
                        }
                }

                /// <summary>
                ///    [取得]緩衝區內的資料
                /// </summary>
                internal SocketToken Token {
                        get {
                                return __cToken;
                        }
                }

                internal ReceiveEvent(Socket socket, SocketToken token) {
                        __cSocket = socket;
                        __cToken = token;
                }
        }
}