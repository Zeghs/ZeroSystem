using System;
using System.Drawing;

namespace Zeghs.Drawing {
	internal sealed class _TextInfo {
		private Rectangle __cRect;
		private IntPtr[] __cHDCs = null;

		internal IntPtr[] hDCs {
			get {
				return __cHDCs;
			}
		}

		internal Rectangle Rectangle {
			get {
				return __cRect;
			}
		}

		internal _TextInfo(IntPtr[] hDCs, int x, int y, int width, int height) {
			__cHDCs = hDCs;
			__cRect = new Rectangle(x, y, width, height);
		}
	}
}
