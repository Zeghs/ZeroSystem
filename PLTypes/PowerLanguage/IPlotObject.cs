using System.Drawing;

namespace PowerLanguage {
	/// <summary>
	///   Plot 繪製物件
	/// </summary>
	public interface IPlotObject<T> {
		/// <summary>
		///   [取得] 背景顏色
		/// </summary>
		Color BGColor {
			get;
		}

		/// <summary>
		///   [取得] 繪製名稱
		/// </summary>
		string Name {
			get;
		}

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		void Set(T val);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		void Set(T val, Color color);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		/// <param name="width">繪製寬度</param>
		void Set(T val, Color color, int width);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="val">繪製資料數值</param>
		/// <param name="colors">繪製顏色陣列</param>
		/// <param name="widths">繪製寬度陣列</param>
		void Set(T val, Color[] colors, int[] widths);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		void Set(int barsAgo, T val);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		void Set(int barsAgo, T val, Color color);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="color">繪製顏色</param>
		/// <param name="width">繪製寬度</param>
		void Set(int barsAgo, T val, Color color, int width);

		/// <summary>
		///   設定繪製資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <param name="val">繪製資料數值</param>
		/// <param name="colors">繪製顏色</param>
		/// <param name="widths">繪製寬度</param>
		void Set(int barsAgo, T val, Color[] colors, int[] widths);
	}
}