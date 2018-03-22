using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Zeghs.Forms;

namespace Zeghs.Controls {
	internal sealed class CustomTabControl : TabControl {
		private const int TCM_ADJUSTRECT = 0x1328;
		
		private struct RECT {
			internal int Left, Top, Right, Bottom;
		}

		internal CustomTabControl() {
			this.Alignment = TabAlignment.Bottom;
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