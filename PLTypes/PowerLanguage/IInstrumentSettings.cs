using System;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   商品設定資訊
	/// </summary>
	public interface IInstrumentSettings {
		/// <summary>
		///   [取得] 商品資訊(為了相容 PowerLanguage 但內部結構並不相同)
		/// </summary>
		Product ASymbolInfo2 {
			get;
		}

		/// <summary>
		///   [取得] 每一大點的金額
		/// </summary>
		double BigPointValue {
			get;
		}
		
		/// <summary>
		///   [取得] 商品分類列舉
		/// </summary>
		ESymbolCategory Category {
			get;
		}

		/// <summary>
		///   [取得] 當日漲跌幅限制
		/// </summary>
		double DailyLimit {
			get;
		}
		
		/// <summary>
		///   [取得] 資料來源名稱
		/// </summary>
		string DataFeed {
			get;
		}

		/// <summary>
		///   [取得] 商品資訊備註
		/// </summary>
		string Description {
			get;
		}
		
		/// <summary>
		///   [取得] 交易所簡稱
		/// </summary>
		string Exchange {
			get;
		}

		/// <summary>
		///   [取得] 到期日(如果是期貨與選擇權為到期日, 如果是股票則是交割時間截止日期)
		/// </summary>
		DateTime Expiration {
			get;
		}

		/// <summary>
		///   [取得] 期貨或選擇權使用之保證金
		/// </summary>
		double Margin {
			get;
		}

		/// <summary>
		///   [取得] 最小跳動點數
		/// </summary>
		double MinMove {
			get;
		}

		/// <summary>
		///   [取得] 商品名稱
		/// </summary>
		string Name {
			get;
		}

		/// <summary>
		///   [取得] 選擇權的買賣權類型(None=無使用, 可能不是選擇權)
		/// </summary>
		OptionType OptionType {
			get;
		}

		/// <summary>
		///   [取得] 最小跳動金額
		/// </summary>
		double PointValue {
			get;
		}

		/// <summary>
		///   [取得] 價格座標
		/// </summary>
		double PriceScale {
			get;
		}

		/// <summary>
		///   [取得] Bars 時間週期
		/// </summary>
		Resolution Resolution {
			get;
		}

		/// <summary>
		///   [取得] 商品開收盤時間
		/// </summary>
		List<SessionObject> Sessions {
			get;
		}

		/// <summary>
		///   [取得] 選擇權的履約價格(若不是選擇權, 此屬性皆為 0)
		/// </summary>
		double StrikePrice {
			get;
		}
	}
}