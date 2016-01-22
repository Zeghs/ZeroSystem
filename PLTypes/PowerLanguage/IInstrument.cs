using System;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///    商品資訊介面(包含開高低收資訊與商品屬性及其他重要資訊)
	/// </summary>
	public interface IInstrument : ISeriesSymbolData {
		/// <summary>
		///   [取得] Bars 最新的更新時間
		/// </summary>
		DateTime BarUpdateTime {
			get;
		}

		/// <summary>
		///   [取得] 目前 Bars 索引值
		/// </summary>
		int CurrentBar {
			get;
		}

		/// <summary>
		///   [取得] 委託價量資訊
		/// </summary>
		IDOMData DOM {
			get;
		}

		/// <summary>
		///   [取得] 商品的所有資料
		/// </summary>
		ISeriesSymbolDataRand FullSymbolData {
			get;
		}

		/// <summary>
		///   [取得] 商品屬性
		/// </summary>
		IInstrumentSettings Info {
			get;
		}

		/// <summary>
		///   [取得] 最後一根 Bars 的時間
		/// </summary>
		DateTime LastBarTime {
			get;
		}

		/// <summary>
		///   [取得] 資料請求資訊
		/// </summary>
		InstrumentDataRequest Request {
			get;
		}

		/// <summary>
		///   [取得] 開收盤的時間週期
		/// </summary>
		List<SessionObject> Sessions {
			get;
		}

		/// <summary>
		///   [取得] Bars 狀態
		/// </summary>
		EBarState Status {
			get;
		}
	}
}