using System;
using PowerLanguage;

namespace Zeghs.Data {
	internal sealed class _DOMInfo {
		private int __iDecimalPoint = 0;
		private DOMPrice __cAsk = new DOMPrice();
		private DOMPrice __cBid = new DOMPrice();

		public double AskPrice {
			get {
				return Math.Round(__cAsk.Price, __iDecimalPoint);
			}
		}
	
		public double AskSize {
			get {
				return __cAsk.Size;
			}
		}

		public double BidPrice {
			get {
				return Math.Round(__cBid.Price, __iDecimalPoint);
			}
		}

		public double BidSize {
			get {
				return __cBid.Size;
			}
		}

		public _DOMInfo(int decimalPoint) {
			__iDecimalPoint = decimalPoint;
		}

		internal void SetDOM(DOMPrice ask, DOMPrice bid) {
			__cAsk = ask;
			__cBid = bid;
		}
	}
}