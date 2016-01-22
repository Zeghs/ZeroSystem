using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Zeghs.Forms {
	internal partial class frmChildBase : Form {
		private const int HTCAPTION = 0x0002;
		private const int WM_NCHITTEST = 0x0084;
		private const int WM_NCLBUTTONDOWN = 0x00a1;
		private static readonly Color 基礎子表單標題明亮色 = Color.FromArgb(2, 102, 251);
		private static readonly Color 基礎子表單標題陰暗色 = Color.FromArgb(0, 87, 228);

		[DllImportAttribute("user32.dll")]
		private static extern bool ReleaseCapture();

		[DllImportAttribute("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		private int __iControlBoxType = -1;

		protected int TitleHeight {
			get;
			set;
		}

		protected bool ShowTitle {
			get;
			set;
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams cParms = base.CreateParams;
				cParms.Style &= ~0x00C00000; //取消視窗標題 WS_CAPTION
				if (this.FormBorderStyle == FormBorderStyle.Fixed3D || this.FormBorderStyle == FormBorderStyle.FixedDialog || this.FormBorderStyle == FormBorderStyle.FixedSingle || this.FormBorderStyle == FormBorderStyle.FixedToolWindow) {
					cParms.Style |= 0x00800000;  //加入可框線屬性 WS_BORDER
				} else {
					cParms.Style |= 0x00040000;  //加入可調整屬性 WS_SIZEBOX
				}
				return cParms;
			}
		}

		internal frmChildBase() {
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

			InitializeComponent();

			this.TitleHeight = 20;
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			Graphics g = e.Graphics;
			int iWidth = ClientSize.Width;
			Rectangle cTitleRect = new Rectangle(0, 0, iWidth, TitleHeight);

			LinearGradientBrush cLGBrush = new LinearGradientBrush(cTitleRect, 基礎子表單標題明亮色, 基礎子表單標題陰暗色, LinearGradientMode.Vertical);
			g.FillRectangle(cLGBrush, cTitleRect);
			cLGBrush.Dispose();

			if (this.ShowTitle) {
				Font cFont = new Font("微軟正黑體", 9, FontStyle.Bold);
				SolidBrush cBrush = new SolidBrush(Color.White);
				g.DrawString(this.Text, cFont, cBrush, 1, 1);
				cBrush.Dispose();
				cFont.Dispose();
			}

			float fY = 1;
			float fX = iWidth - 18 * 3;

			if (this.MinimizeBox) {
				g.DrawImage(((__iControlBoxType == 10) ? ZeroSystem.Properties.Resources.frmChildBase_MouseMove_minbox : ZeroSystem.Properties.Resources.frmChildBase_minbox), fX, fY, 18, 17);
			}

			fX += 18;
			if (this.MaximizeBox) {
				g.DrawImage(((__iControlBoxType == 11) ? ZeroSystem.Properties.Resources.frmChildBase_MouseMove_maxbox : ZeroSystem.Properties.Resources.frmChildBase_maxbox), fX, fY, 18, 17);
			}

			fX += 18;
			if (this.ControlBox) {
				g.DrawImage(((__iControlBoxType == 12) ? ZeroSystem.Properties.Resources.frmChildBase_MouseMove_closebox : ZeroSystem.Properties.Resources.frmChildBase_closebox), fX, fY, 18, 17);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left) {
				int iFlag = CheckControlBox(e.X, e.Y);
				if (iFlag == -1) {
					if (e.X > 0 && e.X < this.Width && e.Y > 0 && e.Y < this.TitleHeight) {
						ReleaseCapture();
						SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
					}
				} else {
					__iControlBoxType = iFlag;
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			if (e.Button == MouseButtons.None) {
				int iFlag = CheckControlBox(e.X, e.Y);
				if (iFlag > -1) {
					if ((iFlag == 0 && !this.MinimizeBox) || (iFlag == 1 && !this.MaximizeBox)) {
						return;
					}

					__iControlBoxType = iFlag + 10;
					this.Invalidate(true);
				} else if (iFlag != __iControlBoxType) {
					__iControlBoxType = -1;
					this.Invalidate(true);
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);

			if (e.Button == MouseButtons.Left) {
				int iFlag = CheckControlBox(e.X, e.Y);
				if (iFlag == __iControlBoxType) {
					switch (iFlag) {
						case 0: //最小化
							if (MinimizeBox) {
								this.WindowState = FormWindowState.Minimized;
							}
							break;
						case 1: //最大化
							if (this.MaximizeBox) {
								if (this.WindowState == FormWindowState.Maximized) {
									this.WindowState = FormWindowState.Normal;
								} else {
									this.WindowState = FormWindowState.Maximized;
								}
							}
							break;
						case 2: //關閉程式
							if (this.ControlBox) {
								this.Close();
							}
							break;
					}
				}
			}

			__iControlBoxType = -1;
			this.Invalidate(true);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);

			this.Invalidate(true);
		}

		private int CheckControlBox(int x, int y) {
			int iTop = 0;
			int iLeft = this.ClientSize.Width - 18 * 3;

			for (int i = 0; i < 3; i++) {
				int iX1 = iLeft, iX2 = iLeft + 18;
				int iY1 = iTop, iY2 = iTop + 17;

				if (x > iX1 && x < iX2 && y > iY1 && y < iY2) {
					return i;
				}

				iLeft += 18;
			}
			return -1;
		}
	}
}