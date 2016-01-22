using System;
using System.Text;

namespace Zeghs.IO {
        /// <summary>
        ///   資料緩衝區類別
        /// </summary>
        public class ZBuffer {
                private int __iSize = 0;
                private int __iLength = 0;

                /// <summary>
                ///    [取得/設定] 緩衝區內容
                /// </summary>
		public byte[] Data {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定] 緩衝區資料大小
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
                ///    [取得/設定] 目前索引位置
                /// </summary>
		public int Position {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得] 緩衝區容量
                /// </summary>
		public int Size {
                        get {
                                return __iSize; 
                        }
                }

		/// <summary>
		///   [取得] 緩衝區內位元組資料
		/// </summary>
		/// <param name="offset">偏移位置(Position + offset為實際位置)</param>
		/// <returns></returns>
		public byte this[int offset] {
                        get {
                                return this.Data[this.Position + offset];
                        }
                }

		/// <summary>
		///   建構子
		/// </summary>
		public ZBuffer() { 
                }

                /// <summary>
                ///   建構子
                /// </summary>
                /// <param name="size">緩衝區大小</param>
		public ZBuffer(int size) {
                        __iSize = size;
                        this.Data = new byte[__iSize + 8]; 
                }

                /// <summary>
                ///   ZBuffer建構子
                /// </summary>
		/// <param name="buffer">緩衝區陣列</param>
		public ZBuffer(byte[] buffer) {
                        __iLength = buffer.Length;
			if (__iLength > 0) {
				while (buffer[--__iLength] == 0);
				
				++__iLength;
				__iSize = __iLength + 8;
				this.Data = buffer;
			}
                }

                /// <summary>
                ///   ZBuffer建構子
                /// </summary>
                /// <param name="buffer">ZBuffer緩衝區</param>
		public ZBuffer(ZBuffer buffer) {
                        __iLength = buffer.Length;
			__iSize = buffer.Size;
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
		public bool Add(ZBuffer buffer) {
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
                        if (iLength < __iSize) {
				Array.Copy(buffer, index, this.Data, __iLength, length);
				__iLength = iLength;
				return true;
			} else {
				return false;
                        }
                }

		/// <summary>
		///   加入布林資料(1bytes)
		/// </summary>
		/// <param name="value">布林資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(bool value) {
			byte bData = (byte) ((value) ? 1 : 0);
			int iLength = __iLength + 1;
			if (iLength < __iSize) {
				this.Data[__iLength++] = bData;
				return true;
			}
			return false;
		}

		/// <summary>
		///   加入byte資料(1bytes)
		/// </summary>
		/// <param name="value">byte資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(byte value) {
			int iLength = __iLength + 1;
			if (iLength < __iSize) {
				this.Data[__iLength++] = value;
				return true;
			}
			return false;
		}

		/// <summary>
		///   加入short資料(2bytes)
		/// </summary>
		/// <param name="value">short資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(short value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入ushort資料(2bytes)
		/// </summary>
		/// <param name="value">ushort資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(ushort value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入int資料(4bytes, 64位元為8bytes)
		/// </summary>
		/// <param name="value">int資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(int value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入uint資料(4bytes, 64位元為8bytes)
		/// </summary>
		/// <param name="value">uint資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(uint value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入long資料(8bytes)
		/// </summary>
		/// <param name="value">long資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(long value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入ulong資料(8bytes)
		/// </summary>
		/// <param name="value">ulong資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(ulong value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入float資料(4bytes)
		/// </summary>
		/// <param name="value">float資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(float value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   加入double資料(8bytes)
		/// </summary>
		/// <param name="value">double資料</param>
		/// <returns>返回值: ture=成功, false=失敗</returns>
		public bool Add(double value) {
			return this.Add(BitConverter.GetBytes(value));
		}

		/// <summary>
		///   取得Boolean資料
		/// </summary>
		/// <returns>返回值: Boolean</returns>
		public Boolean GetBoolean() {
			Boolean bValue = BitConverter.ToBoolean(this.Data, this.Position);
			++this.Position;
			return bValue;
		}

		/// <summary>
		///   取得Byte資料
		/// </summary>
		/// <returns>返回值: Byte</returns>
		public Byte GetByte() {
			Byte bValue = this[0];
			++this.Position;
			return bValue;
		}

		/// <summary>
		///   取得DateTime資料
		/// </summary>
		/// <returns>返回值: DateTime結構</returns>
		public DateTime GetDateTime() {
			int iDate = this.GetInt32();
			int iTime = this.GetInt32();
			
			int iDay = iDate % 100;
			iDate /= 100;
			int iMonth = iDate % 100;
			int iYear = iDate / 100;

			int iHour = iTime / 3600;
			iTime %= 3600;
			int iMinute = iTime / 60;
			int iSecond = iTime % 60;
			return new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
		}

		/// <summary>
		///   取得Int16資料
		/// </summary>
		/// <returns>返回值: Int16</returns>
		public Int16 GetInt16() {
			Int16 isValue = BitConverter.ToInt16(this.Data, this.Position);
			this.Position += 2;
			return isValue;
		}

		/// <summary>
		///   取得UInt16資料
		/// </summary>
		/// <returns>返回值: UInt16</returns>
		public UInt16 GetUInt16() {
			UInt16 usValue = BitConverter.ToUInt16(this.Data, this.Position);
			this.Position += 2;
			return usValue;
		}

		/// <summary>
		///   取得Int32資料
		/// </summary>
		/// <returns>返回值: Int32</returns>
		public Int32 GetInt32() {
			Int32 iValue = BitConverter.ToInt32(this.Data, this.Position);
			this.Position += 4;
			return iValue;
		}

		/// <summary>
		///   取得UInt32資料
		/// </summary>
		/// <returns>返回值: UInt32</returns>
		public UInt32 GetUInt32() {
			UInt32 uiValue = BitConverter.ToUInt32(this.Data, this.Position);
			this.Position += 4;
			return uiValue;
		}

		/// <summary>
		///   取得Int64資料
		/// </summary>
		/// <returns>返回值: Int64</returns>
		public Int64 GetInt64() {
			Int64 lValue = BitConverter.ToInt64(this.Data, this.Position);
			this.Position += 8;
			return lValue;
		}

		/// <summary>
		///   取得UInt64資料
		/// </summary>
		/// <returns>返回值: UInt64</returns>
		public UInt64 GetUInt64() {
			UInt64 ulValue = BitConverter.ToUInt64(this.Data, this.Position);
			this.Position += 8;
			return ulValue;
		}

		/// <summary>
		///   取得Single資料
		/// </summary>
		/// <returns>返回值: Single</returns>
		public Single GetSingle() {
			Single fValue = BitConverter.ToSingle(this.Data, this.Position);
			this.Position += 4;
			return fValue;
		}

		/// <summary>
		///   取得Double資料
		/// </summary>
		/// <returns>返回值: Double</returns>
		public Double GetDouble() {
			Double dValue = BitConverter.ToDouble(this.Data, this.Position);
			this.Position += 8;
			return dValue;
		}

		/// <summary>
		///   將封包資訊轉換成String
		/// </summary>
		/// <returns>返回值: 封包資訊內容字串</returns>
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