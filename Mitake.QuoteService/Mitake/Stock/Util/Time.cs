using System;
using Mitake.Events;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Util {
	internal sealed class Time {
		private static DateTime __cToday = DateTime.Today;

		/// <summary>
		///   轉換 DateTime 為總秒數(00:00:00 到目前的總秒數)
		/// </summary>
		/// <param name="time">DateTime 結構</param>
		/// <returns>返回值: 總秒數</returns>
		internal static int ConvertForTotalSeconds(DateTime time) {
			return time.Hour * 3600 + time.Minute * 60 + time.Second;
		}

		/// <summary>
		///   轉換總秒數為 DateTime 結構
		/// </summary>
		/// <param name="time">總秒數</param>
		/// <returns>返回值: DateTime 結構</returns>
		internal static DateTime ConvertForDateTime(int totalSeconds) {
			return __cToday.AddSeconds(totalSeconds);
		}

		/// <summary>
		///   時間封包解碼(解碼交易日期)
		/// </summary>
		/// <param name="buffer">ZBuffer類別</param>
		/// <returns>返回值: DateTime結構</returns>
		internal static DateTime GetDate(PacketBuffer buffer) {
			ushort TimeByte = (ushort) ((buffer[0] << 8) + buffer[1]);
			int iYear = BitConvert.GetValue(TimeByte, 9, 7) + 1990;
			int iMonth = BitConvert.GetValue(TimeByte, 5, 4);
			int iDay = BitConvert.GetValue(TimeByte, 0, 5);
			buffer.Position += 2;

			iMonth = (iMonth < 1) ? 1 : (iMonth > 12) ? 12 : iMonth;
			int iMaxDay = DateTime.DaysInMonth(iYear, iMonth);
			iDay = (iDay < 1) ? 1 : (iDay > iMaxDay) ? iMaxDay : iDay;
			return new DateTime(iYear, iMonth, iDay);
		}

		/// <summary>
                ///   股票即時資訊時間解碼
                /// </summary>
                /// <param name="buffer">ZBuffer類別</param>
		/// <returns>返回值: DateTime結構</returns>
		internal static DateTime GetTime(PacketBuffer buffer) {
                        int hh = 0, mm = 0, ss = 0;
                        ushort TimeByte = (ushort)((buffer[0] << 8) + buffer[1]);

                        hh = (BitConvert.GetValue(TimeByte, 12, 3) + 9);
			hh = (hh < 0 || hh > 23) ? 0 : hh;
			mm = BitConvert.GetValue(TimeByte, 6, 6);
			mm = (mm < 0 || mm > 59) ? 0 : mm;
                        ss = BitConvert.GetValue(TimeByte, 0, 6);
			ss = (ss < 0 || ss > 59) ? 0 : ss;
			buffer.Position += 2;

			return __cToday.AddSeconds(hh * 3600 + mm * 60 + ss);
                }

                /// <summary>
                ///   股票即時資訊時間解碼(解碼開盤其他時間 9:00:00 ~ 16:59:59 以外的時間)
                /// </summary>
                /// <param name="buffer">ZBuffer類別</param>
		/// <returns>返回值: DateTime結構</returns>
		internal static DateTime GetOther(PacketBuffer buffer) {
                        int hh = 0, mm = 0, ss = 0;
                        ushort TimeByte = (ushort)((buffer[0] << 8) + buffer[1]);

                        hh = BitConvert.GetValue(TimeByte, 12, 3);
                        if (hh == 6 || hh == 7) {
                                hh += 1;
                        } else {
                                hh += 17;
                        }

			hh = (hh < 0 || hh > 23) ? 0 : hh;
			mm = BitConvert.GetValue(TimeByte, 6, 6);
			mm = (mm < 0 || mm > 59) ? 0 : mm;
			ss = BitConvert.GetValue(TimeByte, 0, 6);
			ss = (ss < 0 || ss > 59) ? 0 : ss;

			buffer.Position += 2;
			return __cToday.AddSeconds(hh * 3600 + mm * 60 + ss);
                }

		/// <summary>
		///   股票即時資訊時間解碼(解碼時間 00:00:00 ~ 23:59:59)
		/// </summary>
		/// <param name="buffer">ZBuffer類別</param>
		/// <returns>返回值: DateTime結構</returns>
		internal static DateTime GetSpecial(PacketBuffer buffer) {
			int hh = 0, mm = 0, ss = 0;
			int TimeByte = (buffer[0] << 16) + (buffer[1] << 8) + buffer[2];

			hh = BitConvert.GetValue(TimeByte, 19, 5);
			hh = (hh < 0 || hh > 23) ? 0 : hh;
			hh = (hh < 15) ? 24 + hh : hh;
			mm = BitConvert.GetValue(TimeByte, 13, 6);
			mm = (mm < 0 || mm > 59) ? 0 : mm;
			ss = BitConvert.GetValue(TimeByte, 7, 6);
			ss = (ss < 0 || ss > 59) ? 0 : ss;
			buffer.Position += 3;

			return __cToday.AddSeconds(hh * 3600 + mm * 60 + ss);
		}

		/// <summary>
		///   時間封包解碼(解碼時間)
		/// </summary>
		/// <param name="buffer">ZBuffer類別</param>
		/// <returns>返回值: DateTime結構</returns>
		internal static DateTime GetDateTime(PacketBuffer buffer) {
			int iHour = ((buffer[1] == 35) ? 0 : 12);
			buffer.Position += 4;

			ushort TimeByte = (ushort) ((buffer[0] << 8) + buffer[1]);
                        iHour += BitConvert.GetValue(TimeByte, 12, 4);
			iHour = (iHour < 0 || iHour > 23) ? 0 : iHour;
			int iMinute = BitConvert.GetValue(TimeByte, 6, 6);
			iMinute = (iMinute < 0 || iMinute > 59) ? 0 : iMinute;
			int iSecond = BitConvert.GetValue(TimeByte, 0, 6);
			iSecond = (iSecond < 0 || iSecond > 59) ? 0 : iSecond;
			buffer.Position += 2;
		
			TimeByte = (ushort) ((buffer[0] << 8) + buffer[1]);
			int iYear = BitConvert.GetValue(TimeByte, 9, 7) + 1990;
			int iMonth = BitConvert.GetValue(TimeByte, 5, 4);
			int iDay = BitConvert.GetValue(TimeByte, 0, 5);
			
			iMonth = (iMonth < 1) ? 1 : (iMonth > 12) ? 12 : iMonth;
			int iMaxDay = DateTime.DaysInMonth(iYear, iMonth);
			iDay = (iDay < 1) ? 1 : (iDay > iMaxDay) ? iMaxDay : iDay;
			buffer.Position += 2;

			return new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
		}

                /// <summary>
                ///    設定時間
                /// </summary>
                /// <param name="buffer">ZBuffer類別</param>
                /// <param name="time">時間設定值</param>
                /// <param name="isReal">是否為即時  true=即時   false=補價</param>
                internal static void SetTime(PacketBuffer buffer, int time, bool isReal) {
                        ushort usTime = 0;
                        byte bHour = 0, bMinute = 0, bSecond = 0;
                        bHour = (byte)((time / 3600) - 9);
                        time %= 3600;
                        bMinute = (byte)(time / 60);
                        bSecond = (byte)(time % 60);

                        if (!isReal) {
				usTime = BitConvert.SetValue(usTime, 15, 1);
                        }

                        usTime = BitConvert.SetValue(usTime, 12, bHour);
                        usTime = BitConvert.SetValue(usTime, 6, bMinute);
                        usTime = BitConvert.SetValue(usTime, 0, bSecond);

                        buffer.Data[buffer.Position++] = (byte)((usTime >> 8) & 0xff);
                        buffer.Data[buffer.Position++] = (byte)(usTime & 0xff);
                }

                /// <summary>
                ///    設定時間(9:00:00 ~ 16:59:59 以外的時間)
                /// </summary>
                /// <param name="buffer">ZBuffer類別</param>
                /// <param name="time">時間設定值</param>
                /// <param name="isReal">是否為即時  true=即時   false=補價</param>
		internal static void SetOther(PacketBuffer buffer, int time, bool isReal) {
                        ushort usTime = 0;
                        byte bHour = 0, bMinute = 0, bSecond = 0;
                        bHour = (byte)(time / 3600);
                        
                        if (bHour == 6 || bHour == 7) {
                                --bHour;
                        } else {
                                bHour -= 17;
                        }

                        time %= 3600;
                        bMinute = (byte)(time / 60);
                        bSecond = (byte)(time % 60);

                        if (!isReal) {
				usTime = BitConvert.SetValue(usTime, 15, 1);
                        }

                        usTime = BitConvert.SetValue(usTime, 12, bHour);
                        usTime = BitConvert.SetValue(usTime, 6, bMinute);
                        usTime = BitConvert.SetValue(usTime, 0, bSecond);

                        buffer.Data[buffer.Position++] = (byte)((usTime >> 8) & 0xff);
                        buffer.Data[buffer.Position++] = (byte)(usTime & 0xff);
                }

		/// <summary>
		///   設定今天日期(今天日期會由伺服器送過來)
		/// </summary>
		/// <param name="today">今天日期</param>
		internal static void SetToday(DateTime today) {
			__cToday = today;
		}
        }
}