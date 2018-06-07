using System.Drawing;
using System.Collections.Generic;
using DevAge.Drawing;
using SourceGrid;

namespace Zeghs.Controls {
	internal sealed class CustomGrid : DataGrid {
		private int __iTotalSpanCount = 0;
		private List<int> __cSummation = null;

		internal CustomGrid() {
			__cSummation = new List<int>(16);
		}

		/// <summary>
		///   設定總計欄位
		/// </summary>
		/// <param name="spans">合併欄位個數陣列</param>
		internal void Summation(params int[] spans) {
			lock (__cSummation) {
				__cSummation.Clear();

				int iLength = spans.Length;
				for (int i = 0; i < iLength; i++) {
					int iCount = spans[i];
					for (int j = 0; j < iCount; j++) {
						__cSummation.Add(iCount - j);
					}
				}
			}
		}

		protected override void PaintCell(GraphicsCache graphics, CellContext cellContext, RectangleF drawRectangle) {
			Position cPosition = cellContext.Position;
			int iRowIndex = cPosition.Row;
			if (iRowIndex == this.DataSource.Count) {
				if (__iTotalSpanCount == 0) {
					__iTotalSpanCount = __cSummation[cPosition.Column];
					if (__iTotalSpanCount > 1) {
						Range cRange = cellContext.CellRange;
						cRange.ColumnsCount = __iTotalSpanCount;
						drawRectangle = this.RangeToRectangle(cRange);
					}
					
					base.PaintCell(graphics, cellContext, drawRectangle);
				}

				if (drawRectangle.Right < this.Width) {
					--__iTotalSpanCount;
				} else {
					__iTotalSpanCount = 0;
				}
			} else {
				base.PaintCell(graphics, cellContext, drawRectangle);
			}
		}
	}
}