using System;
using PowerLanguage;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    個股即時成交資訊
        /// </summary>
        public sealed class MitakeQuoteTick : ITick {
		/// <summary>
		///   [取得] 報價Tick成交時委賣資訊
		/// </summary>
		public DOMPrice Ask {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價Tick成交時委買資訊
		/// </summary>
		public DOMPrice Bid {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價Tick的成交價格
		/// </summary>
		public double Price {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價Tick的成交單量
		/// </summary>
		public double Single {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價Tick的成交時間
		/// </summary>
		public DateTime Time {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 報價Tick的成交總量
		/// </summary>
		public double Volume {
			get;
			internal set;
		}

		/// <summary>
		///    [取得] 報價Tick類型:0=即時, 1=補價
		/// </summary>
		public byte 類型 {
			get;
			internal set;
		}
		
		/// <summary>
		///   [取得] 價格類型:0=一般價, 1=開盤價, 2=最高價, 3=最低價, 4=委買價, 5=委賣價
		/// </summary>
		public byte 價格類型 {
			get;
			internal set;
		}
		
		/// <summary>
		///   [取得] 買賣盤:0=無法區分買賣盤, 1=買盤, 2=賣盤
		/// </summary>
		public byte 買賣盤 {
			get;
			internal set;
		}

		/// <summary>
		///    [取得/設定] 報價Tick的序號(序號皆為唯一值)
		/// </summary>
		internal int Serial {
			get;
			set;
		}
	}
}