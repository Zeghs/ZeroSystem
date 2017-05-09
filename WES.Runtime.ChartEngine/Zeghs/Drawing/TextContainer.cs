using System;
using System.Drawing;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Chart;

namespace Zeghs.Drawing {
	/// <summary>
	///   文字繪製容器類別
	/// </summary>
	public sealed class TextContainer : ITextContainer {
		private Font __cTextFont = null;
		private Color __cForeColor = Color.Empty;
		private Color __cBackgroundColor = Color.Empty;
		private VariableSeries<List<TextObject>> __cTextObjects = null;

		/// <summary>
		///   [取得/設定] 作用中的文字繪圖物件
		/// </summary>
		public ITextObject Active {
			get {
				List<TextObject> cObjects = __cTextObjects.Value;
				if(cObjects == null) {
					return null;
				} else {
					int iIndex = cObjects.Count - 1;
					return __cTextObjects.Value[iIndex];
				}
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public TextContainer() {
		}

		/// <summary>
		///   加入文字繪圖物件
		/// </summary>
		/// <param name="textObject">文字繪圖物件類別</param>
		public void AddTextObject(TextObject textObject) {
			lock (__cTextObjects) {
				List<TextObject> cObjects = __cTextObjects.Value;
				if (cObjects == null) {
					cObjects = new List<TextObject>(8);
					__cTextObjects.Value = cObjects;
				}

				cObjects.Add(textObject);
			}
		}

		/// <summary>
		///   清除所有文字繪圖物件
		/// </summary>
		public void Clear() {
			__cTextObjects.Dispose();
		}

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <returns>返回值: ITextObject 介面</returns>
		public ITextObject Create(ChartPoint point, string text) {
			TextObject cTextObject = CreateObject(point, text);
			AddTextObject(cTextObject);

			return cTextObject;
		}

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <param name="onSameSubchart">是否在同一個副圖中</param>
		/// <returns>返回值: ITextObject 介面</returns>
		public ITextObject Create(ChartPoint point, string text, bool onSameSubchart) {
			TextObject cTextObject = CreateObject(point, text);
			cTextObject.OnSameSubchart = onSameSubchart;
			AddTextObject(cTextObject);

			return cTextObject;
		}

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <returns>返回值: ITextObject 介面</returns>
		public ITextObject Create(ChartPoint point, string text, int dataStream) {
			TextObject cTextObject = CreateObject(point, text);
			cTextObject.DataStream = dataStream;
			AddTextObject(cTextObject);

			return cTextObject;
		}

		/// <summary>
		///   取得文字繪圖物件列表
		/// </summary>
		/// <param name="drawingSource">繪製描述來源列舉</param>
		/// <param name="barNumber">起始 Bar Number(預設: 1)</param>
		/// <param name="count">取得文字繪圖物件個數(0=全部)</param>
		/// <returns>返回值: 文字繪圖物件列表(需要使用 foreach 列舉所有文字繪圖物件)</returns>
		public IEnumerable<ITextObject> GetTextObjects(EDrawingSource drawingSource, int barNumber = 1, int count = 0) {
			int iFlag = 0;
			switch (drawingSource) {
				case EDrawingSource.AnyTech:
				case EDrawingSource.CurrentTech:
					iFlag = 1;
					break;
				case EDrawingSource.AnyTechOrManual:
				case EDrawingSource.CurrentTechOrManual:
					iFlag = 3;
					break;
				case EDrawingSource.Manual:
					iFlag = 2;
					break;
				case EDrawingSource.NotCurrentTech:
					iFlag = 6;
					break;
				case EDrawingSource.NotCurrentTechOrManual:
					iFlag = 4;
					break;
			}

			List<ITextObject> cTextObjects = new List<ITextObject>(128);
			int iCount = (count == 0) ? __cTextObjects.Count : count;
			if (iCount > 0) {
				int iBarsAgo = __cTextObjects.Current - barNumber;
				for (int i = 0; i < iCount; i++) {
					List<TextObject> cObjects = __cTextObjects[iBarsAgo - i];
					if (cObjects != null) {
						int iObjectCount = cObjects.Count;
						for (int j = 0; j < iObjectCount; j++) {
							TextObject cObject = cObjects[j];
							if (cObject.Exist && cObject.DrawingSourceFlag == iFlag) {
								cTextObjects.Add(cObject);
							}
						}
					}
				}
			}
			return cTextObjects;
		}

		/// <summary>
		///   初始化 TextContainer 類別(建立 TextObject 儲存體)
		/// </summary>
		/// <param name="master">IStudyControl 腳本控制介面</param>
		public void Initialate(IStudyControl master) {
			__cTextObjects = new VariableSeries<List<TextObject>>(master);
		}

		/// <summary>
		///   設定圖表屬性
		/// </summary>
		/// <param name="property">圖表屬性</param>
		public void SetChartProperty(ChartProperty property) {
			__cTextFont = property.TextFont;
			__cForeColor = property.ForeColor;
			__cBackgroundColor = property.BackgroundColor;
		}

		private TextObject CreateObject(ChartPoint point, string text) {
			point.BarNumber = __cTextObjects.Current;

			TextObject cObject = new TextObject();
			cObject.Location = point;
			cObject.Text = text;
			cObject.DrawingSourceFlag = 1;
			cObject.BGColor = __cBackgroundColor;
			cObject.Color = __cForeColor;
			cObject.Size = (int) __cTextFont.Size;
			cObject.FontName = __cTextFont.Name;
			return cObject;
		}
	}
}