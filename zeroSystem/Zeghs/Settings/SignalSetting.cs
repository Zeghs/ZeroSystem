using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Zeghs.Scripts;

namespace Zeghs.Settings {
	/// <summary>
	///   信號資料設定類別
	/// </summary>
	public sealed class SignalSetting : ScriptSetting {
		/// <summary>
		///   讀取信號腳本設定檔
		/// </summary>
		/// <param name="settingFile">設定檔名稱</param>
		/// <returns>返回值: SignalSetting 類別(null=無設定檔)</returns>
		public static SignalSetting LoadSetting(string settingFile) {
			SignalSetting cSignalSetting = null;
			if (File.Exists(settingFile)) {
				string sSettings = File.ReadAllText(settingFile, Encoding.UTF8);
				cSignalSetting = JsonConvert.DeserializeObject<SignalSetting>(sSettings);
			}
			return cSignalSetting;
		}

		/// <summary>
		///   [取得/設定] 屬性值
		/// </summary>
		public SignalProperty Property {
			get;
			set;
		}

		internal SignalSetting() {
			this.Property = new SignalProperty();
		}
	}
}