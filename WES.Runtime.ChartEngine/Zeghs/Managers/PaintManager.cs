using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using log4net;
using Zeghs.Chart;
using Zeghs.Drawing;
using Zeghs.Informations;

namespace Zeghs.Managers {
	/// <summary>
	///   繪製管理者類別(用來管理圖表引擎模組與建立繪製引擎抽象類別)
	/// </summary>
	public sealed class PaintManager {
		private static readonly ILog logger = LogManager.GetLogger(typeof(PaintManager));
		private static readonly PaintManager __current = new PaintManager();

		/// <summary>
		///   [取得] PaintManager 類別
		/// </summary>
		public static PaintManager Manager {
			get {
				return __current;
			}
		}

		private Dictionary<string, ChartEngineInformation> __cChartEngineInfos = null;

		private PaintManager() {
			__cChartEngineInfos = new Dictionary<string, ChartEngineInformation>(16);
		}

		/// <summary>
		///   建立 Painter 繪製引擎抽象類別
		/// </summary>
		/// <param name="handle">控制項 handle</param>
		/// <param name="engineInformation">圖表引擎資訊類別</param>
		/// <returns>返回值: AbstractPaintEngine 繪製引擎抽象類別</returns>
		public AbstractPaintEngine CreatePainter(IntPtr handle, ChartEngineInformation engineInformation) {
			AbstractPaintEngine cPaintEngine = null;

			if (engineInformation != null) {
				Assembly cAssembly = Assembly.LoadFile(Path.GetFullPath(engineInformation.Location));
				Type cType = cAssembly.GetType(engineInformation.ChartEngine);

				cPaintEngine = Activator.CreateInstance(cType, handle) as AbstractPaintEngine;
			}
			return cPaintEngine;
		}

		/// <summary>
		///   取得所有圖表引擎資訊
		/// </summary>
		/// <returns>返回值:ChartEngineInformation類別的陣列</returns>
		public ChartEngineInformation[] GetChartEngineInformations() {
			ChartEngineInformation[] cChartEngineInfos = null;

			lock (__cChartEngineInfos) {
				int iCount = __cChartEngineInfos.Count;
				cChartEngineInfos = new ChartEngineInformation[iCount];
				__cChartEngineInfos.Values.CopyTo(cChartEngineInfos, 0);
			}
			return cChartEngineInfos;
		}

		/// <summary>
		///   更新圖表引擎資訊
		/// </summary>
		/// <param name="chartDirectory">圖表引擎資料夾</param>
		public void Refresh(string chartDirectory) {
			string[] sDllFiles = Directory.GetFiles(chartDirectory, "*.dll");

			int iLength = sDllFiles.Length;
			if (iLength > 0) {
				for (int i = 0; i < iLength; i++) {
					string sDllFile = sDllFiles[i];
					string sAssembly = Path.GetFileNameWithoutExtension(sDllFile);
					FileVersionInfo cFileInfo = FileVersionInfo.GetVersionInfo(sDllFile);

					ChartEngineInformation cChartEngineInfo = null;
					lock (__cChartEngineInfos) {
						if (!__cChartEngineInfos.TryGetValue(sAssembly, out cChartEngineInfo)) {
							string sChartEngine = null;
							Assembly cAssembly = Assembly.LoadFile(Path.GetFullPath(sDllFile));

							Type[] cTypes = cAssembly.GetTypes();
							foreach (Type cType in cTypes) {
								if (CheckAbstractOrderService(cType)) {
									sChartEngine = cType.FullName;
									break;
								}
							}

							cChartEngineInfo = new ChartEngineInformation();
							cChartEngineInfo.ChartEngine = sChartEngine;

							__cChartEngineInfos.Add(sAssembly, cChartEngineInfo);
						}
					}

					//更新圖表引擎資訊
					cChartEngineInfo.Company = cFileInfo.CompanyName;
					cChartEngineInfo.Description = cFileInfo.Comments;
					cChartEngineInfo.FileVersion = cFileInfo.FileVersion;
					cChartEngineInfo.ModuleName = sAssembly;
					cChartEngineInfo.Location = sDllFile;
					cChartEngineInfo.ProductVersion = cFileInfo.ProductVersion;
				}
			}
		}

		private bool CheckAbstractOrderService(Type baseType) {
			Type cBase = baseType.BaseType;
			if (cBase.FullName.Equals("System.Object")) {
				return false;
			} else if (cBase.FullName.Equals("Zeghs.Chart.AbstractPaintEngine")) {
				return true;
			} else {
				return CheckAbstractOrderService(cBase);
			}

		}
	}
}