using System.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   Plot屬性類別
	/// </summary>
	public sealed class PlotAttributes {
		private static int __iPlotNumber = 0;

		private int __iPlotNum = 0;
		private int __iLayerIndex = 0;
		private bool __bShowLastPrice = false;
		private bool __bUseMasterAxisY = true;
		private string __sName = string.Empty;
		private Color __cBgColor = Color.Black;
		private EPlotShapes __cPlotShape = EPlotShapes.Line;
		private PenStyle[] __cPenStyles = new PenStyle[] { new PenStyle(Color.White, 1) };

		/// <summary>
		///   [取得] 背景顏色
		/// </summary>
		public Color BackgroundColor {
			get {
				return __cBgColor;
			}
		}

		/// <summary>
		///   [取得] 圖表Layer索引(0=預設主圖索引, -1=新建圖層)
		/// </summary>
		public int LayerIndex {
			get {
				return __iLayerIndex;
			}
		}

		/// <summary>
		///   [取得] Plot名稱
		/// </summary>
		public string Name {
			get {
				return __sName;
			}
		}

		/// <summary>
		///   [取得] 畫筆樣式陣列
		/// </summary>
		public PenStyle[] PenStyles {
			get {
				return __cPenStyles;
			}
		}

		/// <summary>
		///   [取得] Plot形狀列舉
		/// </summary>
		public EPlotShapes PlotSharp {
			get {
				return __cPlotShape;
			}
		}

		/// <summary>
		///   [取得] 是否顯示最後價格
		/// </summary>
		public bool ShowLastPrice {
			get {
				return __bShowLastPrice;
			}
		}

		/// <summary>
		///   [取得] 使用主圖座標Y軸作為參考(false=具有獨立座標Y軸刻度參考, true=使用主圖座標Y軸作參考)
		/// </summary>
		public bool UseMasterAxisY {
			get {
				return __bUseMasterAxisY;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="plotNum">Plot編號</param>
		public PlotAttributes(int plotNum) {
			__iPlotNum = plotNum;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		public PlotAttributes(string name) {
			__sName = name;
			__iPlotNum = __iPlotNumber++;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="layerIndex">圖表Layer索引(0=預設主圖索引, -1=新建圖層)</param>
		/// <param name="useMasterAxisY">使用主圖座標Y軸作為參考(false=具有獨立座標Y軸刻度參考, true=使用主圖座標Y軸作參考)</param>
		public PlotAttributes(string name, int layerIndex, bool useMasterAxisY = true) {
			__sName = name;
			__iPlotNum = __iPlotNumber++;
			__iLayerIndex = layerIndex;
			__bUseMasterAxisY = useMasterAxisY;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="plotNum">Plot編號</param>
		public PlotAttributes(string name, int plotNum) {
			__sName = name;
			__iPlotNum = plotNum;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="fgColor">前景顏色</param>
		public PlotAttributes(string name, EPlotShapes type, Color fgColor) 
			: this(name, type, fgColor, 0, true) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="fgColor">前景顏色</param>
		/// <param name="layerIndex">圖表Layer索引(0=預設主圖索引, -1=新建圖層)</param>
		public PlotAttributes(string name, EPlotShapes type, Color fgColor, int layerIndex)
			: this(name, type, fgColor, layerIndex, true) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="fgColor">前景顏色</param>
		/// <param name="layerIndex">圖表Layer索引(0=預設主圖索引, -1=新建圖層)</param>
		/// <param name="useMasterAxisY">使用主圖座標Y軸作為參考(false=具有獨立座標Y軸刻度參考, true=使用主圖座標Y軸作參考)</param>
		public PlotAttributes(string name, EPlotShapes type, Color fgColor, int layerIndex, bool useMasterAxisY) {
			__sName = name;
			__cPlotShape = type;
			__iPlotNum = __iPlotNumber++;
			__cPenStyles[0].Color = fgColor;

			__iLayerIndex = layerIndex;
			__bUseMasterAxisY = useMasterAxisY;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="fgColor">前景顏色</param>
		/// <param name="bgColor">背景顏色</param>
		/// <param name="width">線條寬度</param>
		/// <param name="style">線條樣式</param>
		/// <param name="showLastPrice">是否顯示最後價格</param>
		public PlotAttributes(string name, EPlotShapes type, Color fgColor, Color bgColor, int width, int style, bool showLastPrice)
			: this(name, type, fgColor, bgColor, width, style, showLastPrice, 0, true) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="fgColor">前景顏色</param>
		/// <param name="bgColor">背景顏色</param>
		/// <param name="width">線條寬度</param>
		/// <param name="style">線條樣式</param>
		/// <param name="showLastPrice">是否顯示最後價格</param>
		/// <param name="layerIndex">圖表Layer索引(0=預設主圖索引, -1=新建圖層)</param>
		/// <param name="useMasterAxisY">使用主圖座標Y軸作為參考(false=具有獨立座標Y軸刻度參考, true=使用主圖座標Y軸作參考)</param>
		public PlotAttributes(string name, EPlotShapes type, Color fgColor, Color bgColor, int width, int style, bool showLastPrice, int layerIndex, bool useMasterAxisY) {
			__sName = name;
			__cPlotShape = type;
			__cBgColor = bgColor;
			__iPlotNum = __iPlotNumber++;
			__bShowLastPrice = showLastPrice;
			__cPenStyles[0] = new PenStyle(fgColor, width, style);

			__iLayerIndex = layerIndex;
			__bUseMasterAxisY = useMasterAxisY;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="bgColor">背景顏色</param>
		/// <param name="fgColors">前景顏色陣列</param>
		/// <param name="widths">線條寬度陣列</param>
		/// <param name="styles">線條樣式陣列</param>
		/// <param name="showLastPrice">是否顯示最後價格</param>
		public PlotAttributes(string name, EPlotShapes type, Color bgColor, Color[] fgColors, int[] widths, int[] styles, bool showLastPrice) 
			: this(name, type, bgColor, fgColors, widths, styles, showLastPrice, 0, true) {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="name">Plot名稱</param>
		/// <param name="type">Plot形狀</param>
		/// <param name="bgColor">背景顏色</param>
		/// <param name="fgColors">前景顏色陣列</param>
		/// <param name="widths">線條寬度陣列</param>
		/// <param name="styles">線條樣式陣列</param>
		/// <param name="showLastPrice">是否顯示最後價格</param>
		/// <param name="layerIndex">圖表Layer索引(0=預設主圖索引, -1=新建圖層)</param>
		/// <param name="useMasterAxisY">使用主圖座標Y軸作為參考(false=具有獨立座標Y軸刻度參考, true=使用主圖座標Y軸作參考)</param>
		public PlotAttributes(string name, EPlotShapes type, Color bgColor, Color[] fgColors, int[] widths, int[] styles, bool showLastPrice, int layerIndex, bool useMasterAxisY) {
			__sName = name;
			__cPlotShape = type;
			__cBgColor = bgColor;
			__iPlotNum = __iPlotNumber++;
			__bShowLastPrice = showLastPrice;

			int iLength = fgColors.Length;
			__cPenStyles = new PenStyle[iLength];
			for (int i = 0; i < iLength; i++) {
				__cPenStyles[i] = new PenStyle(fgColors[i], widths[i], styles[i]);
			}

			__iLayerIndex = layerIndex;
			__bUseMasterAxisY = useMasterAxisY;
		}
	}
}