using System.Collections.Generic;

namespace Zeghs.Settings {
	/// <summary>
	///   基礎腳本設定
	/// </summary>
	public class ScriptSetting {
		/// <summary>
		///   [取得/設定] 資料請求資訊陣列
		/// </summary>
		public List<RequestSetting> DataRequests {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		public ScriptSetting() {
			this.DataRequests = new List<RequestSetting>(32);
		}
	}
}