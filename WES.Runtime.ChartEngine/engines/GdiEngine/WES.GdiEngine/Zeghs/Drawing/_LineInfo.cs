using System;
using System.Drawing;

namespace Zeghs.Drawing {
	internal sealed class _LineInfo {
		private Point[] __cPoints;
		private IntPtr __cPen = IntPtr.Zero;

		internal IntPtr Pen {
			get {
				return __cPen;
			}
		}

		internal Point[] Points {
			get {
				return __cPoints;
			}
		}

		internal _LineInfo(IntPtr pen, Point[] points) {
			__cPen = pen;
			__cPoints = points;
		}
	}
}