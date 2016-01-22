namespace PowerLanguage {
	/// <summary>
	///   序列資訊延伸介面
	/// </summary>
	public interface ISeriesSymbolDataRand : ISeriesSymbolData {
		/// <summary>
		///   [取得] 目前索引值位置
		/// </summary>
		int Current {
			get;
		}

		/// <summary>
		///   [取得] 序列資料總個數
		/// </summary>
		int Count {
			get;
		}
	}
}