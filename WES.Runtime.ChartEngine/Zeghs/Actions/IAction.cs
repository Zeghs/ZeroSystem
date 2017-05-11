using System.Drawing;
using Zeghs.Chart;

namespace Zeghs.Actions {
	/// <summary>
	///   使用者操作/動作介面
	/// </summary>
	public interface IAction {
		/// <summary>
		///   [取得] 動作註解/說明
		/// </summary>
		string Comment {
			get;
		}

		/// <summary>
		///   [取得] 動作圖示(0=16*16, 其他可以由設計者自訂)
		/// </summary>
		Image[] Icons {
			get;
		}

		/// <summary>
		///   [取得] 動作名稱
		/// </summary>
		string Name {
			get;
		}

		/// <summary>
		///   執行使用者動作
		/// </summary>
		/// <param name="parameter">圖表必要參數類別</param>
		void Action(ChartParameter parameter);
	}
}