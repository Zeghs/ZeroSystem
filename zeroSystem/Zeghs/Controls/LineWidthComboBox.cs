using System;
using System.Drawing;
using System.Windows.Forms;

namespace Zeghs.Controls {
	public sealed class LineWidthComboBox : ComboBox {
		public LineWidthComboBox() {
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		protected override void OnDrawItem(DrawItemEventArgs e) {
			Rectangle cRect = e.Bounds;

			int iWidth = e.Index + 1;
			int iHeight = cRect.Height;
			int iSize = (iWidth < iHeight) ? iWidth : iHeight;
			int iY = (iHeight - iSize) / 2;

			e.DrawBackground();
			e.Graphics.FillRectangle(Brushes.Black, cRect.X, cRect.Y + iY, cRect.Width, iSize);
			e.DrawFocusRectangle();

			base.OnDrawItem(e);
		}
	}
}