using PowerLanguage;

namespace Zeghs.IO {
	/// <summary>
	///   資料讀取者介面
	/// </summary>
	public interface IDataLoader {
		/// <summary>
		///   非同步讀取資料請求結構內的 IInstrument 資料
		/// </summary>
		/// <param name="request">資料請求結構</param>
		/// <param name="result">當成功取得 IInstrument 商品資料會使用此委派方法回傳資料</param>
		/// <param name="useCache">是否使用快取 [預設:false](true=序列資料結構建立後保存在快取內，下次需要使用直接從快取拿取, false=重新建立序列資料結構，建立的序列資料需要自行移除否則會占用記憶體空間)</param>
		/// <param name="args">使用者自訂參數</param>
		/// <param name="millisecondsTimeout">回補資料 Timeout 毫秒數 [預設:System.Threading.Timeout.Infinite (永遠等待直到回補完成)]</param>
		void BeginLoadData(InstrumentDataRequest request, LoadDataCallback result, bool useCache = false, object args = null, int millisecondsTimeout = System.Threading.Timeout.Infinite);
	}
}