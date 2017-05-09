namespace Zeghs.Events {
	public sealed class ProgressEvent {
		private int __iTotals = 0;
		private int __iCurrent = 0;
		private string __sFilename = null;

		/// <summary>
		///   [取得]目前已經解壓縮的檔案個數
		/// </summary>
		public int Current {
			get {
				return __iCurrent;
			}
		}

		/// <summary>
		///   [取得]目前解壓縮的檔案名稱
		/// </summary>
		public string FileName {
			get {
				return __sFilename;
			}
		}

		/// <summary>
		///   [取得]壓縮檔內的所有檔案個數
		/// </summary>
		public int Totals {
			get {
				return __iTotals;
			}
		}

		public ProgressEvent(string filename, int current, int totals) {
			__sFilename = filename;
			__iCurrent = current;
			__iTotals = totals;
		}
	}
}