using System.Drawing;
using PowerLanguage;

namespace Zeghs.Drawing {
	public sealed class PlotObject<T> : IPlotObject<T> {
		private VariableSeries<T> __cValues = null;
		private VariableSeries<PenStyle[]> __cPenStyles = null;

		/// <summary>
		///   [取得] 背景顏色
		/// </summary>
		public Color BGColor {
			get;
			set;
		}

		/// <summary>
		///   [取得] 繪製名稱
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		///   [取得] 繪製畫筆樣式資料
		/// </summary>
		public VariableSeries<PenStyle[]> PenStyles {
			get {
				return __cPenStyles;
			}
		}

		/// <summary>
		///   [取得] 繪製數值資料
		/// </summary>
		public VariableSeries<T> Value {
			get {
				return __cValues;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">IStudyControl 腳本控制介面</param>
		/// <param name="data_stream">資料串流編號(從 1 開始編號)</param>
		public PlotObject(IStudyControl master, int data_stream = 1) {
			__cValues = new VariableSeries<T>(master, default(T), data_stream);
			__cPenStyles = new VariableSeries<PenStyle[]>(master, null, data_stream);
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		public void Set(T val) {
			__cValues.Value = val;
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		public void Set(T val, Color color) {
			this.Set(val, new Color[] { color }, new int[] { 1 });
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		/// <param name="width">繪製寬度</param>
		public void Set(T val, Color color, int width) {
			this.Set(val, new Color[] { color }, new int[] { width });
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="colors">繪製顏色陣列</param>
		/// <param name="widths">繪製寬度陣列</param>
		public void Set(T val, Color[] colors, int[] widths) {
			int iLength = colors.Length;
			PenStyle[] cPenStyles = new PenStyle[iLength];
			for (int i = 0; i < iLength; i++) {
				cPenStyles[i] = new PenStyle(colors[i], widths[i]);
			}

			__cValues.Value = val;
			__cPenStyles.Value = cPenStyles;
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		public void Set(int barsAgo, T val) {
			int iCurrent = __cValues.Current;
			__cValues.Move(iCurrent - barsAgo - 1);
			__cValues.Value = val;
			__cValues.Current = iCurrent;
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		public void Set(int barsAgo, T val, Color color) {
			this.Set(barsAgo, val, new Color[] { color }, null);
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		/// <param name="width">繪製寬度</param>
		public void Set(int barsAgo, T val, Color color, int width) {
			this.Set(barsAgo, val, new Color[] { color }, new int[] { width });
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="colors">繪製顏色</param>
		/// <param name="widths">繪製寬度</param>
		public void Set(int barsAgo, T val, Color[] colors, int[] widths) {
			int iLength = colors.Length;
			PenStyle[] cPenStyles = new PenStyle[iLength];
			for (int i = 0; i < iLength; i++) {
				cPenStyles[i] = new PenStyle(colors[i], widths[i]);
			}

			int iCurrent = __cValues.Current;
			__cValues.Move(iCurrent - barsAgo - 1);
			__cValues.Value = val;
			__cValues.Current = iCurrent;

			iCurrent = __cPenStyles.Current;
			__cPenStyles.Move(iCurrent - barsAgo - 1);
			__cPenStyles.Value = cPenStyles;
			__cPenStyles.Current = iCurrent;
		}
	}
}