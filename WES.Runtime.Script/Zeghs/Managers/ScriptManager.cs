using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using log4net;
using CSScriptLibrary;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Services;
using Zeghs.Informations;

namespace Zeghs.Managers {
	/// <summary>
	///   腳本管理員類別
	/// </summary>
	public sealed class ScriptManager {
		private static readonly ILog logger = LogManager.GetLogger(typeof(ScriptManager));
		private static readonly ScriptManager __current = new ScriptManager();

		private static ScriptSettings __cScriptSettings = null;

		/// <summary>
		///   [取得] 腳本設定資訊
		/// </summary>
		public static ScriptSettings Settings {
			get {
				return __cScriptSettings;
			}
		}

		/// <summary>
		///   [取得] ScriptManager類別
		/// </summary>
		public static ScriptManager Manager {
			get {
				return __current;
			}
		}

		/// <summary>
		///   讀取腳本設定值
		/// </summary>
		public static void LoadSettings() {
			string sFileName = "WES.Runtime.Script.set";
			if (File.Exists(sFileName)) {  //檔案存在就寫入
				string sSettings = File.ReadAllText(sFileName, Encoding.UTF8);
				__cScriptSettings = JsonConvert.DeserializeObject<ScriptSettings>(sSettings);
			}
		}

		/// <summary>
		///   儲存腳本設定值
		/// </summary>
		public static void SaveSettings() {
			string sLocation = Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			string sSettings = JsonConvert.SerializeObject(__cScriptSettings, Formatting.Indented);
			File.WriteAllText(sFileName, sSettings, Encoding.UTF8);
		}

		/// <summary>
		///   當讀取或新增腳本時會觸發此事件
		/// </summary>
		public event EventHandler<AddationScriptEvent> onAdditionScript = null;

		/// <summary>
		///   讀取所有腳本完成後會觸發此事件
		/// </summary>
		public event EventHandler onLoadScriptCompleted = null;
		
		private Dictionary<string, int> __cKeys = null;
		private List<ScriptInformation> __cScripts = null;

		/// <summary>
		///   [取得] 所有腳本資訊
		/// </summary>
		public List<ScriptInformation> Scripts {
			get {
				return __cScripts;
			}
		}

		private ScriptManager() {
			__cKeys = new Dictionary<string, int>(256);
			__cScripts = new List<ScriptInformation>(256);
		}

		/// <summary>
		///   編譯後更新腳本
		/// </summary>
		/// <param name="scriptFile">腳本檔案名稱</param>
		/// <returns>返回值:true=成功, false=失敗</returns>
		public bool ComplierAndUpdate(string scriptFile) {
			string sComplierFile = this.Complier(scriptFile);
			if (sComplierFile != null) {
				this.AddScript(sComplierFile);
				return true;
			}
			return false;
		}

		/// <summary>
		///   建立新的腳本執行個體
		/// </summary>
		/// <param name="scriptName">完整腳本名稱</param>
		/// <returns>返回值: CStudyAbstract 類別, null=無此腳本</returns>
		public CStudyAbstract CreateScript(string scriptName) {
			int iIndex = 0;
			CStudyAbstract cScript = null;

			if (__cKeys.TryGetValue(scriptName, out iIndex)) {
				ScriptInformation cScriptInfo = __cScripts[iIndex];
				cScript = cScriptInfo.CreateScript();
				cScript.About = cScriptInfo.Property;
			}
			return cScript;
		}

