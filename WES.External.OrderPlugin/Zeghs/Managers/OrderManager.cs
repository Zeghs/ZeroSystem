using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Zeghs.Services;
using Zeghs.Informations;

namespace Zeghs.Managers {
	/// <summary>
	///   下單元件管理員
	/// </summary>
	public sealed class OrderManager {
		private static readonly OrderManager __current = new OrderManager();

		/// <summary>
		///   [取得] OrderManager類別
		/// </summary>
		public static OrderManager Manager {
			get {
				return __current;
			}
		}

		private Dictionary<string, OrderServiceInformation> __cOrderServiceInfos = null;

		private OrderManager() {
			__cOrderServiceInfos = new Dictionary<string, OrderServiceInformation>(128);
		}

		/// <summary>
		///   建立下單元件服務
		/// </summary>
		/// <param name="orderSource">下單來源名稱(format: 下單組件名稱;下單服務類別名稱)</param>
		/// <returns>返回值:AbstractOrderService類別</returns>
		public AbstractOrderService CreateOrderService(string orderSource) {
			AbstractOrderService cOrderService = null;

			if (orderSource != null) {
				string[] sOrderParams = orderSource.Split(';');  //格式:元件模組名稱;下單服務名稱

				OrderServiceInformation cOrderInfo = null;
				lock (__cOrderServiceInfos) {
					__cOrderServiceInfos.TryGetValue(sOrderParams[0], out cOrderInfo);
				}

				if (cOrderInfo != null) {
					Assembly cAssembly = Assembly.LoadFile(Path.GetFullPath(cOrderInfo.Location));
					Type cType = cAssembly.GetType(sOrderParams[1]);

					cOrderService = Activator.CreateInstance(cType) as AbstractOrderService;
					cOrderService.Load();   //讀取資訊
				}
			}
			return cOrderService;
		}

		/// <summary>
		///   取得所有下單元件資訊
		/// </summary>
		/// <returns>返回值:OrderServiceInformation類別的陣列</returns>
		public OrderServiceInformation[] GetOrderServiceInformations() {
			OrderServiceInformation[] cOrderServiceInfos = null;

			lock (__cOrderServiceInfos) {
				int iCount = __cOrderServiceInfos.Count;
				cOrderServiceInfos = new OrderServiceInformation[iCount];
				__cOrderServiceInfos.Values.CopyTo(cOrderServiceInfos, 0);
			}
			return cOrderServiceInfos;
		}

		/// <summary>
		///   更新下單元件資訊
		/// </summary>
		/// <param name="orderDirectory">下單元件資料夾</param>
		public void Refresh(string orderDirectory) {
			string[] sDllFiles = Directory.GetFiles(orderDirectory, "*.dll", SearchOption.AllDirectories);

			int iLength = sDllFiles.Length;
			if (iLength > 0) {
				for (int i = 0; i < iLength; i++) {
					string sDllFile = sDllFiles[i];
					string sAssembly = Path.GetFileNameWithoutExtension(sDllFile);
					FileVersionInfo cFileInfo = FileVersionInfo.GetVersionInfo(sDllFile);

					OrderServiceInformation cOrderInfo = null;
					lock (__cOrderServiceInfos) {
						if (!__cOrderServiceInfos.TryGetValue(sAssembly, out cOrderInfo)) {
							List<string> cServices = new List<string>();
							Assembly cAssembly = Assembly.LoadFile(Path.GetFullPath(sDllFile));
							
							Type[] cTypes = cAssembly.GetTypes();
							foreach (Type cType in cTypes) {
								if (CheckAbstractOrderService(cType)) {
									cServices.Add(cType.FullName);
								}
							}

							cOrderInfo = new OrderServiceInformation();
							cOrderInfo.Services = cServices.ToArray();
							
							__cOrderServiceInfos.Add(sAssembly, cOrderInfo);
						}
					}

					//更新下單資訊
					cOrderInfo.Company = cFileInfo.CompanyName;
					cOrderInfo.Description = cFileInfo.Comments;
					cOrderInfo.FileVersion = cFileInfo.FileVersion;
					cOrderInfo.ModuleName = sAssembly;
					cOrderInfo.Location = sDllFile;
					cOrderInfo.ProductVersion = cFileInfo.ProductVersion;
				}
			}
		}

		private bool CheckAbstractOrderService(Type baseType) {
			Type cBase = baseType.BaseType;
			if (cBase.FullName.Equals("System.Object")) {
				return false;
			} else if (cBase.FullName.Equals("Zeghs.Services.AbstractOrderService")) {
				return true;
			} else {
				return CheckAbstractOrderService(cBase);
			}

		}
	}
}