using System;
using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Settings {
	/// <summary>
	///   資料請求設定類別
	/// </summary>
	public sealed class RequestSetting {
		/// <summary>
		///   將設定檔轉換為 InstrumentDataRequest 列表的格式
		/// </summary>
		/// <param name="requests">RequestSetting 陣列</param>
		/// <returns>返回值: InstrumentDataRequest 列表</returns>
		public static List<InstrumentDataRequest> Convert(RequestSetting[] requests) {
			List<InstrumentDataRequest> cResult = new List<InstrumentDataRequest>();

			foreach (RequestSetting cRequest in requests) {
				string[] sPeriods = cRequest.DataPeriod.Split(',');
				int iPeriodSize = int.Parse(sPeriods[0]);
				EResolution cResolution = (EResolution) Enum.Parse(typeof(EResolution), sPeriods[1]);

				InstrumentDataRequest cDataRequest = new InstrumentDataRequest() {
					Exchange = cRequest.Exchange,
					DataFeed = cRequest.DataFeed,
					Resolution = new Resolution(cResolution, iPeriodSize),
					Symbol = cRequest.SymbolId
				};

				string[] sParams = cRequest.Range.Split(',');
				string sMode = sParams[0];
				string[] sArgs = sParams[1].Split(';');

				switch (sMode) {
					case "barsBack":
						cDataRequest.Range = DataRequest.CreateBarsBack(DateTime.Parse(sArgs[0]), int.Parse(sArgs[1]));
						break;
					case "daysBack":
						cDataRequest.Range = DataRequest.CreateDaysBack(DateTime.Parse(sArgs[0]), int.Parse(sArgs[1]));
						break;
					case "fromTo":
						cDataRequest.Range = DataRequest.CreateFromTo(DateTime.Parse(sArgs[0]), DateTime.Parse(sArgs[1]));
						break;
				}
				cResult.Add(cDataRequest);
			}
			return cResult;
		}

		/// <summary>
		///   [取得/設定] 即時報價來源
		/// </summary>
		public string DataFeed {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 資料週期
		/// </summary>
		public string DataPeriod {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 證交所簡稱
		/// </summary>
		public string Exchange {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 請求區間
		/// </summary>
		public string Range {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商品代號
		/// </summary>
		public string SymbolId {
			get;
			set;
		}
	}
}