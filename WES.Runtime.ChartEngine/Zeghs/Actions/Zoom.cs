using System.Drawing;
using System.Windows.Forms;
using Zeghs.Chart;

namespace Zeghs.Actions {
	internal sealed class Zoom : IAction {
		private const int ZOOM_SCALE_UNIT = 20;  //滑鼠滾輪捲動縮放刻度單位
		private static string __sName = "Zoom";
		private static string __sComment = "Zoom bars";

		public string Comment {
			get {
				return __sComment;
			}
		}

		public Image[] Icons {
			get {
				return null;
			}
		}

		public string Name {
			get {
				return __sName;
			}
		}

		public void Action(ChartParameter parameter) {
			InputDeviceStatus cStatus = parameter.Status;
			if (cStatus.Event == EInputDeviceEvent.MouseWheel) {
				MouseEventArgs e = cStatus.GetCurrentMouseArgs();
				ZChart cChart = parameter.Chart;
				AxisX cAxisX = cChart.AxisX;

				int iCount = cAxisX.BarCount;
				if (e.Delta < 0) {
					iCount += ZOOM_SCALE_UNIT;
				} else {
					iCount -= ZOOM_SCALE_UNIT;
				}

				cAxisX.BarCount = iCount;
				if (cAxisX.Refresh) {
					cChart.Refresh();
				}
			}
		}
	}
}