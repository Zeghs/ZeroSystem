using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Zeghs.Drawing;

namespace Zeghs.Forms {
	public partial class frmTest : Form {
		[DllImport("user32.dll")]
		static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

		[DllImport("gdi32.dll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
		static extern IntPtr CreateSolidBrush(int crColor);

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		static extern bool DeleteObject(IntPtr hObject);
		
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left, Top, Right, Bottom;

			public RECT(int left, int top, int right, int bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) {
			}

			public int X {
				get {
					return Left;
				}
				set {
					Right -= (Left - value);
					Left = value;
				}
			}

			public int Y {
				get {
					return Top;
				}
				set {
					Bottom -= (Top - value);
					Top = value;
				}
			}

			public int Height {
				get {
					return Bottom - Top;
				}
				set {
					Bottom = value + Top;
				}
			}

			public int Width {
				get {
					return Right - Left;
				}
				set {
					Right = value + Left;
				}
			}

			public System.Drawing.Point Location {
				get {
					return new System.Drawing.Point(Left, Top);
				}
				set {
					X = value.X;
					Y = value.Y;
				}
			}

			public System.Drawing.Size Size {
				get {
					return new System.Drawing.Size(Width, Height);
				}
				set {
					Width = value.Width;
					Height = value.Height;
				}
			}

			public static implicit operator System.Drawing.Rectangle(RECT r) {
				return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
			}

			public static implicit operator RECT(System.Drawing.Rectangle r) {
				return new RECT(r);
			}

			public static bool operator ==(RECT r1, RECT r2) {
				return r1.Equals(r2);
			}

			public static bool operator !=(RECT r1, RECT r2) {
				return !r1.Equals(r2);
			}

			public bool Equals(RECT r) {
				return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
			}

			public override bool Equals(object obj) {
				if (obj is RECT)
					return Equals((RECT) obj);
				else if (obj is System.Drawing.Rectangle)
					return Equals(new RECT((System.Drawing.Rectangle) obj));
				return false;
			}

			public override int GetHashCode() {
				return ((System.Drawing.Rectangle) this).GetHashCode();
			}

			public override string ToString() {
				return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
			}
		}

		public frmTest() {
			InitializeComponent();
		}

		private void frmTest_Load(object sender, EventArgs e) {
			//ImgProcTest3();
		}

		protected override void OnPaint(PaintEventArgs e) {
			//base.OnPaint(e);

			//Bitmap bmp = new Bitmap(100, 100);
			Graphics g = e.Graphics;
			g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

			Gdi gg = new Gdi(this.Handle);

			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 100000; i++) {
				//g.FillRectangle(Brushes.Black, 0, 0, 100, 100);
				//g.DrawLine(Pens.Black, 0, 0, 100, 100);
				//gg.DrawLine(Pens.Black, 0, 0, 100, 100);
				//gg.DrawRectangle(Pens.Red, 100, 100, 200, 200);
				//g.DrawRectangle(Pens.Red, 100, 100, 200, 200);
			}

			gg.Dispose();

			//e.Graphics.DrawImageUnscaled(bmp, 0, 0);

			sw.Stop();
			System.Console.WriteLine(sw.ElapsedMilliseconds);
		}
		
		private void ImgProcTest3() {
			Bitmap bmp = new Bitmap(100, 100);

			BitmapData data =
			    bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			    ImageLockMode.WriteOnly,
			    PixelFormat.Format24bppRgb);
			Stopwatch sw = new Stopwatch();
			sw.Start();

			unsafe {
				for (int k = 0; k < 50000; k++) {
					byte* ptr = (byte*) (data.Scan0);
					int padding = data.Stride - (bmp.Width * 3);

					for (int i = 0; i < data.Height; i++) {
						for (int j = 0; j < data.Width; j++) {
							//ptr[0] -> B, ptr[1] -> G, ptr[2] -> R
							ptr[0] = ptr[1] = ptr[2] = 0;
							ptr += 3;
						}
						ptr += padding;
					}
				}
			}

	
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds.ToString());

			bmp.UnlockBits(data);

			pictureBox1.Image = bmp;
		}
	}
}
