using System;
using Mitake.Events;

namespace Mitake.Sockets.Data {
        /// <summary>
        ///    存放Socket會使用到的其他參數
        /// </summary>
	public sealed class SocketToken {
		internal const int MAX_BUFFER_SIZE = 8192;

		private int __iErrorCount = 0;
		private PacketBuffer __cPackage = null;         //存放一個完整的金融封包或是MCP封包
                private PacketBuffer __cRecvBuffer = null;      //接收緩衝區
                private PacketBuffer __cTempBuffer = null;      //暫存緩衝區(接收緩衝區斷包都會暫存到這裡)
                private StockEvent __cStockEvent = null;   //事件類別(Stock事件)
                private McpPacketEvent __cMcpEvent = null; //事件類別(Mcp事件)

                /// <summary>
                ///    [取得]是否暫存緩衝區有資料
                /// </summary>
		internal bool IsDataToTemp {
                        get {
                                return ((__cTempBuffer.Length > 0) ? true : false);
                        }
                }

		/// <summary>
		///    [取得/設定]是否為標準封包起始點
		/// </summary>
		internal bool IsStart {
			get;
			set;
		}
		
		/// <summary>
                ///    [取得]McpPacketEvent事件
                /// </summary>
		internal McpPacketEvent McpPacketEvent {
                        get {
                                __cMcpEvent.Type = __cPackage[1];
                                __cMcpEvent.Command = __cPackage[2];
                                __cMcpEvent.Source = __cPackage;

                                return __cMcpEvent;
                        }
                }

                /// <summary>
                ///    [取得]封包緩衝區(有可能是金融封包或是MCP封包，一包完整的封包)
                /// </summary>
		internal PacketBuffer Package {
                        get {
                                return __cPackage;
                        }
                }

                /// <summary>
                ///    [取得]接收緩衝區(負責提供給Socket接收資料使用)
                /// </summary>
		public PacketBuffer ReceiveBuffer {
                        get {
                                return __cRecvBuffer;
                        }
                }

                /// <summary>
                ///    [取得]StockEvent事件
                /// </summary>
		internal StockEvent StockEvent {
                        get {
                                __cStockEvent.Source = __cPackage;    //來源封包
                                __cStockEvent.Serial = 0;            //預設無股票Id = -1
                                __cStockEvent.Type = __cPackage[6];   //Type
                                __cStockEvent.Header = __cPackage[1]; //Header

                                return __cStockEvent;
                        }
                }

		/// <summary>
		///   建構子
		/// </summary>
		public SocketToken() : this(null) {
                }

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="Buffer">PacketBuffer 類別</param>
		public SocketToken(PacketBuffer Buffer) {
                        __cStockEvent = new StockEvent();
                        __cMcpEvent = new McpPacketEvent();

                        __cPackage = new PacketBuffer(4096);
                        __cTempBuffer = new PacketBuffer(MAX_BUFFER_SIZE);

                        if (Buffer == null) {
                                __cRecvBuffer = new PacketBuffer(MAX_BUFFER_SIZE);
                        } else {
                                __cRecvBuffer = Buffer;
                        }
                }

                /// <summary>
                ///    加入到暫存封包內
                /// </summary>
		internal void AddTemp() {
                        __cTempBuffer.Data[__cTempBuffer.Length] = __cRecvBuffer[0];
                        ++__cTempBuffer.Length;
                }

                /// <summary>
                ///    將封包緩衝區全部清除
                /// </summary>
		internal void Clear() {
                        __cRecvBuffer.Length = 0;
                        __cTempBuffer.Length = 0;
                }

                /// <summary>
                ///    將暫存封包般移到主要緩衝區
                /// </summary>
		internal void Move() {
                        PacketBuffer cTemp = __cRecvBuffer;
                        __cRecvBuffer = __cTempBuffer;
                        __cTempBuffer = cTemp;
                        __cTempBuffer.Length = 0;
                }

                /// <summary>
                ///    縮小暫存封包的資料大小
                /// </summary>
                /// <param name="MaxSize">資料最大Size(只要超過此Size就會縮小暫存封包大小)</param>
                /// <param name="Size">要縮小的大小</param>
		internal void Reduction(int MaxSize, int Size) {
			if (__cRecvBuffer.Length == __cTempBuffer.Length) {
				++__iErrorCount;
				if (__iErrorCount == 3) {
					__iErrorCount = 0;
					__cTempBuffer.Length = 0;
				}
			} else {
				if (__cTempBuffer.Length > MaxSize) {
					Array.Copy(__cTempBuffer.Data,
						   __cTempBuffer.Length - Size,
						   __cTempBuffer.Data,
						   0,
						   Size);
					__cTempBuffer.Length = Size;
				}
				__iErrorCount = 0;
			}
		}

                /// <summary>
                ///     設定完整Package封包
                /// </summary>
                /// <param name="Size">封包大小</param>
		internal void SetPackage(int Size) {
                        __cPackage.Length = 0;
                        __cPackage.Position = 0;
                        __cPackage.Add(__cRecvBuffer.Data, __cRecvBuffer.Position, Size);
                }
        }
}