		/// <summary>
		///   讀取所有腳本
		/// </summary>
		public void LoadScripts() {
			if (__cScriptSettings != null) {
				Dictionary<string, string> cOutputs = new Dictionary<string, string>(256);

				//讀取輸出目錄內的所有編譯後的腳本最後存取時間並保留
				string[] sOutputs = Directory.GetFiles(__cScriptSettings.OutputPath, "*.dll");
				foreach (string sOutput in sOutputs) {
					string sFilename = Path.GetFileNameWithoutExtension(sOutput);
					if (!cOutputs.ContainsKey(sFilename)) {
						cOutputs.Add(sFilename, sOutput);
					}
				}

				//讀取來源目錄內的所有.cs腳本
				string[] sInputs = Directory.GetFiles(__cScriptSettings.SourcePath, "*.cs", SearchOption.AllDirectories);
				foreach (string sInput in sInputs) {
					bool bExist = false, bComplier = true;
					string sOutput = null, sFilename = Path.GetFileNameWithoutExtension(sInput);
					if (cOutputs.TryGetValue(sFilename, out sOutput)) {
						bExist = true;

						FileInfo cIFileInfo = new FileInfo(sInput);
						FileInfo cOFileInfo = new FileInfo(sOutput);
						DateTime cInputTime = cIFileInfo.LastAccessTime;
						DateTime cOutputTime = cOFileInfo.LastAccessTime;
						
						if (cInputTime < cOutputTime) {
							bComplier = false;
						}
					} else {
						string sText = File.ReadAllText(sInput, Encoding.UTF8);
						bool bHaveScript = (sText.IndexOf("[ScriptProperty(") > -1);
						if (!bHaveScript) {
							continue;  //如果不是腳本格式就跳過
						}
					}

					if (bComplier) {
						string sComplierFile = this.Complier(sInput);
						if (sComplierFile == null) {
							if (bExist) {
								if (File.Exists(sOutput)) {
									try {
										File.Delete(sOutput);
									} catch(Exception __errExcep) {
										if (logger.IsErrorEnabled) logger.ErrorFormat("[ScriptManager.LoadScripts] Error Message:{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
									}
								}
								cOutputs.Remove(sFilename);
							}
						} else {
							if (!bExist) {
								cOutputs.Add(sFilename, sComplierFile);
							}
						}
					}
				}

				if (cOutputs.Count > 0) {
					foreach (string sOutput in cOutputs.Values) {
						this.AddScript(sOutput);  //載入腳本
					}

					if (onLoadScriptCompleted != null) {
						onLoadScriptCompleted(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		///   加入腳本
		/// </summary>
		/// <param name="assemblyFile">編譯後的腳本檔案名稱</param>
		public void AddScript(string assemblyFile) {
			string sAssembly = Path.GetFileNameWithoutExtension(assemblyFile);
			Assembly cAssembly = AssemblyResolver.ResolveAssembly(sAssembly, Path.GetDirectoryName(assemblyFile));
			AddScript(cAssembly);
		}

		/// <summary>
		///   加入腳本
		/// </summary>
		/// <param name="assembly">Assembly 組件資訊</param>
		public void AddScript(Assembly assembly) {
			Type[] cTypes = assembly.GetTypes();
			foreach (Type cType in cTypes) {
				ScriptPropertyAttribute[] cPropertys = cType.GetCustomAttributes(typeof(ScriptPropertyAttribute), false) as ScriptPropertyAttribute[];
				foreach (ScriptPropertyAttribute cProperty in cPropertys) {
					int iIndex = 0;
					string sFullAssembly = cType.FullName;
					ScriptInformation cScriptInfo = new ScriptInformation(cType, cProperty);

					lock (__cKeys) {
						if (__cKeys.TryGetValue(sFullAssembly, out iIndex)) {
							__cScripts[iIndex] = cScriptInfo;
						} else {
							iIndex = __cScripts.Count;
							__cScripts.Add(cScriptInfo);
							__cKeys.Add(sFullAssembly, iIndex);
						}
					}

					if (onAdditionScript != null) {
						onAdditionScript(this, new AddationScriptEvent(assembly, cScriptInfo));
					}
				}
			}
		}

		/// <summary>
		///   編譯腳本
		/// </summary>
		/// <param name="scriptFile">腳本檔案名稱</param>
		/// <returns>返回值:編譯後的腳本檔案名稱, null=編譯失敗</returns>
		internal string Complier(string scriptFile) {
			string sComplierFile = null;
			
			if (__cScriptSettings != null) {
				string sPath = __cScriptSettings.OutputPath;
				string sAssembly = Path.GetFileNameWithoutExtension(scriptFile);
				string sFileName = Path.Combine(sPath, sAssembly + ".dll");

				try {
					sComplierFile = CSScript.CompileFile(scriptFile, sFileName, false);
				} catch (Exception __errExcep) {
					if (logger.IsErrorEnabled) logger.ErrorFormat("[ScriptManager.Complier] Error Message:{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
				}
			}
			return sComplierFile;
		}
	}
}