using System;
using System.Text;
using System.Runtime.InteropServices;
using Mitake.Sockets.Data;

namespace Mitake.Sockets {
        /// <summary>
        ///   封包處理類別(可做封包轉換與合併，檢查或設定封包檢查碼....)
        /// </summary>
	internal sealed class MitakePacket {
                /// <summary>
                ///    取得檢查碼是否正確
                /// </summary>
                /// <param name="item">封包資料</param>
                /// <returns>返回值：true=檢查碼正確  false=檢查碼錯誤</returns>
		internal static bool GetChecksum(PacketBuffer item) {
                        return GetChecksum(item, 0);
                }


                /// <summary>
                ///    取得檢查碼是否正確
                /// </summary>
                /// <param name="item">封包資料</param>
                /// <param name="startIndex">起始位置</param>
                /// <returns>返回值：true=檢查碼正確  false=檢查碼錯誤</returns>
		internal static bool GetChecksum(PacketBuffer item, int startIndex) {
                        int iSum = 0;
                        int iBSize = item.Data[startIndex + 3];   //取得封包的資料內容長度
                        int iIndex = startIndex + 3;

                        for (int i = 0; i < iBSize; i++)          //計算CheckSum
                                iSum += item.Data[iIndex + i];

                        return (((iSum & 0x0f) == 0) ? true : false);
                }

                /// <summary>
                ///    設定檢查碼 
                /// </summary>
                /// <param name="item">封包資料</param>
		internal static void SetChecksum(PacketBuffer item) {
                        SetChecksum(item, 0);
                }

                /// <summary>
                ///    設定檢查碼 
                /// </summary>
                /// <param name="item">封包資料</param>
                /// <param name="startIndex">起始位置</param>
		internal static void SetChecksum(PacketBuffer item, int startIndex) {
                        int iSum = 0;
                        int iCheckSum = 0x100;
                        int iBSize = item.Data[startIndex + 3] - 1;   //取得封包的資料內容長度
                        int iIndex = startIndex + 3;

                        for (int i = 0; i < iBSize; i++)         //計算CheckSum
                                iSum += item.Data[iIndex + i];

                        iCheckSum -= (iSum & 0xff);              //算出正確檢查碼
                        item.Data[iIndex + iBSize] = (byte)iCheckSum; //填入檢查碼
                }

                /// <summary>
                ///    將結構轉換成Byte陣列
                /// </summary>
                /// <param name="o">結構類別</param>
                /// <returns>回傳值：byte陣列</returns>
		internal static byte[] ToBuffer(object o) {
                        int size = Marshal.SizeOf(o);
                        IntPtr lpAddr = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(o, lpAddr, true);

                        byte[] buffer = new byte[size];
                        Marshal.Copy(lpAddr, buffer, 0, size);
                        Marshal.FreeHGlobal(lpAddr);
                        return buffer;
                }

                /// <summary>
                ///    將結構轉換成Byte陣列
                /// </summary>
                /// <param name="o">結構類別</param>
                /// <param name="buffer">byte陣列緩衝區</param>
		internal static void ToBuffer(object o, byte[] buffer) {
                        int size = Marshal.SizeOf(o);
                        IntPtr lpAddr = Marshal.AllocHGlobal(size);

                        Marshal.StructureToPtr(o, lpAddr, true);
                        Marshal.Copy(lpAddr, buffer, 0, size);
                        Marshal.FreeHGlobal(lpAddr);
                }


                /// <summary>
                ///    將字串轉換成byte陣列
                /// </summary>
                /// <param name="value">來源字串</param>
                /// <param name="length">字串長度</param>
                /// <returns>回傳值：byte陣列</returns>
		internal static byte[] ToBytes(string value, int length) {
                        byte[] ret = new byte[length];
                        int iLength = ((value.Length < length) ? value.Length : length);
                        Encoding.UTF8.GetBytes(value, 0, iLength, ret, 0);

                        return ret;
                }

                /// <summary>
                ///    將byte陣列轉換成字串
                /// </summary>
                /// <param name="data">來源byte陣列</param>
                /// <returns>回傳值：字串</returns>
		internal static string ToString(byte[] data) {
                        string ret = Encoding.UTF8.GetString(data).Replace("\0", string.Empty);
                        return ret.Trim();
                }

                /// <summary>
                ///   將byte陣列轉換成結構體
                /// </summary>
                /// <typeparam name="T">泛型(結構體類別名稱)</typeparam>
                /// <param name="data">來源byte陣列</param>
                /// <param name="length">byte陣列長度</param>
                /// <returns>回傳直：結構體資料</returns>
		internal static T ToStructure<T>(byte[] data, int length) {
                        IntPtr lpAddr = Marshal.AllocHGlobal(length);
                        Marshal.Copy(data, 0, lpAddr, length);
                        T o = (T)Marshal.PtrToStructure(lpAddr, typeof(T));
                        Marshal.FreeHGlobal(lpAddr);

                        return o;
                }
        }
}