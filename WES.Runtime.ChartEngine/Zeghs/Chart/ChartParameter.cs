using System.Windows.Forms;
using Zeghs.Actions;

namespace Zeghs.Chart {
	/// <summary>
	///   圖表必要參數類別
	/// </summary>
	public sealed class ChartParameter {
		private ZChart __cChart = null;
		private Control __cContext = null;
		private Behavior __cBehavior = null;
		private InputDeviceStatus __cStatus = null;

		/// <summary>
		///   [取得] 使用者操作/動作行為類別
		/// </summary>
		public Behavior Behavior {
			get {
				return __cBehavior;
			}
		}

		/// <summary>
		///   [取得] Chart 圖表類別
		/// </summary>
		public ZChart Chart {
			get {
				return __cChart;
			}
		}

		/// <summary>
		///   [取得] 目標控制項類別(目標控制項可能為 Form, UserControl...)
		/// </summary>
		public Control Context {
			get {
				return __cContext;
			}
		}

		/// <summary>
		///   [取得] 使用者自訂繪製動作名稱(繪製名稱可能是線段, 趨勢線...)
		/// </summary>
		public string CustomPainter {
			get;
			set;
		}

		/// <summary>
		///   [取得] 使用者自訂筆刷(選擇繪製動作時, 可選擇欲繪製的筆刷樣式)
		/// </summary>
		public PowerLanguage.PenStyle CustomPen {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 是否使用者啟用了控制動作
		/// </summary>
		public bool IsAction {
			get;
			set;
		}

		/// <summary>
		///   [取得] 輸入設備狀態(滑鼠與鍵盤或其他輸入設備的狀態)
		/// </summary>
		public InputDeviceStatus Status {
			get {
				return __cStatus;
			}
		}

		/// <summary>
		///   [取得] 是否 Chart 之前曾刷新
		/// </summary>
		public bool Updated {
			get;
			internal set;
		}

		internal ChartParameter(ZChart chart, Control context, Behavior behavior, InputDeviceStatus status) {
			__cChart = chart;
			__cContext = context;
			__cBehavior = behavior;
			__cStatus = status;
		}
	}
}