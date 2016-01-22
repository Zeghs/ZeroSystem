using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Products;
using Zeghs.Informations;

namespace Zeghs.Managers {
	/// <summary>
	///   商品管理員類別
	/// </summary>
	public sealed class ProductManager {
		private static readonly ProductManager __current = new ProductManager();

		/// <summary>
		///   [取得] ProductManager類別
		/// </summary>
		public static ProductManager Manager {
			get {
				return __current;
			}
		}

		/// <summary>
		///   讀取指定資料夾內的所有交易所模組
		/// </summary>
		/// <param name="exchangeDirectory">資料夾名稱</param>
		public static void Load(string exchangeDirectory) {
			string[] sDllFiles = Directory.GetFiles(exchangeDirectory, "*.dll");
			
			int iLength = sDllFiles.Length;
			for (int i = 0; i < iLength; i++) {
				string sDllFile = sDllFiles[i];
				string sPath = Path.GetDirectoryName(sDllFile);
				string sAssembly = Path.GetFileNameWithoutExtension(sDllFile);

				Assembly cAssembly = Assembly.LoadFile(Path.GetFullPath(sDllFile));
				Type cType = cAssembly.GetType(sAssembly);

				AbstractExchange cExchange = Activator.CreateInstance(cType) as AbstractExchange;
				cExchange.Load();       //讀取資訊
				cExchange.Initialize(); //初始化交易所

				__current.AddExchange(cExchange);
			}
		}

		private Dictionary<string, int> __cKeys = null;
		private List<AbstractExchange> __cExchanges = null;

		/// <summary>
		///   [取得] 所有交易所資訊
		/// </summary>
		public List<AbstractExchange> Exchanges {
			get {
				return __cExchanges;
			}
		}

		private ProductManager() {
			__cKeys = new Dictionary<string, int>(128);
			__cExchanges = new List<AbstractExchange>(128);
		}

		/// <summary>
		///   取得交易所
		/// </summary>
		/// <param name="shortName">交易所簡稱</param>
		/// <returns>返回值:AbstractExchange類別</returns>
		public AbstractExchange GetExchange(string shortName) {
			int iIndex = 0;
			AbstractExchange cExchange = null;
			lock (__cKeys) {
				if (__cKeys.TryGetValue(shortName, out iIndex)) {
					cExchange = __cExchanges[iIndex];
				}
			}
			return cExchange;
		}

		/// <summary>
		///   搜尋商品資訊
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <param name="isMatch">是否需要吻合(true=商品代號需要完全吻合, false=商品代號不需要完全吻合)</param>
		/// <returns>返回值:商品資訊列表</returns>
		public List<ProductInformation> SearchProducts(string symbolId, bool isMatch) {
			List<ProductInformation> cProductInfos = new List<ProductInformation>(64);

			int iCount = __cExchanges.Count;
			for (int i = 0; i < iCount; i++) {
				AbstractExchange cExchange = __cExchanges[i];
				List<Product> cProducts = cExchange.SearchProducts(symbolId, isMatch);
				if (cProducts.Count > 0) {
					cProductInfos.Add(new ProductInformation(cExchange, cProducts));
				}
			}
			return cProductInfos;
		}

		internal void AddExchange(AbstractExchange exchange) {
			string sShortName = exchange.ShortName;
			lock (__cKeys) {
				if (!__cKeys.ContainsKey(sShortName)) {
					int iIndex = __cExchanges.Count;
					__cExchanges.Add(exchange);
					__cKeys.Add(sShortName, iIndex);
				}
			}
		}
	}
}