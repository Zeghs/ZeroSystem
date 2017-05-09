using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Zeghs.Chart;
using Zeghs.Scripts;

namespace Zeghs.Settings {
	/// <summary>
	///   Profile 設定檔類別
	/// </summary>
	public sealed class ProfileSetting {
		internal static ProfileSetting CreateProfile(JToken setting) {
			ProfileSetting cProfile = new ProfileSetting();
			int iScriptType = setting["ScriptType"].Value<int>();
			switch (iScriptType) {
				case 0:  //Signal
					cProfile.Script = setting["Script"].ToObject<SignalSetting>();
					break;
				case 1:  //Script:
					break;
			}

			cProfile.ChartProperty = setting["ChartProperty"].ToObject<ChartProperty>();
			cProfile.ProfileId = setting["ProfileId"].Value<string>();
			cProfile.Parameters = setting["Parameters"].ToObject<List<string>>();
			cProfile.ScriptName = setting["ScriptName"].Value<string>();
			cProfile.ScriptType = (ScriptType) Enum.Parse(typeof(ScriptType), iScriptType.ToString());
			cProfile.Window = setting["Window"].ToObject<WindowStatus>();

			return cProfile;
		}

		/// <summary>
		///   [取得/設定] 圖表設定值列表
		/// </summary>
		public ChartProperty ChartProperty {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] Profile ID編號
		/// </summary>
		public string ProfileId {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 參數設定值資訊
		/// </summary>
		public List<string> Parameters {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本設定
		/// </summary>
		public ScriptSetting Script {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本名稱
		/// </summary>
		public string ScriptName {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本類型
		/// </summary>
		public ScriptType ScriptType {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 視窗狀態
		/// </summary>
		public WindowStatus Window {
			get;
			set;
		}
	}
}