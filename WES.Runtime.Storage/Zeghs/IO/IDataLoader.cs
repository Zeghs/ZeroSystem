using PowerLanguage;

namespace Zeghs.IO {
	public interface IDataLoader {
		/// <summary>
		///   非同步讀取資料請求結構內的 IInstrument 資料
		/// </summary>
		/// <param name="request">資料請求結構</param>
		/// <param name="result">當成功取得 IInstrument 商品資料會使用此委派方法回傳資料</param>
		/// <param name="args">使用者自訂參數</param>
		void BeginLoadData(InstrumentDataRequest request, LoadDataCallback result, object args = null);
	}
}