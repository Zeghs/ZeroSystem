using System;

namespace Zeghs.Events {
	internal sealed class DataAdapterCompleteEvent : EventArgs {
		private int __iTotalSeconds = 0;
		private string __sSymbolId = null;

		internal string SymbolId {
			get {
				return __sSymbolId;
			}
		}

		internal int TotalSeconds {
			get {
				return __iTotalSeconds;
			}
		}

		internal DataAdapterCompleteEvent(string symbolId, int totalSeconds) {
			__sSymbolId = symbolId;
			__iTotalSeconds = totalSeconds;
		}
	}
}