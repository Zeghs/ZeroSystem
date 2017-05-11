using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Zeghs.Chart;
using Zeghs.Actions;

namespace Zeghs.Drawing.Actions {
	internal sealed class Cross : IAction {
		private static string __sName = "Cross";
		private static string __sComment = "Cross Line";
		private static Image[] __cImages = new Image[] { Zeghs.Properties.Resources.Cross };

		private Gdi __cGDI = null;
		private GdiEngine __cEngine = null;
		private IAction __cCustomPainter = null;
		private List<_LineInfo> __cLineInfos = null;
		private List<_TextInfo> __cTextInfos = null;

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

		internal Cross(GdiEngine engine) {
			__cGDI = engine.GDI;
			__cEngine = engine;

			__cLineInfos = new List<_LineInfo>(2);
			__cTextInfos = new List<_TextInfo>(2);
		}

		public void Action(ChartParameter parameter) {
			__cGDI.ClearRops(true);
			__cGDI.ClearRops(__cLineInfos, __cTextInfos, !parameter.Updated);

			InputDeviceStatus cStatus = parameter.Status;
			if (cStatus.Event == EInputDeviceEvent.MouseMove) {
				ZChart cChart = parameter.Chart;
				AxisX cAxisX = cChart.AxisX;
				ChartProperty cProperty = cChart.ChartProperty;
				MouseEventArgs e = cStatus.GetCurrentMouseArgs();

				int iOldBKColor = __cGDI.SelectBackground(cProperty.BackgroundColor);
				IntPtr cOldFont = __cGDI.SelectFont(cProperty.AxisFont);
				IntPtr cPen = Gdi.CreatePen(new PowerLanguage.PenStyle(cProperty.ForeColor, 1));

				int iBarNumber = cAxisX.ConvertBarNumberFromX(e.X);
				if (iBarNumber > cAxisX.DataCount) {
					return;
				} else {
					Rectangle cAxisXRect = cAxisX.AxisRectangle;
					AxisXUnit cUnit = cAxisX.ConvertBarNumberToWidth(iBarNumber);
					cAxisXRect.X = cUnit.CenterPoint;
					cAxisXRect.Width = cAxisX.FontMetrics.Width * 12;

					DateTime cDateTime = cAxisX.ConvertBarNumberToTime(iBarNumber);
					__cLineInfos.Add(__cGDI.DrawRopLine(cPen, cUnit.CenterPoint, 0, cUnit.CenterPoint, cAxisXRect.Y));
					__cTextInfos.Add(__cGDI.DrawRopString(cDateTime.ToString("MM/dd HH:mm"), cProperty.BackgroundColor, cProperty.ForeColor, 2, 5, cAxisXRect));
				}
				
				List<Layer> cLayers = cChart.Layers;
				int iCount = cLayers.Count;
				for (int i = 0; i < iCount; i++) {
					Layer cLayer = cLayers[i];
					if (__cLineInfos.Count == 1 && cLayer.IsLayerScope(e.X, e.Y)) {
						AxisY cAxisY = cLayer.AxisY;
						Rectangle cAxisYRect = cAxisY.AxisRectangle;
						cAxisYRect.Y = e.Y;
						cAxisYRect.Height = cAxisY.FontMetrics.Height;

						__cLineInfos.Add(__cGDI.DrawRopLine(cPen, 0, e.Y, cAxisYRect.X, e.Y));
						__cTextInfos.Add(__cGDI.DrawRopString(Math.Round(cAxisY.ConvertValueFromY(e.Y), cAxisY.Decimals).ToString(), cProperty.BackgroundColor, cProperty.ForeColor, 5, 0, cAxisYRect));
					}

					cLayer.LegendIndex = iBarNumber;
					__cEngine.DrawLegend(cLayer, cProperty);
				}

				__cGDI.RemoveObject(__cGDI.SelectFont(cOldFont));
				__cGDI.SelectBackground(iOldBKColor);
			} 

			//如果使用者使用十字線功能, 如果有在選擇繪圖功能會在 CustomPainter 屬性儲存繪圖功能的類別名稱
			//如果有繪圖類別名稱就取出使用
			string sName = parameter.CustomPainter;
			if (__cCustomPainter == null && sName != null) {
				__cCustomPainter = parameter.Behavior.GetCustomAction(sName);
				if (__cCustomPainter == null || !(__cCustomPainter is IDrawable)) {
					parameter.CustomPainter = null;
				}
			}

			if (__cCustomPainter != null) {
				__cCustomPainter.Action(parameter);
				
				//如果繪圖類別名稱 == null 表示繪圖已經完畢
				if (parameter.CustomPainter == null) {
					__cCustomPainter = null;
				}
			}
		}
	}
}