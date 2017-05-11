using System;
using System.Drawing;
using Zeghs.Drawing;

namespace Zeghs.Drawing {
	internal sealed class Boundary {
		internal static bool BoundFix(ref Point point1, ref Point point2, Rectangle4 area) {
			double dMY = point2.Y - point1.Y, dMX = point2.X - point1.X;
			double dM = dMY / dMX;

			bool bRet1 = CheckBound(ref point1, point1, area, dM);
			bool bRet2 = CheckBound(ref point2, point1, area, dM);
			return (bRet1 || bRet2) && point1 != point2;
		}

		internal static bool RayFix(ref Point point1, ref Point point2, Rectangle4 area) {
			bool bRet = true;
			double dMY = point2.Y - point1.Y, dMX = point2.X - point1.X;
			double dM = dMY / dMX;
			Point cBasePoint = point1;
			if (point1.X > point2.X) {
				if (!FixRayFromX(ref point1, cBasePoint, area, dM, area.Right)) {
					bRet = FixRayFromY(ref point1, cBasePoint, area, dM, (point1.Y > point2.Y) ? area.Bottom : area.Top);
				}

				if (!FixRayFromX(ref point2, cBasePoint, area, dM, area.Left)) {
					bRet = FixRayFromY(ref point2, cBasePoint, area, dM, (point2.Y > cBasePoint.Y) ? area.Bottom : area.Top);
				}
			} else {
				if (double.IsInfinity(dM)) {
					if (point1.X >= area.Left && point1.X <= area.Right) {
						if (point1.Y > point2.Y) {
							point1.Y = area.Bottom;
							point2.Y = area.Top;
						} else {
							point1.Y = area.Top;
							point2.Y = area.Bottom;
						}
					} else {
						bRet = false;
					}
				} else {
					if (!FixRayFromX(ref point1, cBasePoint, area, dM, area.Left)) {
						bRet = FixRayFromY(ref point1, cBasePoint, area, dM, (point1.Y > point2.Y) ? area.Bottom : area.Top);
					}

					if (!FixRayFromX(ref point2, cBasePoint, area, dM, area.Right)) {
						bRet = FixRayFromY(ref point2, cBasePoint, area, dM, (point2.Y > cBasePoint.Y) ? area.Bottom : area.Top);
					}
				}
			}
			return bRet;
		}

		internal static bool RayRightFix(ref Point point1, ref Point point2, Rectangle4 area) {
			Point cPoint1 = point1;
			bool bRet = RayFix(ref cPoint1, ref point2, area);
			if (bRet && (point1.Y < area.Top || point1.Y > area.Bottom)) {
				point1 = cPoint1;
			}
			return bRet;
		}

		internal static bool TextFix(ref Point point, Rectangle4 area, SizeF textSize) {
			int iY = point.Y;
			int iTextWidth = (int) textSize.Width;
			int iX = point.X - iTextWidth / 2, iW = point.X + iTextWidth / 2;
			point.X = (iX <= area.Left) ? point.X : (iW >= area.Right) ? area.Right - iTextWidth : iX;
			return iY >= area.Top && iY <= area.Bottom;
		}

		private static int CalculateX(Point point, double m, int y) {
			double dX = ((y - point.Y) + m * point.X) / m;
			return (int) dX;
		}

		private static int CalculateY(Point point, double m, int x) {
			double dY = (m * x - m * point.X) + point.Y;
			return (int) dY;
		}

		private static bool CheckBound(ref Point point, Point point1, Rectangle4 area, double m) {
			int iX = point.X, iY = point.Y;
			if (iX < area.Left) {
				int iAL = area.Left;
				int iFY = CalculateY(point1, m, iAL);
				if (iFY >= area.Top && iFY <= area.Bottom) {
					point.X = iAL;
					point.Y = iFY;
					return true;
				}
			}

			if (iX > area.Right) {
				int iAR = area.Right;
				int iFY = CalculateY(point1, m, iAR);
				if (iFY >= area.Top && iFY <= area.Bottom) {
					point.X = iAR;
					point.Y = iFY;
					return true;
				}
			}

			if (iY < area.Top) {
				int iAT = area.Top;
				if (double.IsInfinity(m)) {
					point.Y = iAT;
					return point.X >= area.Left && point.X <= area.Right;
				} else {
					int iFX = CalculateX(point1, m, iAT);
					if (iFX >= area.Left && iFX <= area.Right) {
						point.X = iFX;
						point.Y = iAT;
						return true;
					}
				}
			}

			if (iY > area.Bottom) {
				int iAB = area.Bottom;
				if (double.IsInfinity(m)) {
					point.Y = iAB;
					return point.X >= area.Left && point.X <= area.Right;
				} else {
					int iFX = CalculateX(point1, m, iAB);
					if (iFX >= area.Left && iFX <= area.Right) {
						point.X = iFX;
						point.Y = iAB;
						return true;
					}
				}
			}
			return point.X >= area.Left && point.X <= area.Right && point.Y >= area.Top && point.Y <= area.Bottom;
		}

		private static bool FixRayFromX(ref Point point, Point point1, Rectangle4 area, double m, int x) {
			int iY = CalculateY(point1, m, x);
			if (iY >= area.Top && iY <= area.Bottom) {
				point.X = x;
				point.Y = iY;
				return true;
			}
			return false;
		}

		private static bool FixRayFromY(ref Point point, Point point1, Rectangle4 area, double m, int y) {
			int iX = CalculateX(point1, m, y);
			if (iX >= area.Left && iX <= area.Right) {
				point.X = iX;
				point.Y = y;
				return true;
			}
			return false;
		}
	}
}