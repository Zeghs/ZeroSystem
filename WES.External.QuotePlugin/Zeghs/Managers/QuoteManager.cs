using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Informations;

namespace Zeghs.Managers {
	/// <summary>
	///   報價元件管理員
	/// </summary>
	public sealed class QuoteManager {
		private static readonly QuoteManager __current = new QuoteManager();

		/// <summary>
		///   [取得] QuoteManager類別
		/// </summary>
		public static QuoteManager Manager {
			get {
				return __current;
			}
		}

		/// <summary>
		///   當報價服務被開啟或關閉時會觸發的事件
		/// </summary>
		public event EventHandler<QuoteServiceSwitchChangedEvent> onQuoteServiceSwitchChanged = null;

		private Dictionary<string, int> __cIndexs = null;
		private List<AbstractQuoteService> __cQuoteServices = null;
		private Dictionary<string, QuoteServiceInformation> __cQuoteServiceInfos = null;

		/// <summary>
		///   [取得] 所有即時報價元件
		/// </summary>
		public List<AbstractQuoteService> QuoteServices {
			get {
				return __cQuoteServices;
			}
		}

		private QuoteManager() {
			__cIndexs = new Dictionary<string, int>(128);
			__cQuoteServices = new List<AbstractQuoteService>(128);
			__cQuoteServiceInfos = new Dictionary<string, QuoteServiceInformation>(128);
		}

		/// <summary>
		///   關閉所有即時報價服務
		/// </summary>
		public void CloseAll() {
			lock (__cQuoteServiceInfos) {
				foreach (QuoteServiceInformation cQuoteServiceInfo in __cQuoteServiceInfos.Values) {
					if (cQuoteServiceInfo.Enabled) {
						RemoveQuoteService(cQuoteServiceInfo.DataSource);
					}
				}
			}
		}

