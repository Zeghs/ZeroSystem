using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Zeghs.Chart;
using Zeghs.Actions;

namespace Zeghs.Drawing.Actions {
	internal sealed class Line : IAction, IDrawable {
		private static string __sName = "Line";
		private static string __sComment = "Line";
		private static Image[] __cImages = new Image[] { Zeghs.Properties.Resources.Line };

		private Gdi __cGDI = null;
		private GdiEngine __cEngine = null;
		private Layer __cLayer = null;
		private List<_LineInfo> __cLineInfos = null;
		private Point __cStart = Point.Empty;

		public string Comment {
			get {
				return __sComment;
			}
		}

		public Image[] Icons {
			get {
				return __cImages;
			}
		}

		public string Name {
			get {
				return __sName;
			}
		}

		internal Line(GdiEngine engine) {
			__cGDI = engine.GDI;
			__cEngine = engine;

			__cLineInfos = new List<_LineInfo>(2);
		}

		public void Action(ChartParameter parameter) {
			ZChart cChart = parameter.Chart;
			InputDeviceStatus cStatus = parameter.Status;
			MouseEventArgs e = cStatus.GetCurrentMouseArgs();

			if (cStatus.Event == EInputDeviceEvent.MouseUp) {
				if (__cStart == Point.Empty) {
					List<Layer> cLayers = cChart.Layers;
					int iCount = cLayers.Count;
					for (int i = 0; i < iCount; i++) {
						Layer cLayer = cLayers[i];
						if (cLayer.IsLayerScope(e.X, e.Y)) {
							__cLayer = cLayer;
							break;
						}
					}

					__cStart = e.Location;
				} else {
					Point cEnd = e.Location;
					Boundary.BoundFix(ref __cStart, ref cEnd, new Rectangle4(__cLayer.LayerRectangleWithoutAxisY));

					//保存至使用者繪製容器內
					parameter.Behavior.DrawContainer.AddDrawObject(__sName, __cLayer, new Point[] { __cStart, cEnd }, new PowerLanguage.PenStyle[] { parameter.CustomPen });

					__cStart = Point.Empty;
					parameter.CustomPainter = null;  //繪圖完畢需要設定為 null
					
					__cGDI.ClearRops(__cLineInfos, null, false);
					cChart.Refresh();
				}
			} else if (__cStart != Point.Empty && cStatus.Event == EInputDeviceEvent.MouseMove) {
				__cGDI.SaveDC();
				__cGDI.ClipRectangle(__cLayer.LayerRectangleWithoutAxisY);
				__cGDI.ClearRops(__cLineInfos, null, !parameter.Updated);

				ChartProperty cProperty = cChart.ChartProperty;
				IntPtr cPen = Gdi.CreatePen(new PowerLanguage.PenStyle(cProperty.DrawAideLineColor, 1));
				__cLineInfos.Add(__cGDI.DrawRopLine(cPen, __cStart.X, __cStart.Y, e.X, e.Y));
	
				__cGDI.RestoreDC();
			}
		}

		public void DrawObject(Layer layer, DrawObject item) {
			Point[] cPoints = item.ConvertPoints(layer);
			bool bDraw = Boundary.BoundFix(ref cPoints[0], ref cPoints[1], new Rectangle4(layer.LayerRectangleWithoutAxisY));

			if (bDraw) {
				IntPtr cOldPen = __cGDI.SelectPen(item.Pens[0]);
				__cGDI.DrawLine(cPoints[0].X, cPoints[0].Y, cPoints[1].X, cPoints[1].Y);
				__cGDI.RemoveObject(__cGDI.SelectPen(cOldPen));
			}
		}
	}
}