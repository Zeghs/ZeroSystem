using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerLanguage;

namespace Zeghs.Settings {
	internal sealed class ProfileManager {
		private static readonly ProfileManager __current = new ProfileManager();

		/// <summary>
		///   [取得] ProfileManager 類別
		/// </summary>
		internal static ProfileManager Manager {
			get {
				return __current;
			}
		}

		internal event EventHandler onLoadProfile = null;

		private WindowStatus __cMainWindowStatus = null;
		private Dictionary<string, ProfileSetting> __cProfiles = null;

		internal WindowStatus MainWindow {
			get {
				return __cMainWindowStatus;
			}
		}

		internal ProfileSetting[] Profiles {
			get {
				int iCount = __cProfiles.Count;
				if (iCount == 0) {
					return null;
				} else {
					ProfileSetting[] cProfiles = new ProfileSetting[__cProfiles.Count];
					__cProfiles.Values.CopyTo(cProfiles, 0);
					return cProfiles;
				}
			}
		}

		private ProfileManager() {
			__cProfiles = new Dictionary<string, ProfileSetting>(32);

			//讀取主視窗設定
			string sFile = string.Format("{0}__main.profile", GlobalSettings.Paths.ProfilePath);
			if (File.Exists(sFile)) {
				string sJSONSettings = File.ReadAllText(sFile, Encoding.UTF8);
				__cMainWindowStatus = JsonConvert.DeserializeObject<WindowStatus>(sJSONSettings);
			} else {
				__cMainWindowStatus = new WindowStatus();
			}
		}

		internal void AddProfile(ProfileSetting profile) {
			string sProfileId = profile.ProfileId;
			lock (__cProfiles) {
				if (!__cProfiles.ContainsKey(sProfileId)) {
					__cProfiles.Add(sProfileId, profile);
				}
			}
		}

		internal void Export(string file) {
			string sJSONSettings = JsonConvert.SerializeObject(__cProfiles, Formatting.Indented);
			File.WriteAllText(file, sJSONSettings, Encoding.UTF8);
		}

		internal ProfileSetting[] Import(string file) {
			if (File.Exists(file)) {
				string sJSONSettings = File.ReadAllText(file, Encoding.UTF8);
				Dictionary<string, JToken> cSettings = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(sJSONSettings);
				
				int iCount = cSettings.Count;
				if (iCount > 0) {
					int iIndex = 0;
					ProfileSetting[] cProfiles = new ProfileSetting[iCount];
					foreach (JToken cSetting in cSettings.Values) {
						ProfileSetting cProfile = ProfileSetting.CreateProfile(cSetting);
						cProfiles[iIndex++] = cProfile;
						AddProfile(cProfile);
					}
					return cProfiles;
				}
			}
			return null;
		}

		internal void Load(string name) {
			string sFile = string.Format("{0}{1}.profile", GlobalSettings.Paths.ProfilePath, name);
			if (File.Exists(sFile)) {
				lock (__cProfiles) {
					__cProfiles.Clear();
				}

				string sJSONSettings = File.ReadAllText(sFile, Encoding.UTF8);
				Dictionary<string, JToken> cSettings = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(sJSONSettings);
				foreach (JToken cSetting in cSettings.Values) {
					AddProfile(ProfileSetting.CreateProfile(cSetting));
				}

				if (onLoadProfile != null) {
					onLoadProfile(this, EventArgs.Empty);
				}
			}
		}

		internal void RemoveProfile(string profileId) {
			lock (__cProfiles) {
				if (__cProfiles.ContainsKey(profileId)) {
					__cProfiles.Remove(profileId);
				}
			}
		}

		internal void Save(string name) {
			//儲存策略視窗設定
			string sFile = string.Format("{0}{1}.profile", GlobalSettings.Paths.ProfilePath, name);
			string sJSONSettings = JsonConvert.SerializeObject(__cProfiles, Formatting.Indented);
			File.WriteAllText(sFile, sJSONSettings, Encoding.UTF8);

			//儲存主視窗設定
			string sMainFile = string.Format("{0}__main.profile", GlobalSettings.Paths.ProfilePath);
			string sJSONMainWindow = JsonConvert.SerializeObject(__cMainWindowStatus, Formatting.Indented);
			File.WriteAllText(sMainFile, sJSONMainWindow, Encoding.UTF8);
		}
	}
}