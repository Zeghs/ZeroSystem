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
		/// <param name="absolutePosition">是否座標位置為螢幕座標而非價格與時間軸座標</param>
		/// <returns>返回值: ITextObject 介面</returns>
		ITextObject Create(ChartPoint point, string text, bool absolutePosition);

		/// <summary>
		///   建立文字繪圖物件
		/// </summary>
		/// <param name="point">文字繪圖物件的座標位置</param>
		/// <param name="text">繪圖文字內容</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="absolutePosition">是否座標位置為螢幕座標而非價格與時間軸座標</param>
		/// <returns>返回值: ITextObject 介面</returns>
		ITextObject Create(ChartPoint point, string text, int dataStream, bool absolutePosition = false);

		/// <summary>
		///   取得文字繪圖物件列表
		/// </summary>
		/// <param name="drawingSource">繪製描述來源列舉</param>
		/// <param name="index">起始 Bar Number(預設: 1)</param>
		/// <param name="count">取得個數(0=全部)</param>
		/// <returns>返回值: 文字繪圖物件列表(需要使用 foreach 列舉所有文字繪圖物件)</returns>
		IEnumerable<ITextObject> GetTextObjects(EDrawingSource drawingSource, int index = 1, int count = 0);
	}
}