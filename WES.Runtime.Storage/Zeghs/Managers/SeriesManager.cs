using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.IO;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Services;

namespace Zeghs.Managers {
	/// <summary>
	///   股票資訊管理者類別
	/// </summary>
	public sealed class SeriesManager {
		private static readonly ILog logger = LogManager.GetLogger(typeof(SeriesManager));
		private static readonly SeriesManager __current = new SeriesManager();

		private static SeriesSettings __cSeriesSettings = null;

		/// <summary>
		///   [取得] Series 設定資訊
		/// </summary>
		public static SeriesSettings Settings {
			get {
				return __cSeriesSettings;
			}
		}

		/// <summary>
		///   [取得] SeriesManager 類別
		/// </summary>
		public static SeriesManager Manager {
			get {
				return __current;
			}
		}

		/// <summary>
		///   讀取 Series 設定值
		/// </summary>
		public static void LoadSettings() {
			string sFileName = "WES.Runtime.Storage.set";
			
			if (File.Exists(sFileName)) {  //檔案存在就讀取
				string sSettings = File.ReadAllText(sFileName, Encoding.UTF8);
				__cSeriesSettings = JsonConvert.DeserializeObject<SeriesSettings>(sSettings);

				DataAdapter.SetRequestUrl(__cSeriesSettings.HttpDomain, __cSeriesSettings.TargetUrl);
			}
		}

		/// <summary>
		///   儲存 Series 設定值
		/// </summary>
		public static void SaveSettings() {
			string sLocation = Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			string sSettings = JsonConvert.SerializeObject(__cSeriesSettings, Formatting.Indented);
			File.WriteAllText(sFileName, sSettings, Encoding.UTF8);
		}

		private Queue<QuoteEvent> __cQueue = null;
		private HashSet<string> __cDataSources = null;
		private Dictionary<string, DataAdapter> __cMinBases = null;
		private Dictionary<string, DataAdapter> __cDayBases = null;
		private Dictionary<string, SeriesStorage> __cStorages = null;

		private bool __bBusy = false;
		private object __oLock = new object();

		private SeriesManager() {
			__cDataSources = new HashSet<string>();
			__cQueue = new Queue<QuoteEvent>(4096);
			
			__cMinBases = new Dictionary<string, DataAdapter>(128);
			__cDayBases = new Dictionary<string, DataAdapter>(128);
			__cStorages = new Dictionary<string, SeriesStorage>(256);
		}
		
		/// <summary>
		///   取得開高低收資訊列表
		/// </summary>
		/// <param name="dataRequest">資料請求結構</param>
		/// <returns>返回值: SeriesSymbolData 類別</returns>
		public SeriesSymbolData GetSeries(InstrumentDataRequest dataRequest) {
			SeriesStorage cStorage = null;
			string sLSymbolId = dataRequest.Symbol.ToLower();
			lock (__cStorages) {
				if (!__cStorages.TryGetValue(sLSymbolId, out cStorage)) {
					DataAdapter cAdapter = LoadAdapter(dataRequest);
					if (cAdapter.Series == null) {
						return null;  //表示沒有檔案
					} else {
						cStorage = new SeriesStorage(16);
						cStorage.Add(cAdapter.Series);

						__cStorages.Add(sLSymbolId, cStorage);
					}
				}
			}

			int iTotalSeconds = dataRequest.Resolution.TotalSeconds;
			SeriesSymbolData cSeries = cStorage.GetSeries(iTotalSeconds);
			if (cSeries == null) {
				int iBaseSeconds = (iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) ? Resolution.MIN_BASE_TOTALSECONDS : Resolution.MAX_BASE_TOTALSECONDS;
				cSeries = cStorage.GetSeries(iBaseSeconds);
				if (cSeries == null) {
					DataAdapter cAdapter = LoadAdapter(dataRequest);
					if (cAdapter.Series != null) {
						cSeries = cAdapter.Series;
						cStorage.Add(cSeries);
					}
				}

				if (iBaseSeconds != iTotalSeconds) {
					cSeries = cStorage.Create(dataRequest);
				}
			}
			return cSeries;
		}

