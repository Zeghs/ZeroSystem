namespace PowerLanguage {
	/// <summary>
	///   內建變數存放類別(存放目前的變數值與前一筆的變數值所設計的類別, 減少使用 VariableSeries 所消耗的記憶體)
	/// </summary>
	/// <typeparam name="T">變數類型</typeparam>
	internal struct _Variable<T> {
		private T __current;
		private T __previous;
		private int __iPreviousBar;
		private IInstrument __cBars;

		/// <summary>
		///   [取得] 前一筆的變數值(以 CurrentBar - 1 為前一筆的依據)
		/// </summary>
		internal T Previous {
			get {
				int iCurrentBar = __cBars.CurrentBar;
				if (iCurrentBar > __iPreviousBar) {
					__previous = __current;
					__iPreviousBar = iCurrentBar;
				}
				return __previous;
			}
		}

		/// <summary>
		///   [取得/設定] 目前最新的變數值(以 CurrentBar 為最新依據)
		/// </summary>
		internal T Value {
			get {
				return __current;
			}

			set {
				__current = value;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">IStudyControl 介面</param>
		/// <param name="defaultVal">預設值</param>
		/// <param name="dataStream">資料串流編號(預設值=1)</param>
		internal _Variable(IStudyControl master, T defaultVal, int dataStream = 1) {
			//目前的 CurrentBar 需要超過兩個 Bars 之後才開始保存前一筆的資訊(前兩個 Bars 資訊可能會因為往前請求而導致 CurrentBar 不準確, 所以都跳過兩個 Bars 後在保存前一筆的資訊)
			__iPreviousBar = 2;
			__current = defaultVal;
			__previous = defaultVal;
			__cBars = master.BarsOfData(dataStream);
		}
	}
}