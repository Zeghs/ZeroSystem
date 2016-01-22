using System.Net.Sockets;
using System.Collections.Generic;
using Mitake.Sockets.Data;

namespace Mitake.Sockets {
        /// <summary>
        ///    SocketAsyncEventArgsPool類別(儲存池)
        /// </summary>
        internal sealed class SocketAsyncEventArgsPool {
                private Stack<SocketAsyncEventArgs> __cStackPool = null;  //Socket Pool

                /// <summary>
                ///    [取得]儲存池內的可用個數
                /// </summary>
		internal int Count {
                        get {
                                return __cStackPool.Count;
                        }
                }

		internal SocketAsyncEventArgsPool(int capacity) {
                        __cStackPool = new Stack<SocketAsyncEventArgs>(capacity + 8);

                        for (int i = 0; i < capacity; i++) {
                                SocketToken cToken = new SocketToken();
                                SocketAsyncEventArgs cEventArgs = new SocketAsyncEventArgs();
                                cEventArgs.UserToken = cToken;
                                __cStackPool.Push(cEventArgs);
                        }
                }

                /// <summary>
                ///    將SocketAsyncEventArgs類別存入pool內
                /// </summary>
                /// <param name="item">SocketAsyncEventArgs類別</param>
		internal void Push(SocketAsyncEventArgs item) {
                        if (item != null) {
                                lock (__cStackPool) {
                                        item.SetBuffer(null, 0, 0);
                                        __cStackPool.Push(item);
                                }
                        }
                }

                /// <summary>
                ///    將SocketAsyncEventArgs類別從pool內取出
                /// </summary>
                /// <returns>返回值：SocketAsyncEventArgs類別</returns>
		internal SocketAsyncEventArgs Pop() {
                        SocketAsyncEventArgs cEventArgs = null;
                        lock (__cStackPool) {
                                if (__cStackPool.Count > 0) {
                                        cEventArgs = __cStackPool.Pop();
                                        
                                        SocketToken cToken = (SocketToken)cEventArgs.UserToken;
                                        cToken.Clear();
                                        cEventArgs.SetBuffer(cToken.ReceiveBuffer.Data, 0, SocketToken.MAX_BUFFER_SIZE);  
                                }
                        }
                        
                        return cEventArgs;
                }
        }
}