		/// <summary>
		///   設定報價資訊服務
		/// </summary>
		/// <param name="quoteService">報價資訊服務</param>
		public void SetQuoteService(AbstractQuoteService quoteService) {
			string sDataSource = quoteService.DataSource;
			lock (__cDataSources) {
				if (!__cDataSources.Contains(sDataSource)) {
					quoteService.onQuote += QuoteService_onQuote;
					quoteService.onReset += QuoteService_onReset;
					
					__cDataSources.Add(sDataSource);
				}
			}
		}

		private void AsyncMergeTick() {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bBusy;
				if (!bBusy) {
					__bBusy = true;
				}
			}

			if (!bBusy) {
				Task.Factory.StartNew(() => {
					while (__cQueue.Count > 0) {
						QuoteEvent cQuoteEvent = null;
						lock (__cQueue) {
							cQuoteEvent = __cQueue.Dequeue();
						}

						SeriesStorage cStorage = null;
						string sLSymbolId = cQuoteEvent.Quote.SymbolId.ToLower();
						lock (__cStorages) {
							__cStorages.TryGetValue(sLSymbolId, out cStorage);
						}

						if (cStorage != null) {
							cStorage.MergeTick(cQuoteEvent.Tick);
						}
					}

					lock (__oLock) {
						__bBusy = false;
					}
				});
			}
		}

		private DataAdapter LoadAdapter(InstrumentDataRequest dataRequest) {
			dataRequest.Resolution = Resolution.GetBaseValue(dataRequest.Resolution);
			DataAdapter cAdapter = new DataAdapter(dataRequest);
			
			SeriesSymbolData cSeries = cAdapter.Series;
			if (cSeries == null) {
				return null;
			}

			if (!cSeries.DataRequest.Range.IsAlreadyRequestAllData) {
				cAdapter.onCompleted += DataAdapter_onCompleted;

				string sLSymbolId = dataRequest.Symbol.ToLower();
				int iTotalSeconds = cSeries.DataRequest.Resolution.TotalSeconds;
				if (iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) {
					lock (__cMinBases) {
						if (!__cMinBases.ContainsKey(sLSymbolId)) {
							__cMinBases.Add(sLSymbolId, cAdapter);
						}
					}
				} else {
					lock (__cDayBases) {
						if (!__cDayBases.ContainsKey(sLSymbolId)) {
							__cDayBases.Add(sLSymbolId, cAdapter);
						}
					}
				}
			}
			return cAdapter;
		}
		
		private void DataAdapter_onCompleted(object sender, DataAdapterCompleteEvent e) {
			DataAdapter cAdapter = sender as DataAdapter;
			cAdapter.Dispose();

			int iTimeFrame = e.TotalSeconds;
			string sSymbolId = e.SymbolId.ToLower();
			if (iTimeFrame < Resolution.MAX_BASE_TOTALSECONDS) {
				lock (__cMinBases) {
					if (__cMinBases.ContainsKey(sSymbolId)) {
						__cMinBases.Remove(sSymbolId);
					}
				}
			} else {
				lock (__cDayBases) {
					if (__cDayBases.ContainsKey(sSymbolId)) {
						__cDayBases.Remove(sSymbolId);
					}
				}
			}
		}

		private void QuoteService_onQuote(object sender, QuoteEvent e) {
			lock (__cQueue) {
				__cQueue.Enqueue(e);
			}
			
			AsyncMergeTick();
		}

		private void QuoteService_onReset(object sender, QuoteResetEvent e) {
			string sDataSource = e.DataSource;
			lock (__cStorages) {
				foreach (SeriesStorage cStorage in __cStorages.Values) {
					cStorage.Reset(sDataSource);
				}
			}
		}
	}
}  //250行