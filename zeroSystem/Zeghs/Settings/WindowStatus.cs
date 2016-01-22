using System.Windows.Forms;

namespace Zeghs.Settings {
	/// <summary>
	///   視窗狀態類別
	/// </summary>
	public sealed class WindowStatus {
		/// <summary>
		///   [取得/設定] 視窗高度
		/// </summary>
		public int Height {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 視窗右邊緣距離
		/// </summary>
		public int Left {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 視窗上邊緣距離
		/// </summary>
		public int Top {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 視窗寬度
		/// </summary>
		public int Width {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 視窗狀態
		/// </summary>
		public FormWindowState WindowState {
			get;
			set;
		}
	}
}