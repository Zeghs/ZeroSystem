using PowerLanguage;
using Zeghs.Data;

namespace PowerLanguage {
	/// <summary>
	///   DataLoader 使用非同步取得 Instrument 商品資料會使用此委派通知使用者
	/// </summary>
	/// <param name="result">資料讀取者回報類別</param>
	public delegate void LoadDataCallback(DataLoaderResult result);

	/// <summary>
	///   資料讀取者回報類別(使用非同步取得 Instrument 會使用此類別存放回報結果)
	/// </summary>
	public sealed class DataLoaderResult {
		private IQuote __cQuote = null;
		private IInstrument __cInstrument = null;
		private object __oParameters = null;

		/// <summary>
		///   [取得] IInstrument 商品資料
		/// </summary>
		public IInstrument Data {
			get {
				return __cInstrument;
			}
		}

		/// <summary>
		///   [取得] 商品資料的即時報價資訊介面
		/// </summary>
		public IQuote Quote {
			get {
				return __cQuote;
			}
		}

		/// <summary>
		///   [取得] 使用者自訂參數
		/// </summary>
		public object Parameters {
			get {
				return __oParameters;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="instrument">商品資訊介面</param>
		/// <param name="quote">Quote 資訊報價介面</param>
		/// <param name="parameters">其他參數</param>
		public DataLoaderResult(IInstrument instrument, IQuote quote, object parameters) {
			__cQuote = quote;
			__cInstrument = instrument;
			__oParameters = parameters;
		}
	}
}