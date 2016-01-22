using System;

namespace Mitake.Sockets.Data {
        /// <summary>
        ///   封包緩衝區
        /// </summary>
	public class PacketBuffer {
                private int __iSize = 0;
                private int __iLength = 0;

                /// <summary>
                ///    [取得/設定]緩衝區內容
                /// </summary>
		public byte[] Data {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]緩衝區資料大小
                /// </summary>
		public int Length {
                        get {
                                return __iLength;
                        }

                        set {
                                __iLength = value;
                                
                                if (__iSize == 0 || __iSize < __iLength) {
                                        __iSize = __iLength; 
                                }
                        }
                }

                /// <summary>
                ///    [取得/設定]目前索引位置
                /// </summary>
		public int Position {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]緩衝區容量
                /// </summary>
		public int Size {
                        get {
                                return __iSize; 
                        }
                }

		/// <summary>
		///   取得偏移位置資訊(以 Position 為起始位置)
		/// </summary>
		/// <param name="Offset">偏移位置</param>
		/// <returns>返回值: byte資料</returns>
		public byte this[int Offset] {
                        get {
                                return this.Data[this.Position + Offset];
                        }
                }

		/// <summary>
		///   PacketBuffer建構子
		/// </summary>
		public PacketBuffer() { 
                }

                /// <summary>
		///   PacketBuffer建構子
                /// </summary>
                /// <param name="size">緩衝區大小</param>
		public PacketBuffer(int size) {
                        __iSize = size;
                        this.Data = new byte[__iSize + 32]; 
                }

                /// <summary>
                ///   ZBuffer建構子
                /// </summary>
                /// <param name="buffer">緩衝區陣列</param>
		public PacketBuffer(byte[] buffer) {
                        __iLength = buffer.Length;
                        __iSize = __iLength + 32;
                        this.Data = buffer;
                }

                /// <summary>
                ///   ZBuffer建構子
                /// </summary>
                /// <param name="buffer">ZBuffer緩衝區</param>
		public PacketBuffer(PacketBuffer buffer) {
                        __iLength = buffer.Length;
                        __iSize = __iLength + 32;
                        this.Data = buffer.Data;
                }

                /// <summary>
                ///    加入新封包
                /// </summary>
                /// <param name="buffer">來源封包陣列</param>
                /// <returns>返回值:ture=成功  false=失敗</returns>
		public bool Add(byte[] buffer) {
                        return Add(buffer, 0, buffer.Length);
                }

                /// <summary>
                ///    加入新封包
                /// </summary>
                /// <param name="buffer">來源ZBuffer封包</param>
                /// <returns>返回值:ture=成功  false=失敗</returns>
		public bool Add(PacketBuffer buffer) {
                        return Add(buffer.Data, 0, buffer.Length);
                }

                /// <summary>
                ///    加入新封包
                /// </summary>
                /// <param name="buffer">來源封包陣列</param>
                /// <param name="index">來源封包起始位置</param>
                /// <param name="length">複製長度</param>
                /// <returns>返回值:ture=成功  false=失敗</returns>
		public bool Add(byte[] buffer, int index, int length) {
                        int iLength = __iLength + length;
                        if (iLength > __iSize) {
                                return false;
                        } else {
                                Array.Copy(buffer, index, this.Data, __iLength, length);
                                __iLength = iLength;
                                return true;
                        }
                }

		public override string ToString() {
                        System.Text.StringBuilder cSBHex = new System.Text.StringBuilder();
                        System.Text.StringBuilder cSBStr = new System.Text.StringBuilder();

                        int iCount = this.Length + (16 - (this.Length % 16));
                        for (int i = 0; i < iCount; i++) {
                                if ((i % 16) == 0) {
                                        cSBHex.Append("  ").AppendLine(cSBStr.ToString());
                                        cSBStr.Remove(0, cSBStr.Length);
                                }
                                cSBHex.Append(this.Data[i].ToString("X2")).Append(" ");
                                cSBStr.Append((char)((this.Data[i] >= 0x20 && this.Data[i] <= 0x7f) ? this.Data[i] : 0x2e));
                        }

                        cSBHex.Append("  ").AppendLine(cSBStr.ToString());
                        return cSBHex.ToString();
                }
        }
}