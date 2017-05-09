using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Actions {
	/// <summary>
	///   使用者繪製物件保存容器類別
	/// </summary>
	public sealed class DrawContainer {
		private Dictionary<int, List<DrawObject>> __cDraws = null;

		internal DrawContainer() {
			__cDraws = new Dictionary<int, List<DrawObject>>(128);
		}

		/// <summary>
		///   加入使用者繪製物件
		/// </summary>
		/// <param name="name">繪製物件名稱</param>
		/// <param name="layer">繪製的目標 Layer 圖層</param>
		/// <param name="points">Point 繪製點陣列</param>
		/// <param name="pens">繪製筆刷陣列</param>
		public void AddDrawObject(string name, Layer layer, Point[] points, PenStyle[] pens) {
			AxisX cAxisX = layer.AxisX;
			AxisY cAxisY = layer.AxisY;

			int iMin = int.MaxValue, iMax = int.MinValue, iLength = points.Length;
			ChartPoint[] cPoints = new ChartPoint[iLength];
			for (int i = 0; i < iLength; i++) {
				Point cPoint = points[i];
				int iBarNumber = cAxisX.ConvertBarNumberFromX(cPoint.X);
				ChartPoint cChartPoint = new ChartPoint(iBarNumber, cAxisY.ConvertValueFromY(cPoint.Y));
				cChartPoint.Time = cAxisX.ConvertBarNumberToTime(iBarNumber);
				cPoints[i] = cChartPoint;

				if (iMin > iBarNumber) {
					iMin = iBarNumber;
				}

				if (iMax < iBarNumber) {
					iMax = iBarNumber;
				}
			}
			AddDrawObjectFromRange(iMin, iMax, new DrawObject(name, layer.LayerIndex, cPoints, pens));
		}

		/// <summary>
		///   取得區間內的使用者繪製物件
		/// </summary>
		/// <param name="min">最小 barNumber 編號</param>
		/// <param name="max">最大 barNumber 編號</param>
		/// <returns>返回值: 使用者繪製物件的 HashSet 集合</returns>
		public HashSet<DrawObject> GetDrawObjects(int min, int max) {
			HashSet<DrawObject> cDrawObjects = new HashSet<DrawObject>();
			for (int i = min; i <= max; i++) {
				List<DrawObject> cList = GetDrawObjects(i);
				
				int iCount = cList.Count;
				for(int j = 0; j < iCount; j++) {
					cDrawObjects.Add(cList[j]);
				}
			}
			return cDrawObjects;
		}

		private void AddDrawObjectFromRange(int min, int max, DrawObject item) {
			List<DrawObject> cList = GetDrawObjects(min);
			cList.Add(item);

			if (max > min) {
				cList = GetDrawObjects(max);
				cList.Add(item);
			}
		}

		private List<DrawObject> GetDrawObjects(int barNumber) {
			List<DrawObject> cList = null;
			if (!__cDraws.TryGetValue(barNumber, out cList)) {
				cList = new List<DrawObject>(8);
				__cDraws.Add(barNumber, cList);
			}
			return cList;
		}
	}
}