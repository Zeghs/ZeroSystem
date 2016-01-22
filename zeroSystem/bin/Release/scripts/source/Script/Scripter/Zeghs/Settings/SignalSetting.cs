using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Zeghs.Scripts;
using Zeghs.Managers;

namespace Zeghs.Settings {
	/// <summary>
	///   信號資料設定類別
	/// </summary>
	public sealed class SignalSetting {
		/// <summary>
		///   讀取信號腳本設定檔
		/// </summary>
		/// <param name="signal">信號類別</param>
		/// <returns>返回值: SignalSetting 類別(null=無設定檔)</returns>
		public static SignalSetting LoadSetting(object signal) {
			SignalSetting cSignalSetting = null;
			Type cType = signal.GetType();
			string sFileName = cType.Name + ".Strategy.set";
			string sPathAndFile = string.Format("{0}\\{1}", Path.GetDirectoryName(cType.Assembly.Location), sFileName);
			if (File.Exists(sPathAndFile)) {
				string sSettings = File.ReadAllText(sPathAndFile, Encoding.UTF8);
				cSignalSetting = JsonConvert.DeserializeObject<SignalSetting>(sSettings);

				//複製設定檔至輸出目錄
				File.Copy(sFileName, ScriptManager.Settings.OutputPath + sFileName, true);
			}
			return cSignalSetting;
		}

		/// <summary>
		///   [取得/設定] 資料請求資訊陣列
		/// </summary>
		public RequestSetting[] DataRequests {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 屬性值
		/// </summary>
		public SignalProperty Property {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 通道編號
		/// </summary>
		public int PipedNumber {
			get;
			set;
		}
	}
}