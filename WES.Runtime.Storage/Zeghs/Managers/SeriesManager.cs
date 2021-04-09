using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
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
			string sFileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".set";
			
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

		/// <summary>
		///   設定歷史資料裝置建立者介面
		/// </summary>
		/// <param name="deviceCreator">歷史資料裝置建立者介面</param>
		public static void SetDeviceCreator(IDeviceCreator deviceCreator) {
			DataAdapter.SetDeviceCreator(deviceCreator);
		}

		private sealed class _AsyncEventArgs {
			internal EventWaitHandle handle = null;
			internal InstrumentDataRequest request;
		}

		private Queue<QuoteEvent> __cQueue = null;
		private HashSet<string> __cDataSources = null;
		private Dictionary<string, DataAdapter> __cMinBases = null;
		private Dictionary<string, DataAdapter> __cDayBases = null;
		private Dictionary<string, SeriesStorage> __cStorages = null;
		private Dictionary<string, _AsyncEventArgs> __cAsyncArgs = null;

		private bool __bBusy = false;
		private object __oLock = new object();

		private SeriesManager() {
			__cDataSources = new HashSet<string>();
			__cQueue = new Queue<QuoteEvent>(4096);
			
			__cMinBases = new Dictionary<string, DataAdapter>(128);
			__cDayBases = new Dictionary<string, DataAdapter>(128);
			__cStorages = new Dictionary<string, SeriesStorage>(256);
			__cAsyncArgs = new Dictionary<string, _AsyncEventArgs>(32);

			//附掛開啟或關閉事件(當使用者開啟或關閉即時報價來源時會觸發的事件, 用來偵測是否報價資訊源被關閉)
			QuoteManager.Manager.onQuoteServiceSwitchChanged += QuoteManager_onQuoteServiceSwitchChanged;
		}

		/// <summary>
		///   非同步模式取得序列商品資訊
		/// </summary>
		/// <param name="dataRequest">資料請求結構</param>
		/// <param name="result">序列商品資訊回報事件</param>
		/// <param name="useCache">是否使用快取 [預設:true](true=序列資料結構建立後保存在快取內，下次需要使用直接從快取拿取, false=重新建立序列資料結構，建立的序列資料需要自行移除否則會占用記憶體空間)</param>
		/// <param name="args">使用者自訂參數</param>
		/// <param name="millisecondsTimeout">回補資料 Timeout 毫秒數 [預設:System.Threading.Timeout.Infinite (永遠等待直到回補完成)]</param>
		public void AsyncGetSeries(InstrumentDataRequest dataRequest, EventHandler<SeriesResultEvent> result, bool useCache = true, object args = null, int millisecondsTimeout = Timeout.Infinite) {
			Task.Factory.StartNew(() => {
				CheckLogin(dataRequest.DataFeed);
				Complement(dataRequest, millisecondsTimeout);

				SeriesSymbolDataRand cSeries = InternalGetSeries(dataRequest, useCache);
				result(this, new SeriesResultEvent(cSeries, args));
			});
		}

		/// <summary>
		///   取得序列商品資訊
		/// </summary>
		/// <param name="dataRequest">資料請求結構</param>
		/// <param name="useCache">是否使用快取 [預設:true](true=序列資料結構建立後保存在快取內，下次需要使用直接從快取拿取, false=重新建立序列資料結構，建立的序列資料需要自行移除否則會占用記憶體空間)</param>
		/// <param name="millisecondsTimeout">回補資料 Timeout 毫秒數 [預設:System.Threading.Timeout.Infinite (永遠等待直到回補完成)]</param>
		/// <returns>返回值: SeriesSymbolDataRand 類別</returns>
		public SeriesSymbolDataRand GetSeries(InstrumentDataRequest dataRequest, bool useCache = true, int millisecondsTimeout = Timeout.Infinite) {
			CheckLogin(dataRequest.DataFeed);
			Complement(dataRequest, millisecondsTimeout);
			
			return InternalGetSeries(dataRequest, useCache);
		}

		/// <summary>
		///   設定報價資訊服務
		/// </summary>
		/// <param name="dataSource">報價資訊來源名稱</param>
		public void SetQuoteService(string dataSource) {
			lock (__cDataSources) {
				if (!__cDataSources.Contains(dataSource)) {
					AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(dataSource);
					if (cService != null) {
						if (!cService.IsLogin) {
							cService.onLoginCompleted += QuoteService_onLoginCompleted;
						}

						cService.onQuote += QuoteService_onQuote;
						cService.onReset += QuoteService_onReset;
						cService.onComplementCompleted += QuoteService_onComplementCompleted;
						
						__cDataSources.Add(dataSource);
					}
				}
			}
		}

		/// <summary>
		///   移除商品資訊(如果 GetSeries 不是使用 useCache 模式都需要移除)
		/// </summary>
		/// <param name="seriesSymbolDataRand">商品資訊類別</param>
		internal void RemoveSeries(SeriesSymbolDataRand seriesSymbolDataRand) {
			SeriesSymbolData cSeries = seriesSymbolDataRand.Source;

			SeriesStorage cStorage = null;
			string sLSymbolId = cSeries.DataRequest.Symbol.ToLower();
			lock (__cStorages) {
				__cStorages.TryGetValue(sLSymbolId, out cStorage);
			}

			if (cStorage != null) {
				if (cSeries.Id > 0x40000000) {  //Id 編號從 0x40000001 開始編號(如果低於表示使用時間週期總秒數當作 Hash, 使用時間週期總秒數都是 Cache 資料所以不能移除) 
					cStorage.Remove(cSeries.Id);
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

		private void CheckLogin(string dataSource) {
			if (__cDataSources.Contains(dataSource)) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(dataSource);
				if (cService != null) {
					if (!cService.IsLogin) {
						EventWaitHandle cWaitHandle = null;
						lock (__cAsyncArgs) {
							_AsyncEventArgs cArgs = null;
							if (__cAsyncArgs.TryGetValue(dataSource, out cArgs)) {
								cWaitHandle = cArgs.handle;
							} else {
								cArgs = new _AsyncEventArgs();
								cWaitHandle = new ManualResetEvent(false);
								cArgs.handle = cWaitHandle;
								
								__cAsyncArgs.Add(dataSource, cArgs);
							}
						}

						if (cWaitHandle != null) {
							cWaitHandle.WaitOne();
						}
					}
				}
			}
		}

		private void Complement(InstrumentDataRequest request, int millisecondsTimeout) {
			string sDataSource = request.DataFeed;
			if (__cDataSources.Contains(sDataSource)) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
				if (cService != null) {
					string sSymbolId = request.Symbol;
					IQuote cQuote = cService.Storage.GetQuote(sSymbolId);
					if (cQuote != null && cQuote.ComplementStatus != ComplementStatus.Complemented) {
						EventWaitHandle cWaitHandle = null;
						lock (__cAsyncArgs) {
							_AsyncEventArgs cArgs = null;
							string sHashKey = string.Format("{0}_{1}", sDataSource, sSymbolId);
							if (__cAsyncArgs.TryGetValue(sHashKey, out cArgs)) {
								cWaitHandle = cArgs.handle;
							} else {
								if (cQuote.ComplementStatus == ComplementStatus.NotComplement) {
									cArgs = new _AsyncEventArgs();
									cArgs.request = request;
									
									cWaitHandle = new ManualResetEvent(false);
									cArgs.handle = cWaitHandle;
									__cAsyncArgs.Add(sHashKey, cArgs);

									cService.AddSubscribe(sSymbolId);
									cService.Complement(sSymbolId);
								}
							}
						}

						if (cWaitHandle != null) {
							if (!cWaitHandle.WaitOne(millisecondsTimeout)) {  //等待回補資訊(如果 millisecondsTimeout 為 -1 則會無限等待, 如果有設定 Timeout 在時間到了之後還沒回補完畢就直接釋放 WaitHandle 元件並移除)
								cWaitHandle.Dispose();
								
								string sHashKey = string.Format("{0}_{1}", sDataSource, sSymbolId);
								lock (__cAsyncArgs) {
									__cAsyncArgs.Remove(sHashKey);
								}
							}
						}
					}
				}
			}
		}

		private SeriesSymbolDataRand InternalGetSeries(InstrumentDataRequest dataRequest, bool useCache) {
			SeriesStorage cStorage = null;
			string sLSymbolId = dataRequest.Symbol.ToLower();
			lock (__cStorages) {
				if (!__cStorages.TryGetValue(sLSymbolId, out cStorage)) {
					cStorage = new SeriesStorage(16);
					__cStorages.Add(sLSymbolId, cStorage);
				}
			}

			SeriesSymbolDataRand cSeriesRand = null;
			int iTotalSeconds = dataRequest.Resolution.TotalSeconds;
			if (useCache) {  //是否使用快取
				lock (cStorage) {  //需要 lock 這個區塊(避免非同步讀取資料時發生問題)
					int iBaseSeconds = (iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) ? Resolution.MIN_BASE_TOTALSECONDS : Resolution.MAX_BASE_TOTALSECONDS;
					SeriesSymbolData cSeries = cStorage.GetSeries(iTotalSeconds);
					if (cSeries == null) {
						cSeries = cStorage.GetSeries(iBaseSeconds);
						if (cSeries == null) {
							DataAdapter cAdapter = LoadAdapter(ref dataRequest);
							cSeries = cAdapter.Series;
							cStorage.Add(cSeries);
						}
					}

					if (iBaseSeconds == iTotalSeconds) {
						goto exit;
					} else {
						cSeries = cSeries.CreateSeries(dataRequest); //利用基礎周期建立其他的資料周期
						cStorage.Add(cSeries);  //加入至 SeriesStorage
					}

					dataRequest.Resolution = cSeries.DataRequest.Resolution;  //將目標的週期結構更新至傳入的 InstrumentDataRequest 週期結構
					
					DataRequestEvent cRequestEvent = new DataRequestEvent(dataRequest);
					cSeries.OnRequest(cRequestEvent);  //如果已經存在則請求使用者需要的歷史資料區間(請求方法會檢查目前已下載的歷史資料區間是否足夠, 如果使用者需要的歷史資料區間比較大會向伺服器請求)
					dataRequest.Range.Count = cRequestEvent.Count;  //將請求後的正確數量傳入至結構內
				exit:
					cSeriesRand = new SeriesSymbolDataRand(cSeries, dataRequest);
				}
			} else {
				DataAdapter cAdapter = LoadAdapter(ref dataRequest, false);  //重新建立新的基礎週期序列資料(不使用快取, 不保存至快取內, 使用完畢之後立即 Dispose)
				SeriesSymbolData cSeries = cAdapter.Series;  //取得新的基礎周期序列資料

				int iBaseSeconds = (iTotalSeconds < Resolution.MAX_BASE_TOTALSECONDS) ? Resolution.MIN_BASE_TOTALSECONDS : Resolution.MAX_BASE_TOTALSECONDS;
				if (iBaseSeconds != iTotalSeconds) {
					SeriesSymbolData cTargetSeries = cSeries.CreateSeries(dataRequest);  //使用 InstrumentDataRequest 建立新的其他週期序列資料
					cSeries.Merge(cTargetSeries);  //將基礎周期序列資料合併至新的其他週期序列資料
					cSeries.Dispose();  //釋放基礎周期序列資料

					cSeries = cTargetSeries;
				}

				cSeriesRand = new SeriesSymbolDataRand(cSeries, dataRequest);
				cStorage.Add(cSeries, true);  //保存序列資料(存放在 SeriesStorage 內的序列資料才會自動合併最新的即時資訊報價)
				cAdapter.Dispose();  //釋放資料配置者類別
			}
			return cSeriesRand;
		}

		private DataAdapter LoadAdapter(ref InstrumentDataRequest dataRequest, bool useCache = true) {
			InstrumentDataRequest cRequest = dataRequest;
			cRequest.Resolution = Resolution.GetBaseValue(dataRequest.Resolution);
			DataAdapter cAdapter = new DataAdapter(cRequest);

			InstrumentDataRequest cDataRequest = cAdapter.Series.DataRequest;
			dataRequest.Range.Count = cDataRequest.Range.Count;  //將正確的請求歷史資料數量傳入至結構

			if (useCache) {  //使用快取(true=建立完 DataAdapter 之後，將其保存至快取內以方便後續請求時使用)
				if (!cDataRequest.Range.IsAlreadyRequestAllData) {
					cAdapter.onCompleted += DataAdapter_onCompleted;

					string sLSymbolId = dataRequest.Symbol.ToLower();
					int iTotalSeconds = cDataRequest.Resolution.TotalSeconds;
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

		private void QuoteService_onComplementCompleted(object sender, QuoteComplementCompletedEvent e) {
			string sHashKey = string.Format("{0}_{1}", e.DataSource, e.SymbolId);

			_AsyncEventArgs cArgs = null;
			lock (__cAsyncArgs) {
				if (__cAsyncArgs.TryGetValue(sHashKey, out cArgs)) {
					__cAsyncArgs.Remove(sHashKey);
				}
			}

			if (cArgs != null) {
				EventWaitHandle cWaitHandle = cArgs.handle;
				if (cWaitHandle != null) {
					cWaitHandle.Set();
					cWaitHandle.Dispose();
				}
			}
		}

		private void QuoteService_onLoginCompleted(object sender, EventArgs e) {
			AbstractQuoteService cService = sender as AbstractQuoteService;
			cService.onLoginCompleted -= QuoteService_onLoginCompleted;
			
			_AsyncEventArgs cArgs = null;
			string sDataSource = cService.DataSource;
			lock (__cAsyncArgs) {
				if (__cAsyncArgs.TryGetValue(sDataSource, out cArgs)) {
					__cAsyncArgs.Remove(sDataSource);
				}
			}

			if (cArgs != null) {
				EventWaitHandle cWaitHandle = cArgs.handle;
				cWaitHandle.Set();
				cWaitHandle.Dispose();
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

		private void QuoteManager_onQuoteServiceSwitchChanged(object sender, QuoteServiceSwitchChangedEvent e) {
			if (e.IsRunning) {  //如果使用者開啟了即時報價資訊源
				this.SetQuoteService(e.DataSource);  //設定即時報價資訊源
			} else {  //如果使用者關閉了即時報價資訊源
				string sDataSource = e.DataSource;  //取得報價資料來源名稱
				lock (__cDataSources) {
					if (__cDataSources.Contains(sDataSource)) {  //如果有報價資料來源
						__cDataSources.Remove(sDataSource);  //移除報價資料來源名稱
					}
				}
			}
		}
	}
}  //468行