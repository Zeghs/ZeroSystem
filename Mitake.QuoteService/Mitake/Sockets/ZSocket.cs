using System;
using System.Net;
using System.Net.Sockets;
using log4net;
using Mitake.Events;
using Mitake.Sockets.Data;

namespace Mitake.Sockets {
	internal class ZSocket : Socket {
                private static readonly ILog logger = LogManager.GetLogger(typeof(ZSocket));

		internal event ConnectHandler ConnectProc;  //完成連結事件
		internal event CloseHandler CloseProc;    //關閉事件        
		internal event ReceiveHandler ReceiveProc;  //完成接收事件

                private SocketAsyncEventArgsPool __cSocketPool = null; //SocketAsyncEventArgsPool

		internal ZSocket() : this(1) { 
                }

		internal ZSocket(int capacity) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
                        __cSocketPool = new SocketAsyncEventArgsPool(capacity);
                }

                /// <summary>
                ///   連結至遠端伺服器
                /// </summary>
                /// <param name="RemoteIP">遠端伺服器IP位址</param>
                /// <param name="Port">遠端伺服器Port號碼</param>
                /// <returns>返回值：true=連線成功 false=連線失敗</returns>
		internal new bool Connect(string RemoteIP, int Port) {
                        bool bRet = false;

			if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.Connect] Connetcing... address={0}({1})", RemoteIP, Port);
                        IPEndPoint cRemote = new IPEndPoint(IPAddress.Parse(RemoteIP), Port);
                        try {
                                this.Connect(cRemote);
                        } catch (Exception __errExcep) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
                        }

                        if (this.Connected) {
                                SocketAsyncEventArgs cAsync = __cSocketPool.Pop();
                                cAsync.RemoteEndPoint = cRemote;
                                ConnectComplete(cAsync);
                                bRet = true;
                                if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.Connect] Socket({0}) Connected...", this.Handle.ToInt32());
                        }
                        return bRet;
                }

                /// <summary>
                ///    關閉連線端口，並釋放連線資源(關閉後此類別則無法在使用)
                /// </summary>
		internal new void Close() {
                        base.Close();

                        SocketAsyncEventArgs cArgs = null;
                        int iCount = __cSocketPool.Count;
                        for (int i = 0; i < iCount; i++) {
                                cArgs = __cSocketPool.Pop();
                                if (cArgs != null) {
                                        cArgs.SetBuffer(null, 0, 0);
                                        cArgs.UserToken = null;
                                        cArgs.Dispose();
                                }
                        }
                        if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.Close] Socket({0}) Close socket...", this.Handle.ToInt32());
                }

                private void ConnectComplete(SocketAsyncEventArgs e) {
                        bool bError = false;
                        try {
                                if (ConnectProc != null)
                                        ConnectProc(this, new ConnectEvent(this));
                        } catch (Exception __errExcep) {
				bError = true;
				if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
                        }
                        if (logger.IsDebugEnabled) logger.DebugFormat("Socket({0}) ConnectEvent事件發送完成！", this.Handle.ToInt32());

                        if (this.Connected) {
                                e.Completed += ReceiveCallback;  //加入ReceiveCallback事件

                                try {
                                        if (!this.ReceiveAsync(e))
                                                ReceiveCallback(this, e);
                                } catch {
                                        e.Completed -= ReceiveCallback;
                                        bError = true;
                                }
                        } else {
                                bError = true;
                        }

                        if (bError) {
                                __cSocketPool.Push(e);
                        }
                }

                private void ReceiveCallback(object sender, SocketAsyncEventArgs e) {
                        bool bError = false;
                        Socket cHandler = (Socket)sender;
                        if (e.SocketError == SocketError.Success) {
                                int iBytesRead = e.BytesTransferred;
                                if (iBytesRead > 0) {
                                        SocketToken cToken = (SocketToken)e.UserToken;
                                        if (ReceiveProc != null) {
						if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.ReceiveCallback] Socket({0}) packetSize={1}", cHandler.Handle.ToInt32(), iBytesRead);
                                                try {
                                                        cToken.ReceiveBuffer.Length = iBytesRead;
							if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.ReceiveCallback] Socket({0}) Packet Dump:{1}", cHandler.Handle.ToInt32(), cToken.ReceiveBuffer.ToString());
                                                        ReceiveProc(this, new ReceiveEvent(cHandler, cToken));
                                                } catch (Exception __errExcep) {
							bError = true;
							if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
                                                }
                                        }

                                        if (!bError && cHandler.Connected) {
                                                try {
                                                        e.SetBuffer(cToken.ReceiveBuffer.Data, 0, SocketToken.MAX_BUFFER_SIZE);
                                                        if (!cHandler.ReceiveAsync(e))
                                                                ReceiveCallback(sender, e);
                                                } catch {
                                                        bError = true;
                                                }
                                        } else {
                                                bError = true;
                                        }
                                } else {
                                        bError = true;
                                }
                        } else {
                                bError = true;
                        }

                        if (bError) {
                                e.Completed -= ReceiveCallback;  //取消ReceiveCallback事件
                                lock (__cSocketPool) {
                                        __cSocketPool.Push(e);   //回收Socket到pool
                                }

                                CloseEvent cCloseEvent = null;
                                if (CloseProc != null) {
                                        cCloseEvent = new CloseEvent(cHandler);
                                }

                                try {
                                        if (cHandler.Connected) {
                                                cHandler.Shutdown(SocketShutdown.Both);
                                                cHandler.Disconnect(true); //斷開連結
                                        }
                                } catch {
                                } 
                                
                                if (CloseProc != null) {
                                        CloseProc(cHandler, cCloseEvent);  //發送關閉事件
                                }
                                cHandler.Close(); //關閉Socket連結  
				if (logger.IsDebugEnabled) logger.DebugFormat("[ZSocket.ReceiveCallback] Socket({0}) Socket closed...", cHandler.Handle.ToInt32());
                        }
                }
        }
}