using System;
using System.Collections.Generic;

namespace Zeghs.Data {
	public sealed class ForeignInvestmentGroup {
		private Dictionary<string, ForeignInvestmentData> __cForeigns = null;

		public ForeignInvestmentGroup() {
			__cForeigns = new Dictionary<string, ForeignInvestmentData>(8);
		}

		public ForeignInvestmentData GetForeignInvestment(string symbolId) {
			ForeignInvestmentData cData = null;
			__cForeigns.TryGetValue(symbolId, out cData);
			return cData;
		}

		internal void Add(string symbolId, ForeignInvestmentData data) {
			if (!__cForeigns.ContainsKey(symbolId)) {
				__cForeigns.Add(symbolId, data);
			}
		}
	}
}