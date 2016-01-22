using System;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   文字繪製容器介面
	/// </summary>
	public interface ITextContainer {
		/// <summary>
		///   [取得] 作用中的文字繪圖物件
		/// </summary>
		ITextObject Active {
			get;
		}

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <returns>返回值: ITextObject 介面</returns>
		ITextObject Create(ChartPoint point, string text);

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <param name="onSameSubchart">是否在同一個副圖中</param>
		/// <returns>返回值: ITextObject 介面</returns>
		ITextObject Create(ChartPoint point, string text, bool onSameSubchart);

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <returns>返回值: ITextObject 介面</returns>
		ITextObject Create(ChartPoint point, string text, int dataStream);

		/// <summary>
		///   取得文字繪圖物件列表
		/// </summary>
		/// <param name="drawingSource">繪製描述來源列舉</param>
		/// <returns>返回值: 文字繪圖物件列表(需要使用 foreach 列舉所有文字繪圖物件)</returns>
		IEnumerable<ITextObject> GetTextObjects(EDrawingSource drawingSource);
	}
}