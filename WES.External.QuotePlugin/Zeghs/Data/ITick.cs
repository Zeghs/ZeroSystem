using System;
using PowerLanguage;

namespace Zeghs.Data {
	/// <summary>
	///   即時成交報價資訊介面
	/// </summary>
	public interface ITick {
		/// <summary>
		///   [取得] 委賣資訊
		/// </summary>
		DOMPrice Ask {
			get;
		}

		/// <summary>
		///   [取得] 委買資訊
		/// </summary>
		DOMPrice Bid {
			get;
		}

		/// <summary>
		///   [取得] 報價Tick的成交價格
		/// </summary>
		double Price {
			get;
		}

		/// <summary>
		///   [取得] 報價Tick的成交單量
		/// </summary>
		double Single {
			get;
		}

		/// <summary>
		///   [取得] 報價Tick的成交時間
		/// </summary>
		DateTime Time {
			get;
		}
		
		/// <summary>
		///   [取得] 報價Tick的成交總量
		/// </summary>
		double Volume {
			get;
		}
	}
}