		/// <summary>
		///   取得即時報價服務
		/// </summary>
		/// <param name="dataSource">報價元件名稱</param>
		/// <returns>返回值: AbstractQuoteService 報價服務抽象類別</returns>
		public AbstractQuoteService GetQuoteService(string dataSource) {
			int iIndex = 0;
			AbstractQuoteService cQuoteService = null;

			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(dataSource, out iIndex)) {
					cQuoteService = __cQuoteServices[iIndex];
				}
			}
			return cQuoteService;
		}

		/// <summary>
		///   取得即時報價服務
		/// </summary>
		/// <param name="quoteServiceInformation">報價服務資訊</param>
		/// <returns>返回值: AbstractQuoteService 報價服務抽象類別</returns>
		public AbstractQuoteService GetQuoteService(QuoteServiceInformation quoteServiceInformation) {
			AbstractQuoteService cQuoteService = null;
			string sDataSource = quoteServiceInformation.DataSource;
			if (sDataSource == null) {
				string sFilename = Path.GetFileName(quoteServiceInformation.Location);
				Assembly cAssembly = File.Exists(sFilename) ? Assembly.LoadFile(Path.GetFullPath(sFilename)) : Assembly.LoadFile(Path.GetFullPath(quoteServiceInformation.Location));
				Type cType = cAssembly.GetType(quoteServiceInformation.Name);

				cQuoteService = Activator.CreateInstance(cType) as AbstractQuoteService;
				cQuoteService.Load();   //讀取資訊
			} else {
				cQuoteService = GetQuoteService(sDataSource);
			}
			return cQuoteService;
		}

		/// <summary>
		///   取得所有報價元件資訊
		/// </summary>
		/// <returns>返回值:QuoteServiceInformation類別的陣列</returns>
		public QuoteServiceInformation[] GetQuoteServiceInformations() {
			QuoteServiceInformation[] cQuoteServiceInfos = null;

			lock (__cQuoteServiceInfos) {
				int iCount = __cQuoteServiceInfos.Count;
				cQuoteServiceInfos = new QuoteServiceInformation[iCount];
				__cQuoteServiceInfos.Values.CopyTo(cQuoteServiceInfos, 0);
			}
			return cQuoteServiceInfos;
		}

		/// <summary>
		///   更新報件元件資訊
		/// </summary>
		/// <param name="quoteDirectory">報價元件資料夾</param>
		public void Refresh(string quoteDirectory) {
			string[] sDllFiles = Directory.GetFiles(quoteDirectory, "*.dll");

			int iLength = sDllFiles.Length;
			for (int i = 0; i < iLength; i++) {
				string sDllFile = sDllFiles[i];
				string sAssembly = Path.GetFileNameWithoutExtension(sDllFile);
				FileVersionInfo cFileInfo = FileVersionInfo.GetVersionInfo(sDllFile);

				QuoteServiceInformation cQuoteInfo = null;
				lock (__cQuoteServiceInfos) {
					if (!__cQuoteServiceInfos.TryGetValue(sAssembly, out cQuoteInfo)) {
						cQuoteInfo = new QuoteServiceInformation();
						__cQuoteServiceInfos.Add(sAssembly, cQuoteInfo);
					}
				}

				//更新報價元件
				if (!cQuoteInfo.Enabled) { //沒有被啟動的才可以更新
					cQuoteInfo.Company = cFileInfo.CompanyName;
					cQuoteInfo.Description = cFileInfo.Comments;
					cQuoteInfo.FileVersion = cFileInfo.FileVersion;
					cQuoteInfo.Name = sAssembly;
					cQuoteInfo.Location = sDllFile;
					cQuoteInfo.ProductVersion = cFileInfo.ProductVersion;
				}
			}
		}

		/// <summary>
		///   從資料來源搜尋商品代號資訊
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <returns>返回值:DataSourceInformation類別列表</returns>
		public List<DataSourceInformation> SearchSymbolFromDataSource(string symbolId) {
			int iCount = __cQuoteServices.Count;
			List<DataSourceInformation> cDataSourceInfos = new List<DataSourceInformation>(iCount);

			for (int i = 0; i < iCount; i++) {
				AbstractQuoteService cQuoteService = __cQuoteServices[i];
				if (cQuoteService.Storage.IsSymbolExist(symbolId)) {
					AbstractExchange cExchange = ProductManager.Manager.GetExchange(cQuoteService.ExchangeName);
					if (cExchange != null) {
						Product cProduct = cExchange.GetProduct(symbolId);
						if (cProduct != null) {
							cDataSourceInfos.Add(new DataSourceInformation(cExchange.ShortName, cQuoteService, cProduct));
						}
					}
				}
			}
			return cDataSourceInfos;
		}

		/// <summary>
		///   啟動報價元件服務
		/// </summary>
		/// <param name="quoteServiceInformation">報價服務資訊</param>
		/// <param name="userId">使用者ID</param>
		/// <param name="password">使用者密碼</param>
		/// <returns>返回值:true=登入成功, false=登入失敗</returns>
		public bool StartQuoteService(QuoteServiceInformation quoteServiceInformation, string userId = null, string password = null) {
			bool bRet = quoteServiceInformation.Enabled;

			if (!bRet) {
				AbstractQuoteService cQuoteService = GetQuoteService(quoteServiceInformation);
				if (userId != null && password != null) {
					cQuoteService.UserId = userId;
					cQuoteService.Password = password;
				}

				bool bLogin = cQuoteService.Login();  //登入報價伺服器
				if (bLogin) {
					quoteServiceInformation.Enabled = bLogin;
					quoteServiceInformation.DataSource = cQuoteService.DataSource;

					AddQuoteService(cQuoteService);
					bRet = bLogin;
				}
			}
			return bRet;
		}

		/// <summary>
		///   停止報價元件服務
		/// </summary>
		/// <param name="quoteServiceInformation">報價服務資訊</param>
		public void StopQuoteService(QuoteServiceInformation quoteServiceInformation) {
			if (quoteServiceInformation.Enabled) {
				string sDataSource = quoteServiceInformation.DataSource;
				RemoveQuoteService(sDataSource);  //移除報價服務

				quoteServiceInformation.Enabled = false;
				quoteServiceInformation.DataSource = null;
			}
		}

		internal void AddQuoteService(AbstractQuoteService quoteService) {
			int iIndex = -1;
			bool bNotHave = false;
			string sDataSource = quoteService.DataSource;
			lock (__cIndexs) {
				if (!__cIndexs.TryGetValue(sDataSource, out iIndex)) {
					__cIndexs.Add(sDataSource, __cQuoteServices.Count);
					__cQuoteServices.Add(quoteService);
					bNotHave = true;
				}
			}

			if (bNotHave) {
				if (onQuoteServiceSwitchChanged != null) {
					onQuoteServiceSwitchChanged(this, new QuoteServiceSwitchChangedEvent(sDataSource, true));
				}
			}
		}
		
		internal void RemoveQuoteService(string dataSource) {
			int iIndex = -1;
			bool bHave = false;
			lock (__cIndexs) {
				if (__cIndexs.TryGetValue(dataSource, out iIndex)) {
					iIndex = __cIndexs[dataSource];
					
					AbstractQuoteService cCurrent = __cQuoteServices[iIndex];
					cCurrent.Dispose();  //釋放資源

					int iLast = __cQuoteServices.Count - 1;
					if (iLast > 0 && iLast > iIndex) {
						AbstractQuoteService cLast = __cQuoteServices[iLast];

						__cIndexs[cLast.DataSource] = iIndex;
						__cQuoteServices[iIndex] = cLast;
					}
					
					__cIndexs.Remove(dataSource);
					__cQuoteServices.RemoveAt(iLast);
					bHave = true;
				}
			}
			
			if (bHave) {
				if (onQuoteServiceSwitchChanged != null) {
					onQuoteServiceSwitchChanged(this, new QuoteServiceSwitchChangedEvent(dataSource, false));
				}
			}
		}
	}
}