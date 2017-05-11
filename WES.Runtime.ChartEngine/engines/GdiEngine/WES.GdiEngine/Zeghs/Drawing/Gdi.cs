using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zeghs.Drawing {
	internal sealed class Gdi : IDisposable {
		private const int TRANSPARENT = 1;

		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BitBlt(IntPtr hDC, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
		private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, uint crColor);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateSolidBrush(uint crColor);

		[DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
		private static extern bool DeleteDC(IntPtr hDC);

		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll", EntryPoint = "ExtTextOutW")]
		private static extern bool ExtTextOut(IntPtr hdc, int X, int Y, ETOOptions fuOptions, [In] ref Rectangle4 lprc, [MarshalAs(UnmanagedType.LPWStr)] string lpString, uint cbCount, [In] IntPtr lpDx);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("gdi32.dll")]
		private static extern IntPtr GetStockObject(StockObjects fnObject);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		private static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

		[DllImport("gdi32.dll")]
		private static extern int IntersectClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[DllImport("gdi32.dll")]
		private static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

		[DllImport("gdi32.dll")]
		private static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

		[DllImport("gdi32.dll")]
		private static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		private static extern bool RestoreDC(IntPtr hdc, int nSavedDC);
		
		[DllImport("gdi32.dll")]
		private static extern int SaveDC(IntPtr hdc);
		
		[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
		private static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		private static extern int SetBkColor(IntPtr hdc, int crColor);

		[DllImport("gdi32.dll")]
		private static extern int SetBkMode(IntPtr hdc, int iBkMode);

		[DllImport("gdi32.dll")]
		private static extern int SetROP2(IntPtr hdc, int fnDrawMode);

		[DllImport("gdi32.dll")]
		private static extern int SetTextColor(IntPtr hdc, int crColor);
		
		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		private static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

		internal static IntPtr CreatePen(PowerLanguage.PenStyle pen) {
			PenStyle cStyle = PenStyle.PS_NULL;
			uint uMask = (uint) pen.Pattern;
			switch (uMask) {
				case 0xeeeeeeee:
					cStyle = PenStyle.PS_DASH;
					break;
				case 0xebaebaeb:
					cStyle = PenStyle.PS_DASHDOT;
					break;
				case 0xeaeaeaea:
					cStyle = PenStyle.PS_DASHDOTDOT;
					break;
				case 0xaaaaaaaa:
					cStyle = PenStyle.PS_DOT;
					break;
				default:
					cStyle = PenStyle.PS_SOLID;
					break;
			}
			return CreatePen(cStyle, (int) pen.Width, (uint) ColorTranslator.ToWin32(pen.Color));
		}

		private bool __bSaveRop = false;
		private bool __bDisposed = false;
		private IntPtr __cHDC = IntPtr.Zero;
		private IntPtr __cHandle = IntPtr.Zero;
		private List<_TextInfo> __cTextInfos = null;
		private List<_LineInfo> __cLineInfos = null;

		internal Gdi(IntPtr handle) {
			__cHandle = handle;
			__cHDC = GetDC(__cHandle);
			
			__cLineInfos = new List<_LineInfo>(64);
			__cTextInfos = new List<_TextInfo>(64);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void BeginRopDraw() {
			__bSaveRop = true;
		}

		internal void ClearRops(bool isRecovery) {
			ClearRops(__cLineInfos, __cTextInfos, isRecovery);
		}

		internal void ClearRops(List<_LineInfo> lines, List<_TextInfo> texts, bool isRecovery) {
			if (lines != null) {
				int iCount = lines.Count;
				if (iCount > 0) {
					if (isRecovery) {
						for (int i = 0; i < iCount; i++) {
							ClearRopLines(lines[i]);
						}
					}

					for (int i = 0; i < iCount; i++) {
						RemoveObject(lines[i].Pen);
					}
					lines.Clear();
				}
			}

			if (texts != null) {
				int iCount = texts.Count;
				if (iCount > 0) {
					for (int i = 0; i < iCount; i++) {
						ClearRopString(texts[i], isRecovery);
					}
					texts.Clear();
				}
			}
		}

		internal void ClearTransparent(int mode) {
			SetBkMode(__cHDC, mode);
		}

		internal void ClipRectangle(Rectangle rectangle) {
			IntersectClipRect(__cHDC, rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
		}

		internal void DrawLine(int x1, int y1, int x2, int y2) {
			DrawLines(new Point[] { new Point(x1, y1), new Point(x2, y2) });
		}

		internal _LineInfo DrawRopLine(IntPtr pen, int x1, int y1, int x2, int y2) {
			return DrawRopLines(pen, new Point[] { new Point(x1, y1), new Point(x2, y2) });
		}

		internal void DrawLines(Point[] points) {
			int iLength = points.Length;
			for (int i = 1; i < iLength; i++) {
				Point cStart = points[i - 1];
				Point cEnd = points[i];

				MoveToEx(__cHDC, cStart.X, cStart.Y, IntPtr.Zero);
				LineTo(__cHDC, cEnd.X, cEnd.Y);
			}
		}

		internal _LineInfo DrawRopLines(IntPtr pen, Point[] points) {
			int iROP = SetROP(BinaryRasterOperations.R2_XORPEN);
			IntPtr cOldPen = SelectPen(pen);

			int iLength = points.Length;
			for (int i = 1; i < iLength; i++) {
				Point cStart = points[i - 1];
				Point cEnd = points[i];

				MoveToEx(__cHDC, cStart.X, cStart.Y, IntPtr.Zero);
				LineTo(__cHDC, cEnd.X, cEnd.Y);
			}

			SelectPen(cOldPen);
			SetROP(iROP);
			
			_LineInfo cLineInfo = new _LineInfo(pen, points);
			if (__bSaveRop) {
				__cLineInfos.Add(cLineInfo);
			}
			return cLineInfo;
		}

		internal void DrawRectangle(int x, int y, int width, int height) {
			IntPtr cStockObject = SelectObject(__cHDC, GetStockObject(StockObjects.NULL_BRUSH));
			Rectangle(__cHDC, x, y, x + width, y + height);
			SelectObject(__cHDC, cStockObject);
		}

		internal void DrawString(string text, Color color, int x, int y) {
			SetTextColor(__cHDC, ColorTranslator.ToWin32(color));
			TextOut(__cHDC, x, y, text, text.Length);
		}

		internal _TextInfo DrawRopString(string text, Color color, int x, int y) {
			Size cSize = this.MeasureString(text);

			IntPtr hMemDC = CreateCompatibleDC(__cHDC);
			IntPtr hBitmap = CreateCompatibleBitmap(__cHDC, cSize.Width, cSize.Height);
			IntPtr cOldObject = SelectObject(hMemDC, hBitmap);

			BitBlt(hMemDC, 0, 0, cSize.Width, cSize.Height, __cHDC, x, y, TernaryRasterOperations.SRCCOPY);

			int iOldColor = SetTextColor(__cHDC, ColorTranslator.ToWin32(color));
			TextOut(__cHDC, x, y, text, text.Length);

			_TextInfo cTextInfo = new _TextInfo(new IntPtr[] { hMemDC, hBitmap, cOldObject }, x, y, cSize.Width, cSize.Height);
			if (__bSaveRop) {
				__cTextInfos.Add(cTextInfo);
			}
			return cTextInfo;
		}

		internal _TextInfo DrawRopString(string text, Color foreColor, Color background, int x, int y,Rectangle rect) {
			IntPtr hMemDC = CreateCompatibleDC(__cHDC);
			IntPtr hBitmap = CreateCompatibleBitmap(__cHDC, rect.Width, rect.Height);
			IntPtr cOldObject = SelectObject(hMemDC, hBitmap);

			BitBlt(hMemDC, 0, 0, rect.Width, rect.Height, __cHDC, rect.X, rect.Y, TernaryRasterOperations.SRCCOPY);

			int iOldColor = SetTextColor(__cHDC, ColorTranslator.ToWin32(foreColor));
			int iOldBackColor = SetBkColor(__cHDC, ColorTranslator.ToWin32(background));
			Rectangle4 cRect4 = new Rectangle4(rect);
			ExtTextOut(__cHDC, 0, 0, ETOOptions.ETO_OPAQUE, ref cRect4, string.Empty, 0, IntPtr.Zero);
			TextOut(__cHDC, rect.X + x, rect.Y + y, text, text.Length);
			SetBkColor(__cHDC, iOldBackColor);

			_TextInfo cTextInfo = new _TextInfo(new IntPtr[] { hMemDC, hBitmap, cOldObject }, rect.X, rect.Y, rect.Width, rect.Height);
			if (__bSaveRop) {
				__cTextInfos.Add(cTextInfo);
			}
			return cTextInfo;
		}

		internal void EndRopDraw() {
			__bSaveRop = false;
		}

		internal void FillRectangle(Color color, int x, int y, int width, int height) {
			int iOldColor = SetBkColor(__cHDC, ColorTranslator.ToWin32(color));
			Rectangle4 cRect = new Rectangle4(x, y, x + width, y + height);
			ExtTextOut(__cHDC, 0, 0, ETOOptions.ETO_OPAQUE, ref cRect, string.Empty, 0, IntPtr.Zero);
			SetBkColor(__cHDC, iOldColor);
		}

		internal Size MeasureString(string text) {
			Size sz = Size.Empty;
			GetTextExtentPoint32(__cHDC, text, text.Length, out sz);
			return sz;
		}

		internal bool RemoveObject(IntPtr obj) {
			return DeleteObject(obj);
		}

		internal void RestoreDC() {
			RestoreDC(__cHDC, -1);
		}

		internal void SaveDC() {
			SaveDC(__cHDC);
		}

		internal int SelectBackground(int color) {
			return SetBkColor(__cHDC, color);
		}

		internal int SelectBackground(Color color) {
			return SetBkColor(__cHDC, ColorTranslator.ToWin32(color));
		}

		internal IntPtr SelectFont(IntPtr font) {
			return SelectObject(__cHDC, font);
		}

		internal IntPtr SelectFont(Font font) {
			return SelectObject(__cHDC, font.ToHfont());
		}

		internal IntPtr SelectPen(IntPtr pen) {
			return SelectObject(__cHDC, pen);
		}

		internal IntPtr SelectPen(PowerLanguage.PenStyle pen) {
			return SelectObject(__cHDC, CreatePen(pen));
		}

		internal int SelectTransparent() {
			return SetBkMode(__cHDC, TRANSPARENT);
		}

		internal int SetROP(int fnDrawMode) {
			return SetROP2(__cHDC, fnDrawMode);
		}

		internal int SetROP(BinaryRasterOperations fnDrawMode) {
			return SetROP((int) fnDrawMode);
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					ClearRops(false);
					ReleaseDC(__cHandle, __cHDC);
				}
			}
		}

		private void ClearRopLines(_LineInfo lineInfo) {
			int iROP = SetROP(BinaryRasterOperations.R2_XORPEN);
			IntPtr cOldPen = SelectPen(lineInfo.Pen);

			Point[] cPoints = lineInfo.Points;
			int iLength = cPoints.Length;
			for (int i = 1; i < iLength; i++) {
				Point cStart = cPoints[i - 1];
				Point cEnd = cPoints[i];

				MoveToEx(__cHDC, cStart.X, cStart.Y, IntPtr.Zero);
				LineTo(__cHDC, cEnd.X, cEnd.Y);
			}

			SelectPen(cOldPen);
			SetROP(iROP);
		}

		private void ClearRopString(_TextInfo textInfo, bool isRecovery) {
			IntPtr[] cHDCs = textInfo.hDCs;
			Rectangle cRect = textInfo.Rectangle;
			IntPtr hMemDC = cHDCs[0];
			IntPtr hBitmap = cHDCs[1];
			IntPtr cOldObject = cHDCs[2];
			if (isRecovery) {
				BitBlt(__cHDC, cRect.X, cRect.Y, cRect.Width, cRect.Height, hMemDC, 0, 0, TernaryRasterOperations.SRCCOPY);
			}

			SelectObject(hMemDC, cOldObject);
			DeleteObject(hBitmap);
			DeleteDC(hMemDC);
		}
	}
}