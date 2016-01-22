using System;

namespace Netwings.Orders {
	public sealed class OrderTrust {
		public string 商品代號 {
			get;
			set;
		}

		public string 委託書號 {
			get;
			set;
		}

		public bool 買賣別 {
			get;
			set;
		}

		public int 未成交數量 {
			get;
			set;
		}

		public int 成交數量 {
			get;
			set;
		}

		public DateTime 委託時間 {
			get;
			set;
		}

		public double 委託價格 {
			get;
			set;
		}

		public string 倉別 {
			get;
			set;
		}

		public bool 是否刪單 {
			get;
			set;
		}

		public string 備註 {
			get;
			set;
		}
	}
}