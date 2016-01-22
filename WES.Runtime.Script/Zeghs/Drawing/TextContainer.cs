using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Settings;

namespace Zeghs.Drawing {
	/// <summary>
	///   文字繪製容器類別
	/// </summary>
	public sealed class TextContainer : ITextContainer {
		private ChartProperty __cProperty = null;
		private List<TextObject> __cTextObjects = null;

		/// <summary>
		///   [取得/設定] 作用中的文字繪圖物件
		/// </summary>
		public ITextObject Active {
			get;
			set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		public TextContainer() {
			__cTextObjects = new List<TextObject>(128);
		}

		/// <summary>
		///   加入文字繪圖物件
		/// </summary>
		/// <param name="textObject">文字繪圖物件類別</param>
		public void AddTextObject(TextObject textObject) {
			lock (__cTextObjects) {
				__cTextObjects.Add(textObject);
			}
		}

		/// <summary>
		///   清除所有文字繪圖物件
		/// </summary>
		public void Clear() {
			__cTextObjects.Clear();
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
		/// <returns>返回值: 文字繪圖物件列表(需要使用 foreach 列舉所有文字繪圖物件)</returns>
		public IEnumerable<ITextObject> GetTextObjects(EDrawingSource drawingSource) {
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
			int iCount = __cTextObjects.Count;
			if (iCount > 0) {
				for (int i = 0; i < iCount; i++) {
					TextObject cTextObject = __cTextObjects[i];
					int iDrawingFlag = cTextObject.DrawingSourceFlag;
					if (cTextObject.Exist && (iFlag & iDrawingFlag) == iDrawingFlag) {
						cTextObjects.Add(cTextObject);
					}
				}
			}
			return cTextObjects;
		}

		/// <summary>
		///   設定圖表屬性
		/// </summary>
		/// <param name="property">圖表屬性</param>
		public void SetChartProperty(ChartProperty property) {
			__cProperty = property;
		}

		private TextObject CreateObject(ChartPoint point, string text) {
			TextObject cObject = new TextObject();
			cObject.Location = point;
			cObject.Text = text;
			cObject.DrawingSourceFlag = 1;
			cObject.BGColor = __cProperty.BackgroundColor;
			cObject.Color = __cProperty.ForeColor;
			cObject.Size = __cProperty.FontSize;
			cObject.FontName = __cProperty.FontName;

			return cObject;
		}
	}
}