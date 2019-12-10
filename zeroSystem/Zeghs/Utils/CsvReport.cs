using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Zeghs.Data;

namespace Zeghs.Utils {
	internal sealed class CsvReport {
		internal static void Save(TradeBoundList openTrades, HistoryBoundList history, string file) {
			int iIndex = file.LastIndexOf(".");
			if (iIndex > -1) {
				string sOpenFile = file.Substring(0, iIndex) + "_open.csv";
				string sCloseFile = file.Substring(0, iIndex) + "_close.csv";

				File.WriteAllText(sOpenFile, ProcessOpenTrades(openTrades), Encoding.UTF8);
				File.WriteAllText(sCloseFile, ProcessHistorys(history), Encoding.UTF8);
			}
		}

		private static string ProcessHistorys(HistoryBoundList trades) {
			StringBuilder cBuilder = new StringBuilder(1024 * 1024);
			cBuilder.Append("NO.").Append(',').Append("SymbolID").Append(',').Append("Category").Append(',').Append("Action").Append(',').Append("Volume").Append(',').Append("Price").Append(',').Append("Profit").Append(',').Append("Fee").Append(',').Append("Tax").Append(',').Append("Trading time").Append(',').AppendLine("Description");

			int iCount = trades.Count - 1;  //內建的歷史資料用來統計總和放在最後一筆
			for (int i = 0; i < iCount; i++) {
				_TradeInfo cTrade = trades.GetItemAt(i);
				cBuilder.Append(cTrade.Ticket).Append(',').Append(cTrade.SymbolId).Append(',').Append(cTrade.Category).Append(',').Append(cTrade.Action).Append(',').Append(cTrade.Contracts).Append(',').Append(cTrade.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cTrade.Fee).Append(',').Append(cTrade.Tax).Append(',').Append(cTrade.Time).Append(',').AppendLine(cTrade.Comment);
			}
			return cBuilder.ToString();
		}

		private static string ProcessOpenTrades(TradeBoundList trades) {
			StringBuilder cBuilder = new StringBuilder(1024 * 1024);
			cBuilder.Append("NO.").Append(',').Append("SymbolID").Append(',').Append("Category").Append(',').Append("Action").Append(',').Append("Volume").Append(',').Append("Price").Append(',').Append("Profit").Append(',').Append("Fee").Append(',').Append("Tax").Append(',').Append("Trading time").Append(',').AppendLine("Description");

			int iCount = trades.Count - 1;  //開倉資料用來小計總合放在最後一筆
			for (int i = 0; i < iCount; i++) {
				_TradeInfo cTrade = trades.GetItemAt(i);
				cBuilder.Append(cTrade.Ticket).Append(',').Append(cTrade.SymbolId).Append(',').Append(cTrade.Category).Append(',').Append(cTrade.Action).Append(',').Append(cTrade.Contracts).Append(',').Append(cTrade.Price).Append(',').Append(cTrade.Profit).Append(',').Append(cTrade.Fee).Append(',').Append(cTrade.Tax).Append(',').Append(cTrade.Time).Append(',').AppendLine(cTrade.Comment);
			}
			return cBuilder.ToString();
		}
	}
}