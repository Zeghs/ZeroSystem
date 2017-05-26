using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Drawing.Plots {
	internal sealed class PlotTextObject : AbstractPlot {
		private Gdi __cGDI = null;
		private List<Layer> __cLayers = null;
		private TextContainer __cTextObjects = null;
		Dictionary<int, Queue<TextObject>> __cObjects = null;

		protected override int Current {
			get {
				return 0;
			}
		}

		protected override int SeriesCount {
			get {
				return 0;
			}
		}

		internal PlotTextObject(GdiEngine engine, ZChart chart, object source, ChartSetting chartSetting)
			: base(engine, chartSetting) {
			
			__cGDI = engine.GDI;
			__cTextObjects = source as TextContainer;
			__cTextObjects.SetChartProperty(chart.ChartProperty);

			__cLayers = new List<Layer>(8);
			__cObjects = new Dictionary<int, Queue<TextObject>>(8);
			__cLayers = chart.Layers;
		}

		public override void CalculatePlot(AxisX axis) {
		}

		public override bool DrawPlot(Layer layer, ChartProperty property, bool onlyUpdateLastBar) {
			AxisX cAxisX = layer.AxisX;

			IntPtr iOldFont = __cGDI.SelectFont(property.TextFont);

			int iIndex = cAxisX.BarNumber, iCount = 1;
			if (onlyUpdateLastBar) {
				iIndex = cAxisX.BarNumber + cAxisX.BarCount - 1;
				iIndex = (iIndex > cAxisX.DataCount) ? cAxisX.DataCount : iIndex;
			} else {
				iCount = cAxisX.BarCount;
				int iEndIndex = cAxisX.BarNumber + iCount - 1;
				iCount = (iEndIndex > cAxisX.DataCount) ? cAxisX.DataCount - cAxisX.BarNumber : iCount;
			}

			IEnumerable<ITextObject> cTextObjects = __cTextObjects.GetTextObjects(property.DrawingSource, iIndex, iCount);
			ProccessObjects(cTextObjects, property, onlyUpdateLastBar);

			__cGDI.RemoveObject(__cGDI.SelectFont(iOldFont));
			return true;
		}

		protected override void DrawLegend(int barNumber, ChartProperty property) {
		}

		private int CalculateXFromStyle(int x, Size size, ETextStyleH style) {
			switch (style) {
				case ETextStyleH.Center:
					x -= size.Width / 2;
					break;
				case ETextStyleH.Right:
					x -= size.Width;
					break;
			}
			return x;
		}

		private int CalculateYFromStyle(int y, Size size, ETextStyleV style) {
			switch (style) {
				case ETextStyleV.Above:
					y -= size.Height;
					break;
				case ETextStyleV.Center:
					y -= size.Height / 2;
					break;
			}
			return y;
		}

		private void DrawTextObject(Layer layer, TextObject textObject, ChartProperty property, bool useROP) {
			AxisX cAxisX = layer.AxisX;
			AxisY cAxisY = layer.AxisY;
			Rectangle cRectY = cAxisY.AxisRectangle;

			int iOldBackground = 0;
			IntPtr iOldFont = IntPtr.Zero;
			bool bUseBG = textObject.BGColor != property.BackgroundColor;
			bool bUseFont = textObject.FontName != property.TextFont.Name || textObject.Size != property.TextFont.Size;
			if (bUseBG) {
				iOldBackground = __cGDI.SelectBackground(textObject.BGColor);
			} else {
				iOldBackground = __cGDI.SelectTransparent();
			}

			if (bUseFont) {
				iOldFont = __cGDI.SelectFont(new Font(textObject.FontName, textObject.Size, textObject.Style));
			}

			int iX = 0, iY = 0;
			ChartPoint cPoint = textObject.Location;
			bool bAbsolute = textObject.AbsolutePosition;
			if (bAbsolute) {
				iY = cRectY.Y + (int) cPoint.Price;
				iX = cPoint.BarNumber.Value;
			} else {
				iY = cRectY.Y + cAxisY.ConvertValueToHeight(cPoint.Price) + 1;
				AxisXUnit cUnit = cAxisX.ConvertBarNumberToWidth(cPoint.BarNumber.Value);
				Size cSize = __cGDI.MeasureString(textObject.Text);

				iY = CalculateYFromStyle(iY, cSize, textObject.VStyle);
				iX = CalculateXFromStyle(cUnit.CenterPoint, cSize, textObject.HStyle);
			}

			Rectangle cLayerRect = layer.LayerRectangleWithoutAxisY;
			if (useROP || bAbsolute) {
				__cGDI.BeginRopDraw();
				__cGDI.DrawRopString(textObject.Text, textObject.Color, iX, iY);
				__cGDI.EndRopDraw();
			} else {
				__cGDI.DrawString(textObject.Text, textObject.Color, iX, iY);
			}

			if (bUseFont) {
				__cGDI.RemoveObject(__cGDI.SelectFont(iOldFont));
			}

			if (bUseBG) {
				__cGDI.SelectBackground(iOldBackground);
			} else {
				__cGDI.ClearTransparent(iOldBackground);
			}
		}

		private void ProccessObjects(IEnumerable<ITextObject> textObjects, ChartProperty property, bool useROP) {
			//將 TextObject 物件按照 DataStream 分類
			foreach (TextObject cTextObject in textObjects) {
				Queue<TextObject> cQueue = null;
				if (!__cObjects.TryGetValue(cTextObject.DataStream, out cQueue)) {
					cQueue = new Queue<TextObject>(128);
					__cObjects.Add(cTextObject.DataStream, cQueue);
				}
				cQueue.Enqueue(cTextObject);
			}

			//將分類好的 TextObject 顯示在 Chart 上
			foreach (int iDataStream in __cObjects.Keys) {
				ChartSetting cChartSetting = property.ChartSettings[iDataStream - 1];
				if (cChartSetting.IsSubChart) {
					Layer cLayer = __cLayers[cChartSetting.LayerIndex];
					Rectangle cLayerRect = cLayer.LayerRectangleWithoutAxisY;

					__cGDI.SaveDC();
					__cGDI.ClipRectangle(cLayerRect);

					Queue<TextObject> cQueue = __cObjects[iDataStream];
					while (cQueue.Count > 0) {
						DrawTextObject(cLayer, cQueue.Dequeue(), property, useROP);
					}
					__cGDI.RestoreDC();
				}
			}
		}
	}
}