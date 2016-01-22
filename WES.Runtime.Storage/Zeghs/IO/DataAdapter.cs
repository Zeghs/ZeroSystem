using System;
using System.Net;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.IO {
	internal sealed class DataAdapter : IDisposable {
		private const int DATA_BLOCK_SIZE = 48;
		private static string __sHttpUrl = null;

		internal static void SetRequestUrl(string domain, string target) {
			__sHttpUrl = domain + target;
		}

		internal event EventHandler<DataAdapterCompleteEvent> onCompleted = null;

		private long __lPosition = -1;
		private bool __bDisposed = false;
		private CookieContainer __cCookies = null;
		private SeriesSymbolData __cSeries = null;

		internal SeriesSymbolData Series {
			get {
				return __cSeries;
			}
		}

		internal DataAdapter(InstrumentDataRequest request) {
			__cCookies = new CookieContainer();
			int iCount = request.Range.Count;

			__cSeries = new SeriesSymbolData(request);
			__cSeries.onRequest += SeriesSymbolData_onRequest;

			__cSeries.OnRequest(new DataRequestEvent(iCount, iCount, 1));  //請求歷史資料
			__cSeries.MergeTicks();  //合併即時Tick資訊
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		/// <param name="disposing">是否正在處理資源中</param>
		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					__cSeries.RemoveRequest();  //如果資料配置者已經釋放就移除請求事件(因為資料還需要保存在 SeriesStorage 內, 所以不可以 Dispose)
					onCompleted = null;
				}
			}
		}

		private void Request(DataRequestEvent e) {
			InstrumentDataRequest cDataRequest = __cSeries.DataRequest;
			DataRequest cRange = cDataRequest.Range;
			
			ZRequest cRequest = new ZRequest();
			cRequest.Method = "POST";
			cRequest.Url = __sHttpUrl;
			cRequest.Parameters = string.Format("exchange={0}&symbolId={1}&timeFrame={2}&position={3}&startDate={4}&endDate={5}&count={6}", cDataRequest.Exchange, cDataRequest.Symbol, cDataRequest.Resolution.TotalSeconds, __lPosition, cRange.From.ToString("yyyy-MM-dd"), cRange.To.ToString("yyyy-MM-dd"), (cRange.RequestType == DataRequestType.BarsBack || __lPosition != -1) ? e.Count : cRange.Count);
			cRequest.CookieContainer = __cCookies;

			int iRet = cRequest.Request();
			if (iRet == 0) {
				ZReader cReader = new ZReader(cRequest.Response);
				e.Result = cReader.Result;

				if (cReader.Result == 0) {
					int iCount = e.Count = cReader.Count;
					__cSeries.AdjustSize(iCount, true);

					ZBuffer cBuffer = cReader.Read();
					while (--iCount >= 0) {
						cBuffer.Position = iCount * DATA_BLOCK_SIZE;
						DateTime cDate = cBuffer.GetDateTime();
						double dOpen = cBuffer.GetDouble();
						double dHigh = cBuffer.GetDouble();
						double dLow = cBuffer.GetDouble();
						double dClose = cBuffer.GetDouble();
						double dVolume = cBuffer.GetDouble();

						__cSeries.AddSeries(cDate, dOpen, dHigh, dLow, dClose, dVolume, false);
					}

					__lPosition = cReader.Position;
					e.Ranges[0] = cReader.BeginDate;
					e.Ranges[1] = cReader.EndDate;

					if (__lPosition == 0) {
						e.IsAlreadyRequestAllData = true;
					}
				}
			}
		}

		private void SeriesSymbolData_onRequest(object sender, DataRequestEvent e) {
			int iCount = e.Count;
			int iBaseCount = __cSeries.DataRequest.Range.Count;
			if (e.Position > iBaseCount) {
				Request(e);

				if (e.Result == 0) {
					e.Count += iBaseCount;
					if (e.IsAlreadyRequestAllData) {
						__cSeries.RemoveRequest();  ////如果已經全部讀取完畢就取消事件

						if (onCompleted != null) {
							InstrumentDataRequest cDataRequest = __cSeries.DataRequest;
							onCompleted(this, new DataAdapterCompleteEvent(cDataRequest.Symbol, cDataRequest.Resolution.TotalSeconds));
						}
					}
				}
			} else {
				DataRequest cDataRequest = __cSeries.DataRequest.Range;

				e.Result = 0;
				e.Count = iBaseCount;
				e.Ranges = new DateTime[] { cDataRequest.From, cDataRequest.To };
				e.IsAlreadyRequestAllData = cDataRequest.IsAlreadyRequestAllData;
			}
		}
	}
}