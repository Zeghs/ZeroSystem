using System;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;

namespace Zeghs.IO {
	internal sealed class DataAdapter : IDisposable {
		private static IDeviceCreator __cDeviceCreator = new HttpPostCreator();

		internal static void SetRequestUrl(string domain, string target) {
			HttpPostCreator.SetRequestUrl(domain + target);
		}

		internal event EventHandler<DataAdapterCompleteEvent> onCompleted = null;

		private bool __bDisposed = false;
		private AbstractDevice __cDevice = null;
		private SeriesSymbolData __cSeries = null;

		internal SeriesSymbolData Series {
			get {
				return __cSeries;
			}
		}

		internal DataAdapter(InstrumentDataRequest request) {
			__cSeries = new SeriesSymbolData(request);
			__cSeries.onRequest += SeriesSymbolData_onRequest;

			__cDevice = __cDeviceCreator.Create();
			__cDevice.SetSeries(__cSeries);

			__cSeries.OnRequest(new DataRequestEvent(request));  //請求歷史資料
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

		private void SeriesSymbolData_onRequest(object sender, DataRequestEvent e) {
			int iCount = e.Count;
			DataRequest cDataRequest = __cSeries.DataRequest.Range;
			int iBaseCount = cDataRequest.Count;
			bool bRequest = e.Totals > iBaseCount;
			if (!bRequest) {
				bRequest = (__cDevice.Position == -1) ? true : e.CheckRequest(cDataRequest);
			}

			if (bRequest) {
				__cDevice.Request(e);

				if (e.Result == 0) {
					e.Count += iBaseCount;
					
					if (e.IsAlreadyRequestAllData) {
						__cSeries.RemoveRequest();  //如果已經全部讀取完畢就取消事件

						if (onCompleted != null) {
							InstrumentDataRequest cInstDataRequest = __cSeries.DataRequest;
							onCompleted(this, new DataAdapterCompleteEvent(cInstDataRequest.Symbol, cInstDataRequest.Resolution.TotalSeconds));
						}
					}
				}
			} else {
				e.Result = 0;
				e.Count = iBaseCount;
				e.Ranges = new DateTime[] { cDataRequest.From, cDataRequest.To };
				e.IsAlreadyRequestAllData = cDataRequest.IsAlreadyRequestAllData;
			}
		}
	}
}