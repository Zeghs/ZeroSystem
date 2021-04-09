using System;
using System.Net;
using PowerLanguage;
using Zeghs.Events;

namespace Zeghs.IO {
	internal sealed class HttpPostCreator : IDeviceCreator {
		internal static void SetRequestUrl(string httpUrl) {
			HttpPostDevice.SetRequestUrl(httpUrl);
		}

		public AbstractDevice Create() {
			return new HttpPostDevice();
		}
	}

	internal sealed class HttpPostDevice : AbstractDevice {
		private const int DATA_BLOCK_SIZE = 48;
		private static string __sHttpUrl = null;

		internal static void SetRequestUrl(string httpUrl) {
			__sHttpUrl = httpUrl;
		}

		private CookieContainer __cCookies = null;

		internal HttpPostDevice() {
			__cCookies = new CookieContainer();
		}

		/// <summary>
		///   請求商品歷史資料
		/// </summary>
		/// <param name="e">資料請求事件</param>
		protected internal override void Request(DataRequestEvent e) {
			InstrumentDataRequest cDataRequest = this.DataRequest;
			DateTime[] cRanges = e.Ranges;

			ZRequest cRequest = new ZRequest();
			cRequest.Method = "POST";
			cRequest.Url = __sHttpUrl;
			cRequest.Parameters = string.Format("exchange={0}&symbolId={1}&timeFrame={2}&position={3}&startDate={4}&endDate={5}&count={6}", cDataRequest.Exchange, cDataRequest.Symbol, cDataRequest.Resolution.TotalSeconds, this.Position, cRanges[0].ToString("yyyy-MM-dd"), cRanges[1].ToString("yyyy-MM-dd"), e.Count);
			cRequest.CookieContainer = __cCookies;

			int iRet = cRequest.Request();
			if (iRet == 0) {
				ZReader cReader = new ZReader(cRequest.Response);
				e.Result = cReader.Result;

				if (cReader.Result == 0) {
					int iCount = e.Count = cReader.Count;
					this.AdjustSize(iCount);

					ZBuffer cBuffer = cReader.Read();
					while (--iCount >= 0) {
						cBuffer.Position = iCount * DATA_BLOCK_SIZE;
						DateTime cDate = cBuffer.GetDateTime();
						double dOpen = cBuffer.GetDouble();
						double dHigh = cBuffer.GetDouble();
						double dLow = cBuffer.GetDouble();
						double dClose = cBuffer.GetDouble();
						double dVolume = cBuffer.GetDouble();

						this.AddSeries(cDate, dOpen, dHigh, dLow, dClose, dVolume);
					}

					this.Position = cReader.Position;
					e.Ranges[0] = cReader.BeginDate;
					e.Ranges[1] = cReader.EndDate;

					if (this.Position == 0) {
						e.IsAlreadyRequestAllData = true;
					}
				}
			}
		}
	}
}