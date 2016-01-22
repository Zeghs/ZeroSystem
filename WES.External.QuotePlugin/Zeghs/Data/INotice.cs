namespace Zeghs.Data {
	/// <summary>
	///   即時公告資訊介面
	/// </summary>
	public interface INotice {
		/// <summary>
		///   [取得] 公告內容
		/// </summary>
		string Content {
			get;
		}

		/// <summary>
		///   [取得] 公告等級(備用)
		/// </summary>
		int Level {
			get;
		}

		/// <summary>
		///   [取得] 公告發出的時間
		/// </summary>
		string Time {
			get;
		}

		/// <summary>
		///   [取得] 公告標題
		/// </summary>
		string Title {
			get;
		}
	}
}