using System.Drawing;
using System.Runtime.InteropServices;

namespace Zeghs.Drawing {
	[StructLayout(LayoutKind.Sequential)]
	internal struct Rectangle4 {
		private int __iLeft, __iTop, __iRight, __iBottom;

		internal Rectangle4(int left, int top, int right, int bottom) {
			__iLeft = left;
			__iTop = top;
			__iRight = right;
			__iBottom = bottom;
		}

		internal Rectangle4(Rectangle r)
			: this(r.Left, r.Top, r.Right, r.Bottom) {
		}

		internal int Bottom {
			get {
				return __iBottom;
			}

			set {
				__iBottom = value;
			}
		}

		internal int Left {
			get {
				return __iLeft;
			}

			set {
				__iRight -= (__iLeft - value);
				__iLeft = value;
			}
		}

		internal int Top {
			get {
				return __iTop;
			}
	
			set {
				__iBottom -= (__iTop - value);
				__iTop = value;
			}
		}

		internal int Height {
			get {
				return __iBottom - __iTop;
			}
			
			set {
				__iBottom = value + __iTop;
			}
		}

		internal int Right {
			get {
				return __iRight;
			}

			set {
				__iRight = value;
			}
		}

		internal int Width {
			get {
				return __iRight - __iLeft;
			}
			
			set {
				__iRight = value + __iLeft;
			}
		}

		public static implicit operator Rectangle(Rectangle4 r) {
			return new Rectangle(r.__iLeft, r.__iTop, r.Width, r.Height);
		}

		public static implicit operator Rectangle4(System.Drawing.Rectangle r) {
			return new Rectangle4(r);
		}

		public static bool operator ==(Rectangle4 r1, Rectangle4 r2) {
			return r1.Equals(r2);
		}

		public static bool operator !=(Rectangle4 r1, Rectangle4 r2) {
			return !r1.Equals(r2);
		}

		public bool Equals(Rectangle4 r) {
			return r.__iLeft == __iLeft && r.__iTop == __iTop && r.__iRight == __iRight && r.__iBottom == __iBottom;
		}

		public override bool Equals(object obj) {
			if (obj is Rectangle4)
				return Equals((Rectangle4) obj);
			else if (obj is System.Drawing.Rectangle)
				return Equals(new Rectangle4((System.Drawing.Rectangle) obj));
			return false;
		}

		public override int GetHashCode() {
			return ((Rectangle) this).GetHashCode();
		}

		public override string ToString() {
			return string.Format("{{Left={0}, Top={1}, Right={2}, Bottom={3}}}", __iLeft, __iTop, __iRight, __iBottom);
		}
	}
}