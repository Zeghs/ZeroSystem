using System;
using PowerLanguage;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    指數即時資訊類別
        /// </summary>
        public sealed class MitakeIndexTick : ITick {
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
			get {
				return this.Classifys[0].IndexValue; //0=上市櫃大盤指數
			}
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
		///   [取得] 成交張數
		/// </summary>
		public uint 成交張數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 成交筆數
		/// </summary>
		public uint 成交筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 成交張數單量
		/// </summary>
		public uint 成交張數單量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 成交筆數單量
		/// </summary>
		public uint 成交筆數單量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委買合計筆數
		/// </summary>
		public uint 委買合計筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委賣合計筆數
		/// </summary>
		public uint 委賣合計筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委買總漲停張數
		/// </summary>
		public uint 委買總漲停張數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委賣總漲停張數
		/// </summary>
		public uint 委賣總漲停張數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委買總漲停筆數
		/// </summary>
		public uint 委買總漲停筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委賣總漲停筆數
		/// </summary>
		public uint 委賣總漲停筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委買總跌停張數
		/// </summary>
		public uint 委買總跌停張數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委賣總跌停張數
		/// </summary>
		public uint 委賣總跌停張數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委買總跌停筆數
		/// </summary>
		public uint 委買總跌停筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 委賣總跌停筆數
		/// </summary>
		public uint 委賣總跌停筆數 {
			get;
			internal set;
		}

                /// <summary>
                ///    [取得] 大盤指數資訊(請使用上市指數代號表或是上櫃指數代號表這兩個列舉類別來選定需要的類股代號)
                /// </summary>
		public MitakeIndexClassify[] Classifys {
			get;
			internal set;
		}

		/// <summary>
		///   [取得/設定] 接收旗標, 如果為 15 表示此四種封包都有收到(0x32=1, 0x33=2, 0x34=4, 0xb3=8)
		/// </summary>
		internal int Flag {
			get;
			set;
		}
	
		/// <summary>
		///   指數即時資訊類別
		/// </summary>
		public MitakeIndexTick() {
			this.Classifys = new MitakeIndexClassify[64];
			for (int i = 0; i < 64; i++) {
				Classifys[i] = new MitakeIndexClassify();
			}
		}

		internal void Clone(MitakeIndexTick tick, int flag) {
			this.Flag = tick.Flag;
			this.Ask = tick.Ask;
			this.Bid = tick.Bid;

			switch (flag) {
				case 1:  //0x32
				case 8:  //0xb3
					CloneIndex(tick);
					break;
				case 2:  //0x33
					CloneVolume(tick);
					break;
				case 4:  //0x34
					CloneTrust(tick);
					break;
			}
		}

		/// <summary>
		///   設定旗標, 如果為 15 表示此四種封包都有收到(0x32=1, 0x33=2, 0x34=4, 0xb3=8)
		/// </summary>
		/// <param name="flag">旗標(0x32=1, 0x33=2, 0x34=4, 0xb3=8)</param>
		internal void SetFlag(int flag) {
			this.Flag |= flag;
		}

		private void CloneIndex(MitakeIndexTick tick) {
			MitakeIndexClassify[] cClassifys = tick.Classifys;
			for (int i = 0; i < 64; i++) {
				this.Classifys[i].Clone(cClassifys[i]);
			}
		}

		private void CloneVolume(MitakeIndexTick tick) {
			this.Volume = tick.Volume;
			this.成交張數 = tick.成交張數;
			this.成交筆數 = tick.成交筆數;
		}

		private void CloneTrust(MitakeIndexTick tick) {
			this.委買合計筆數 = tick.委買合計筆數;
			this.委買總跌停張數 = tick.委買總跌停張數;
			this.委買總跌停筆數 = tick.委買總跌停筆數;
			this.委買總漲停張數 = tick.委買總漲停張數;
			this.委買總漲停筆數 = tick.委買總漲停筆數;
			this.委賣合計筆數 = tick.委賣合計筆數;
			this.委賣總跌停張數 = tick.委賣總跌停張數;
			this.委賣總跌停筆數 = tick.委賣總跌停筆數;
			this.委賣總漲停張數 = tick.委賣總漲停張數;
			this.委賣總漲停筆數 = tick.委賣總漲停筆數;
		}
	}
}