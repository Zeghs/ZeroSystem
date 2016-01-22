using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   市場交易倉位介面
	/// </summary>
	public interface IMarketPosition {
		/// <summary>
		///   [取得] 歷史交易明細
		/// </summary>
		List<ITrade> ClosedTrades {
			get;
		}

		/// <summary>
		///   [取得] 帳上最大虧損
		/// </summary>
		double MaxDrawDown {
			get;
		}

		/// <summary>
		///   [取得] 帳上最大獲利
		/// </summary>
		double MaxRunUp {
			get;
		}

		/// <summary>
		///   [取得] 開倉單總數量
		/// </summary>
		int OpenLots {
			get;
		}

		/// <summary>
		///   [取得] 開倉單總損益
		/// </summary>
		double OpenProfit {
			get;
		}

		/// <summary>
		///   [取得] 留倉交易明細
		/// </summary>
		List<ITrade> OpenTrades {
			get;
		}

		/// <summary>
		///   [取得] 已平倉的總損益
		/// </summary>
		double Profit {
			get;
		}

		/// <summary>
		///   [取得] 目前未平倉單的平均損益
		/// </summary>
		double ProfitPerContract {
			get;
		}

		/// <summary>
		///   [取得] 目前倉位的方向
		/// </summary>
		EMarketPositionSide Side {
			get;
		}

		/// <summary>
		///   [取得] 總下單數量
		/// </summary>
		int Value {
			get;
		}
	}
}