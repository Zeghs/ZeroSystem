using System;
using System.Drawing;
using System.Windows.Forms;
using Zeghs.Chart;

namespace Zeghs.Actions {
	internal sealed class Move : IAction {
		private static string __sName = "Move";
		private static string __sComment = "Move bars";

		private int __iMouseX = 0;
		private int __iInitBarNumber = 0;

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
			if (cStatus.IsDrag && cStatus.Event == EInputDeviceEvent.MouseMove) {
				MouseEventArgs e = cStatus.GetCurrentMouseArgs();
				if (e.Button == MouseButtons.Left) {
					ZChart cChart = parameter.Chart;
					AxisX cAxisX = cChart.AxisX;
					double dUnit = cAxisX.AxisRectangle.Width / (double) cAxisX.BarCount;
					int iOffset = (int) ((__iMouseX - e.X) / dUnit);

					cAxisX.BarNumber = __iInitBarNumber + iOffset;
					if (cAxisX.Refresh) {
						cChart.Refresh();
					}
				}
			} else if (!cStatus.IsDrag && cStatus.Event == EInputDeviceEvent.MouseDown) {
				MouseEventArgs e = cStatus.GetCurrentMouseArgs();
				if (e.Button == MouseButtons.Left) {
					parameter.IsAction = true;
					parameter.Context.Cursor = Cursors.SizeWE;
					__iMouseX = cStatus.GetCurrentMouseArgs().X;
					__iInitBarNumber = parameter.Chart.AxisX.BarNumber;
				}
			} else if (cStatus.Event == EInputDeviceEvent.MouseUp) {
				parameter.IsAction = false;
				parameter.Context.Cursor = Cursors.Default;
			}
		}
	}
}