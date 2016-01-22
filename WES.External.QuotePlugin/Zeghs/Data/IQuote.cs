using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Data {
	/// <summary>
	///   Quote報價資訊介面
	/// </summary>
	public interface IQuote {
		/// <summary>
		///   [取得] 今日收盤價
		/// </summary>
		double Close {
			get;
		}

		/// <summary>
		///   [取得] 即時資訊回補狀態
		/// </summary>
		ComplementStatus ComplementStatus {
			get;
		}

		/// <summary>
		///    [取得] 委買委賣價量表
		/// </summary>
		IDOMData DOM {
			get;
		}

		/// <summary>
		///   [取得] 今日最高價
		/// </summary>
		double High {
			get;
		}

		/// <summary>
		///   [取得] 今日最低價
		/// </summary>
		double Low {
			get;
		}

		/// <summary>
		///   [取得] 今日開盤價
		/// </summary>
		double Open {
			get;
		}

		/// <summary>
		///    [取得] 分價表
		/// </summary>
		List<IPriceClassify> Prices {
			get;
		}

		/// <summary>
		///   [取得] 昨收價(昨收價為今日參考價)
		/// </summary>
		double ReferPrice {
			get;
		}

		/// <summary>
		///   [取得] 目前最新即時報價
		/// </summary>
		ITick RealTick {
			get;
		}

		/// <summary>
		///   [取得] 商品代號
		/// </summary>
		string SymbolId {
			get;
		}

		/// <summary>
		///   [取得] 商品名稱
		/// </summary>
		string SymbolName {
			get;
		}

		/// <summary>
		///   [取得] 所有即時報價資訊的Tick Count
		/// </summary>
		int TickCount {
			get;
		}

		/// <summary>
		///   [取得/設定] 更新次數
		/// </summary>
		int UpdateCount {
			get;
			set;
		}

		/// <summary>
		///   取得即時報價Tick
		/// </summary>
		/// <param name="index">索引值(0=最新報價資訊)</param>
		/// <returns>返回值:ITick報價資訊</returns>
		ITick GetTick(int index);
	}
}