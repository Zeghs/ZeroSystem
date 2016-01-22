using System;
using System.Collections.Generic;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Decode {
	/*
	 *  暫緩撮合訊息(<type:1> = ' = '，[3D]16)
	 *   <data:N> = <時間:2> <format:1> <成交價:n1> <累計成交量:n2>
	 */
	/// <summary>
	///   暫緩撮合訊息
	/// </summary>
	internal sealed class Decode_S3D {
		internal static void Decode(MitakeQuote stock, Mitake.Sockets.Data.PacketBuffer Buffer) {
			int iSize = Buffer.Length - 2;
			byte bSType = Buffer.Data[6];

			Buffer.Position = 9;

			//取得時間
			//if ((bSType & 0x80) == 0) {
			//        iTime = Time.GetTime(Buffer);
			//} else {
			//        iTime = Time.GetOther(Buffer);
			//}

			//取得價量旗標與戳合類型
			byte bFlag = Buffer[0];
			++Buffer.Position;

			stock.戳合類型 = BitConvert.GetValue(bFlag, 2, 2);
		}
	}
}