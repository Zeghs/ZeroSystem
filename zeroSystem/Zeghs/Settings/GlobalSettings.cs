using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Zeghs.Chart;

namespace Zeghs.Settings {
	/// <summary>
	///   全域設定類別
	/// </summary>
	public sealed class GlobalSettings {
		private static BaseSetting __cBase = null;
		private static PathSetting __cPaths = null;
		private static TestSetting __cTesting = null;
		private static ChartProfile __cChartProfile = null;

		/// <summary>
		///   [取得] 基礎設定值
		/// </summary>
		public static BaseSetting Base {
			get {
				return __cBase;
			}
		}

		/// <summary>
		///   [取得] 預設的圖表設定值
		/// </summary>
		public static ChartProfile ChartProfile {
			get {
				return __cChartProfile;
			}
		}
		
		/// <summary>
		///   [取得] 預設系統目錄路徑設定值
		/// </summary>
		public static PathSetting Paths {
			get {
				return __cPaths;
			}
		}

		/// <summary>
		///   [取得] 測試性質功能設定值
		/// </summary>
		public static TestSetting Testing {
			get {
				return __cTesting;
			}
		}

		/// <summary>
		///   讀取全域設定
		/// </summary>
		/// <param name="filename">設定檔名稱</param>
		public static void Load(string filename) {
			if (File.Exists(filename)) {
				string[] sDatas = File.ReadAllLines(filename, Encoding.UTF8);
				__cChartProfile = JsonConvert.DeserializeObject<ChartProfile>(sDatas[0]);
				__cPaths = JsonConvert.DeserializeObject<PathSetting>(sDatas[1]);
				__cBase = JsonConvert.DeserializeObject<BaseSetting>(sDatas[2]);
				__cTesting = JsonConvert.DeserializeObject<TestSetting>(sDatas[3]);
			}

			//加入延伸模組
			string[] sFiles = Directory.GetFiles("plugins\\extends", "*.dll");
			foreach (string sFile in sFiles) {
				Assembly.LoadFrom(sFile);
			}
		}

		/// <summary>
		///   儲存全域設定
		/// </summary>
		/// <param name="filename">設定檔名稱</param>
		public static void Save(string filename) {
			StringBuilder cBuilder = new StringBuilder(32 * 1024);
			cBuilder.AppendLine(JsonConvert.SerializeObject(__cChartProfile));
			cBuilder.AppendLine(JsonConvert.SerializeObject(__cPaths));
			cBuilder.AppendLine(JsonConvert.SerializeObject(__cBase));
			cBuilder.AppendLine(JsonConvert.SerializeObject(__cTesting));

			File.WriteAllText(filename, cBuilder.ToString(), Encoding.UTF8);
		}
	}
}