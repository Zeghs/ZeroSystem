using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Zeghs.Data;

namespace Zeghs.Utils {
	internal sealed class JsonReport {
		internal static void Save(TradeBoundList openTrades, HistoryBoundList history, string file) {
			int iIndex = file.LastIndexOf(".");
			if (iIndex > -1) {
				string sOpenFile = file.Substring(0, iIndex) + "_open.json";
				string sCloseFile = file.Substring(0, iIndex) + "_close.json";

				File.WriteAllText(sOpenFile, ProcessOpenTrades(openTrades), Encoding.UTF8);
				File.WriteAllText(sCloseFile, ProcessHistorys(history), Encoding.UTF8);
			}
		}

		private static string ProcessHistorys(HistoryBoundList trades) {
			int iCount = trades.Count - 1;  //內建的歷史資料用來統計總和放在最後一筆
			if (iCount > 0) {
				_TradeInfo[] cItems = new _TradeInfo[iCount];
				for (int i = 0; i < iCount; i++) {
					cItems[i] = trades.GetItemAt(i);
				}
				
				return JsonConvert.SerializeObject(cItems, Formatting.Indented);
			}
			return "[]";
		}

		private static string ProcessOpenTrades(TradeBoundList trades) {
			int iCount = trades.Count;
			if (iCount > 0) {
				_TradeInfo[] cItems = new _TradeInfo[iCount];
				for (int i = 0; i < iCount; i++) {
					cItems[i] = trades.GetItemAt(i);
				}
				
				return JsonConvert.SerializeObject(cItems, Formatting.Indented);
			}
			return "[]";
		}
	}
}