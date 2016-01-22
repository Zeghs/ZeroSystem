using System;

namespace Netwings.Orders {
	public sealed class OrderDeal {
		public string 商品代號 {
			get;
			set;
		}

		public string 成交書號 {
			get;
			set;
		}

		public bool 買賣別 {
			get;
			set;
		}

		public int 成交數量 {
			get;
			set;
		}

		public double 成交價格 {
			get;
			set;
		}

		public DateTime 成交時間 {
			get;
			set;
		}

		public string 倉別 {
			get;
			set;
		}

		public double 手續費 {
			get;
			set;
		}

		public double 交易稅 {
			get;
			set;
		}
	}
}