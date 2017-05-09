using System.Collections.Generic;
using Zeghs.Chart;

namespace Zeghs.Actions {
	/// <summary>
	///   使用者繪製物件介面
	/// </summary>
	public interface IDrawable {
		/// <summary>
		///   繪製使用者建立的繪圖物件
		/// </summary>
		/// <param name="layer">Layer 圖層</param>
		/// <param name="item">使用者繪製物件</param>
		void DrawObject(Layer layer, DrawObject item);
	}
}