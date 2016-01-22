using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Zeghs.Forms;

namespace Zeghs.Controls {
	public sealed class CustomTabControl : TabControl {
		private const int TCM_ADJUSTRECT = 0x1328;
		
		private struct RECT {
			internal int Left, Top, Right, Bottom;
		}

		public CustomTabControl() {
			base.SetStyle(
			     ControlStyles.UserPaint |
			     ControlStyles.ResizeRedraw |
			     ControlStyles.OptimizedDoubleBuffer |
			     ControlStyles.AllPaintingInWmPaint |
			     ControlStyles.SupportsTransparentBackColor,
			     true);
			
			base.UpdateStyles();

			this.Alignment = TabAlignment.Bottom;
		}

		protected override void OnPaint(PaintEventArgs e) {
			Graphics g = e.Graphics;
			int iSelectedIndex = this.SelectedIndex;

			Rectangle cTabControlRect = this.ClientRectangle;
			float fPY = cTabControlRect.Y + cTabControlRect.Height - 23;
			g.FillRectangle(Brushes.White, cTabControlRect.X, cTabControlRect.Y, cTabControlRect.Width, cTabControlRect.Height - 23);
			g.DrawLine(Pens.Black, cTabControlRect.X, fPY, cTabControlRect.Width - 1, fPY);

			StringFormat cStringFormat = new StringFormat();
			cStringFormat.LineAlignment = StringAlignment.Center;
			cStringFormat.Alignment = StringAlignment.Center;

			int iTabCount = this.TabCount;
			for (int i = 0; i < iTabCount; i++) {
				TabPage cTabPage = this.TabPages[i];
				Rectangle cTabPageRect = this.GetTabRect(i);

				SolidBrush cLGBrush = new SolidBrush((i == iSelectedIndex) ? Color.White : SystemColors.Control);
				g.FillRectangle(cLGBrush, cTabPageRect);
				cLGBrush.Dispose();

				string sText = cTabPage.Text;
				Font cFont = cTabPage.Font;
				SizeF cTextSize = g.MeasureString(sText, cFont, cTabPageRect.Width, cStringFormat);
				PointF cTextPoint = new PointF(cTabPageRect.X + ((cTabPageRect.Width - cTextSize.Width) / 2), cTabPageRect.Y + ((cTabPageRect.Height - cTextSize.Height) / 2));

				g.DrawRectangle(Pens.Black, cTabPageRect);
				if (i == iSelectedIndex) {
					g.DrawLine(Pens.White, cTabPageRect.X, cTabPageRect.Y, cTabPageRect.X + cTabPageRect.Width, cTabPageRect.Y);
					g.DrawString(sText, cFont, Brushes.Black, cTextPoint);
				} else {
					g.DrawString(sText, cFont, Brushes.Gray, cTextPoint);
				}
			}
			cStringFormat.Dispose();
		}
		
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if (m.Msg == TCM_ADJUSTRECT) {
				RECT rc = (RECT) m.GetLParam(typeof(RECT));
				rc.Left -= 3;
				rc.Top -= 3;
				rc.Right += 3;
				rc.Bottom += 1;
				Marshal.StructureToPtr(rc, m.LParam, true);
			}
			base.WndProc(ref m);
		}
	}
}