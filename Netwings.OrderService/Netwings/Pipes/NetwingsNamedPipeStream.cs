using System;
using System.IO;
using System.Text;
using System.IO.Pipes;
using log4net;
using Netwings.Event;

namespace Netwings.Pipes {
	public sealed class NetwingsNamedPipeStream : IDisposable {
		private const int CONNECT_TIMEOUT = 5000;
		private const int MAX_BUFFER_SIZE = 8192;
		private const int MAX_NUMBER_OF_SERVER_INSTANCE = 8;

		private static readonly ILog logger = LogManager.GetLogger(typeof(NetwingsNamedPipeStream));

		public event EventHandler<MessageEvent> onMessage = null;

		private bool __bDisposed = false;
		private byte[] __cBuffer = null;
		private string __sPipeName = null;
		private MemoryStream __cMemStream = null;
		private NamedPipeServerStream __cNamedPipeServer = null;

		public NetwingsNamedPipeStream(string pipeName) {
			__sPipeName = pipeName;
			__cBuffer = new byte[MAX_BUFFER_SIZE];
			__cMemStream = new MemoryStream(MAX_BUFFER_SIZE * 4);
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Listen() {
			__cNamedPipeServer = new NamedPipeServerStream(__sPipeName, PipeDirection.In, MAX_NUMBER_OF_SERVER_INSTANCE, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
			__cNamedPipeServer.BeginWaitForConnection(onAccept, __cNamedPipeServer);
		}

		public bool Send(string pipeName, string message) {
			bool bSuccess = true;
			using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.Out)) {
				try {
					pipeStream.Connect(CONNECT_TIMEOUT);
					pipeStream.ReadMode = PipeTransmissionMode.Message;

					byte[] cData = Encoding.UTF8.GetBytes(message);
					pipeStream.Write(cData, 0, cData.Length);
					pipeStream.Flush();
				} catch (Exception __errExcep) {
					if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
					bSuccess = false;
				} finally {
					pipeStream.Close();
				}
			}
			return bSuccess;
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onMessage = null;

					if (__cNamedPipeServer != null) {
						__cNamedPipeServer.Close();
						__cNamedPipeServer.Dispose();
					}
					__cMemStream.Dispose();

					__cBuffer = null;
					__cMemStream = null;
				}
			}
		}

		private void onAccept(IAsyncResult ar) {
			if (!__bDisposed) {
				NamedPipeServerStream cNamedPipeServer = ar.AsyncState as NamedPipeServerStream;

				try {
					cNamedPipeServer.EndWaitForConnection(ar);
					cNamedPipeServer.BeginRead(__cBuffer, 0, MAX_BUFFER_SIZE, onRead, cNamedPipeServer);
				} catch (Exception __errExcep) {
					if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);

					cNamedPipeServer.Close();
					cNamedPipeServer.Dispose();
				}
			}
		}

		private void onRead(IAsyncResult ar) {
			NamedPipeServerStream cNamedPipeServer = ar.AsyncState as NamedPipeServerStream;

			try {
				int iNumBytes = cNamedPipeServer.EndRead(ar);
				if (iNumBytes == 0) {
					cNamedPipeServer.Close();
					cNamedPipeServer.Dispose();

					Listen();
				} else {
					if (iNumBytes < MAX_BUFFER_SIZE) {
						byte[] bData = __cBuffer;
						if (__cMemStream.Position > 0) {
							__cMemStream.Write(__cBuffer, 0, iNumBytes);
							
							bData = __cMemStream.GetBuffer();
							iNumBytes = (int) __cMemStream.Position;
							__cMemStream.Position = 0;
						}

						string sText = Encoding.UTF8.GetString(bData, 0, iNumBytes);
						if (onMessage != null) {
							onMessage(this, new MessageEvent() {
								Buffer = bData,
								Length = iNumBytes,
								Message = sText
							});
						}
					} else {
						__cMemStream.Write(__cBuffer, 0, iNumBytes);
					}
	
					cNamedPipeServer.BeginRead(__cBuffer, 0, MAX_BUFFER_SIZE, onRead, cNamedPipeServer);
				}
			} catch(Exception __errExcep) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
				
				cNamedPipeServer.Close();
				cNamedPipeServer.Dispose();
			}
		}
	}
}