using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Orders;

namespace Zeghs.Utils {
	/// <summary>
	///   產生交易報表類別
	/// </summary>
	public sealed class TradeReport {
		/// <summary>
		///   儲存交易報表(CSV格式)
		/// </summary>
		/// <param name="filename">欲儲存的檔案</param>
		/// <param name="symbolId">商品名稱</param>
		/// <param name="positions">商品交易的倉位資訊</param>
		public static void Save(string filename, string symbolId, ISeries<IMarketPosition> positions) {
			if (File.Exists(filename)) {
				File.Delete(filename);
			}

			PositionSeries cPositions = positions as PositionSeries;
			StringBuilder cBuilder = new StringBuilder(1024 * 1024);
			cBuilder.Append("NO.").Append(',').Append("SymbolID").Append(',').Append("Category").Append(',').Append("Action").Append(',').Append("Volume").Append(',').Append("Price").Append(',').Append("Profit").Append(',').Append("Fee").Append(',').Append("Tax").Append(',').Append("Trading time").Append(',').AppendLine("Description");

			int iCount = cPositions.Count;
			for (int i = 0; i < iCount; i++) {
				IMarketPosition cPosition = cPositions[i];
				if (cPosition.Value > 0) {
					List<ITrade> cTrades = cPosition.ClosedTrades;
					foreach (ITrade cTrade in cTrades) {
						ITradeOrder cOpenO = cTrade.EntryOrder;
						ITradeOrder cCloseO = cTrade.ExitOrder;

						cBuilder.Append(cOpenO.Ticket).Append(',').Append(symbolId).Append(',').Append(cOpenO.Category).Append(',').Append(cOpenO.Action).Append(',').Append(cOpenO.Contracts).Append(',').Append(cOpenO.Price).Append(',').Append(string.Empty).Append(',').Append(cOpenO.Fee).Append(',').Append(cOpenO.Tax).Append(',').Append(cOpenO.Time.ToString("yyyy/MM/dd HH:mm:ss")).Append(',').AppendLine(cOpenO.Name);
						cBuilder.Append(cCloseO.Ticket).Append(',').Append(symbolId).Append(',').Append(cCloseO.Category).Append(',').Append(cCloseO.Action).Append(',').Append(cCloseO.Contracts).Append(',').Append(cCloseO.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cCloseO.Fee).Append(',').Append(cCloseO.Tax).Append(',').Append(cCloseO.Time.ToString("yyyy/MM/dd HH:mm:ss")).Append(',').AppendLine(cCloseO.Name);
					}
				}
			}
			File.WriteAllText(filename, cBuilder.ToString(), Encoding.UTF8);
		}
	}
}