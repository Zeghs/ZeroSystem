using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Zeghs.Chart;

namespace Zeghs.Actions {
	internal sealed class Resize : IAction {
		private static string __sName = "Resize";
		private static string __sComment = "Resize";

		private bool __bResize = false;
		private double __dLayerHeight = 0;
		private int __iMouseY = 0, __iLayerIndex = 0;

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
			if (cStatus.Event == EInputDeviceEvent.MouseMove) {
				MouseEventArgs e = cStatus.GetCurrentMouseArgs();
				if (cStatus.IsDrag && e.Button == MouseButtons.Left) {
					int iOffset = e.Y - __iMouseY;
					ZChart cChart = parameter.Chart;
					List<Layer> cLayers = cChart.Layers;
					Layer cLayer = cLayers[__iLayerIndex];

					double dScale = (__dLayerHeight + iOffset) / cChart.ChartRectangle.Height;
					cLayers[__iLayerIndex + 1].HeightScale += cLayer.HeightScale - dScale;
					cLayer.HeightScale = dScale;

					cChart.Resize();
					cChart.Refresh();
				} else {
					ZChart cChart = parameter.Chart;
					List<Layer> cLayers = cChart.Layers;
					int iCount = cLayers.Count - 1;
					for (int i = 0; i < iCount; i++) {
						Layer cLayer = cLayers[i];
						if (cLayer.IsLayerScope(e.X, e.Y)) {
							Rectangle cRect = cLayer.LayerRectangle;
							int iMaxBottom = cRect.Y + cRect.Height;
							int iMinBottom = iMaxBottom - 5;
							if (e.Y >= iMinBottom && e.Y <= iMaxBottom) {
								__bResize = true;
								__iLayerIndex = i;
								__dLayerHeight = cRect.Height;
								parameter.Context.Cursor = Cursors.SizeNS;
								return;
							}
						}
					}

					__bResize = false;
					parameter.Context.Cursor = Cursors.Default;
				}
			} else if (__bResize && !cStatus.IsDrag && cStatus.Event == EInputDeviceEvent.MouseDown) {
				parameter.IsAction = true;
				__iMouseY = cStatus.GetCurrentMouseArgs().Y;
			} else if (cStatus.Event == EInputDeviceEvent.MouseUp) {
				__bResize = false;
				parameter.IsAction = false;
				parameter.Context.Cursor = Cursors.Default;
			}
		}
	}
}