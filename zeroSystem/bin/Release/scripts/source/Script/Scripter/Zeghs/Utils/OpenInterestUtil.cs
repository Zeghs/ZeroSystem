using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Web;
using Zeghs.Data;

namespace Zeghs.Utils {
	public sealed class OpenInterestUtil {
		private static CookieContainer __cCookies = new CookieContainer();

		public static OpenInterestData Load(DateTime date) {
			OpenInterestData cData = null;

			string sFile = string.Format("{0}OI\\{1}.dat", GlobalSettings.Paths.DatabasePath, date.ToString("yyyyMMdd"));
			if (File.Exists(sFile)) {
				byte[] bArray = File.ReadAllBytes(sFile);
				ZBuffer cBuffer = new ZBuffer();
				cBuffer.Data = bArray;
				cBuffer.Length = bArray.Length;

				cData = new OpenInterestData(date, cBuffer);
			}
			return cData;
		}

		private static ZBuffer Decode(string content) {
			ZBuffer cBuffer = null;
			string[] sDatas = content.Split(new char[] { '\r', '\n' });
			if (sDatas[0].Length == 0) {
				return cBuffer;
			}

			int iLength = sDatas.Length;
			if (iLength > 1) {
				cBuffer = new ZBuffer(iLength * 20);

				int iIndexM = -1, iIndexW = -1;
				bool bWeek = false;
				string sSymbolId = null;
				string sSymbol = string.Empty;
				string sContract = string.Empty;
				for (int i = 1; i < iLength; i++) {
					string sData = sDatas[i];
					if (sData.Length > 0) {
						string[] sItems = sData.Split(',');
						int iInterest = int.Parse(sItems[11]);
						if (iInterest > 0) {
							bWeek = sItems[2][6] == 'W';
							float fTarget = float.Parse(sItems[3]);
							char chCallOrPut = sItems[4].Equals("買權") ? 'C' : 'P';

							if (!sSymbol.Equals(sItems[1])) {
								iIndexW = -1;
								iIndexM = -1;
								sSymbol = sItems[1];
								sContract = string.Empty;
							} 
							
							if (!sContract.Equals(sItems[2])) {
								if (bWeek) {
									++iIndexW;
								} else {
									++iIndexM;
								}
								sContract = sItems[2];
							}
							sSymbolId = string.Format("{0}{1}{2}{3}", (bWeek) ? "TXW" : sSymbol, (bWeek) ? iIndexW : iIndexM, chCallOrPut, fTarget);

							cBuffer.Add(sSymbolId);
							cBuffer.Add(iInterest);
						}
					}

				}
			}
			return cBuffer;
		}

		public static void Download(DateTime date) {
			string sDate = string.Format("{0}%2F{1}%2F{2}", date.Year, date.Month, date.Day);

			ZRequest cRequest = new ZRequest();
			cRequest.Method = "POST";
			cRequest.Url = "https://www.taifex.com.tw/chinese/3/3_2_3_b.asp";
			cRequest.CookieContainer = __cCookies;
			cRequest.Parameters = string.Format("goday=&DATA_DATE={0}&DATA_DATE1={0}&DATA_DATE_Y=&DATA_DATE_M=&DATA_DATE_D=&DATA_DATE_Y1=&DATA_DATE_M1=&DATA_DATE_D1=&syear=&smonth=&sday=&syear1=&smonth1=&sday1=&datestart={0}&dateend={0}&COMMODITY_ID=all&commodity_id2t=&his_year=2014", sDate);
			int iRet = cRequest.Request();
			if (iRet == 0) {
				ZReader cReader = new ZReader(cRequest.Response);
				cReader.Charset = "big5";
				ZBuffer cBuffer = Decode(cReader.ReadText());
				if (cBuffer != null && cBuffer.Length > 0) {
					string sFile = string.Format("{0}OI\\{1}.dat", GlobalSettings.Paths.DatabasePath, date.ToString("yyyyMMdd"));
					Write(sFile, cBuffer);
				}
			}
			cRequest.Close();
		}

		private static void Write(string file, ZBuffer buffer) {
			using (FileStream cStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				cStream.Write(buffer.Data, 0, buffer.Length);
			}
		}
	}
}