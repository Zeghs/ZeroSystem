using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Web;
using Zeghs.Data;

namespace Zeghs.Utils {
	public sealed class ForeignInvestmentUtil {
		private static CookieContainer __cCookies = new CookieContainer();

		private static Dictionary<string, string> __cSymbols = new Dictionary<string, string>() {
			{ "臺股期貨", "TXF0.tw" }, { "電子期貨", "EXF0.tw" }, { "金融期貨", "FXF0.tw" }, { "小型臺指期貨", "MXF0.tw" }
		};

		public static ForeignInvestmentGroup Load(DateTime date) {
			ForeignInvestmentGroup cData = new ForeignInvestmentGroup();

			string sFile = string.Format("{0}FI\\{1}.dat", GlobalSettings.Paths.DatabasePath, date.ToString("yyyyMMdd"));
			if (File.Exists(sFile)) {
				byte[] bArray = File.ReadAllBytes(sFile);
				ZBuffer cBuffer = new ZBuffer();
				cBuffer.Data = bArray;
				cBuffer.Length = bArray.Length;

				for (int i = 0; i < 4; i++) {
					string sSymbolId = cBuffer.GetString();
					cData.Add(sSymbolId, ForeignInvestmentData.Create(cBuffer));
				}
			}
			return cData;
		}

		public static void Download(DateTime date) {
			string sDate = string.Format("{0}%2F{1}%2F{2}", date.Year, date.Month, date.Day);

			ZRequest cRequest = new ZRequest();
			cRequest.Method = "POST";
			cRequest.Url = "https://www.taifex.com.tw/chinese/3/7_12_8dl.asp";
			cRequest.CookieContainer = __cCookies;
			cRequest.Parameters = string.Format("goday=&DATA_DATE_Y={0}&DATA_DATE_M={1}&DATA_DATE_D={2}&DATA_DATE_Y_E={0}&DATA_DATE_M_E={1}&DATA_DATE_D_E={2}&syear={0}&smonth={1}&sday={2}&eyear={0}&emonth={1}&eday={2}&datestart={3}&dateend={3}&COMMODITY_ID=", date.Year, date.Month, date.Day, sDate);
			int iRet = cRequest.Request();
			if (iRet == 0) {
				ZReader cReader = new ZReader(cRequest.Response);
				cReader.Charset = "big5";
				ZBuffer cBuffer = Decode(cReader.ReadText());
				if (cBuffer != null && cBuffer.Length > 0) {
					string sFile = string.Format("{0}FI\\{1}.dat", GlobalSettings.Paths.DatabasePath, date.ToString("yyyyMMdd"));
					Write(sFile, cBuffer);
				}
			}
			cRequest.Close();
		}

		private static ZBuffer Decode(string content) {
			ZBuffer cBuffer = null;
			string[] sDatas = content.Split(new char[] { '\r', '\n' });
			if (sDatas[0][0] == '<') {
				return cBuffer;
			}

			int iLength = sDatas.Length;
			if (iLength > 1) {
				int iCount = 0;
				cBuffer = new ZBuffer(iLength * 20);

				for (int i = 1; i < iLength; i++) {
					string sData = sDatas[i];
					if (sData.Length > 0) {
						string sSymbolId = null;
						string[] sItems = sData.Split(',');
						if (__cSymbols.TryGetValue(sItems[1], out sSymbolId)) {
							if (iCount == 0) {
								cBuffer.Add(sSymbolId);
							}

							cBuffer.Add(int.Parse(sItems[3]));  //多方交易量
							cBuffer.Add(int.Parse(sItems[5]));  //空方交易量
							cBuffer.Add(int.Parse(sItems[9]));  //多方留倉口
							cBuffer.Add(int.Parse(sItems[11])); //空方留倉口

							if (++iCount == 3) {
								iCount = 0;
							}
						}
					}
				}
			}
			return cBuffer;
		}

		private static void Write(string file, ZBuffer buffer) {
			using (FileStream cStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				cStream.Write(buffer.Data, 0, buffer.Length);
			}
		}
	}
}