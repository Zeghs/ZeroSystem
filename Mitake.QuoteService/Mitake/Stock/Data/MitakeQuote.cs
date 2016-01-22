using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    個股資訊類別
        /// </summary>
        public sealed class MitakeQuote : IQuote {
		private HashSet<int> __cKeys = null;
		private SortedList<int, MitakeQuoteTick> __cTicks = null;

		/// <summary>
		///   [取得] 今日收盤價
		/// </summary>
		public double Close {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 即時資訊回補狀態
		/// </summary>
		public ComplementStatus ComplementStatus {
			get;
			internal set;
		}

		/// <summary>
		///    [取得] 委買委賣價量表
		/// </summary>
		public IDOMData DOM {
			get {
				return this.委買委賣資訊;
			}
		}

		/// <summary>
		///   [取得] 今日最高價
		/// </summary>
		public double High {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 今日最低價
		/// </summary>
		public double Low {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 今日開盤價
		/// </summary>
		public double Open {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 分價表
		/// </summary>
		public List<IPriceClassify> Prices {
			get {
				return MitakePriceClassify.GetPrices(__cTicks.Values as IEnumerable<ITick>);
			}
		}

		/// <summary>
		///   [取得] 目前最新即時報價
		/// </summary>
		public ITick RealTick {
			get {
				return this.即時資訊;
			}
		}

		/// <summary>
		///   [取得] 昨收價(昨收價為今日參考價)
		/// </summary>
		public double ReferPrice {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 商品代號
		/// </summary>
		public string SymbolId {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 商品名稱
		/// </summary>
		public string SymbolName {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 所有即時報價資訊的Tick Count
		/// </summary>
		public int TickCount {
			get {
				return __cTicks.Count;
			}
		}

		/// <summary>
		///   [取得/設定] 更新次數
		/// </summary>
		public int UpdateCount {
			get;
			set;
		}

		/// <summary>
		///   [取得] 期貨擴充:1=一般, 2=現月, 3=次月 (股票無使用此變數)
		/// </summary>
		public byte FutureMark {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下市(五個交易日後清除，會再分配給新上市使用)
		/// </summary>
		public bool 下市 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 個股是否為警示股
		/// </summary>
		public bool 警示 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 戳合類型:0=一般揭示, 1=暫緩撮合且瞬間趨跌, 2=暫緩撮合且瞬間趨漲, 3=試算後延後收盤
		/// </summary>
		public byte 戳合類型 {
			get;
			internal set;
		}

                /// <summary>
                ///   [取得] 市場別:0=集中市場, 1=上櫃, 2=期貨, 3=興櫃
                /// </summary>
		public byte 市場別 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 個股漲停價位
		/// </summary>
		public float 漲停價 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 個股跌停價位
		/// </summary>
		public float 跌停價 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 結算價(可能是期貨與選擇權專用)
		/// </summary>
		public float 結算價 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 昨日總成交量
		/// </summary>
		public uint 昨日總成交量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 今日總成交量
		/// </summary>
		public double 今日總成交量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 今日總成交額
		/// </summary>
		public double 今日總成交額 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 累計買量
		/// </summary>
		public uint 累計買量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 累計賣量
		/// </summary>
		public uint 累計賣量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 單筆買進巨量
		/// </summary>
		public uint 單筆買進巨量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 單筆賣出巨量
		/// </summary>
		public uint 單筆賣出巨量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 累計買進巨量
		/// </summary>
		public uint 累計買進巨量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 累計買進巨量
		/// </summary>
		public uint 累計賣出巨量 {
			get;
			internal set;
		}
		
                /// <summary>
                ///    [取得] 累計買進成交筆數 (選擇權:委託買進總筆數) @與選擇權共用
                /// </summary>
		public uint 買進成交筆數 {
			get;
			internal set;
		}

                /// <summary>
		///    [取得] 累計賣出成交筆數 (選擇權:委託賣出總筆數) @與選擇權共用
                /// </summary>
		public uint 賣出成交筆數 {
			get;
			internal set;
		}

                /// <summary>
		///    [取得] 累計成交合約數 (選擇權:總成交量) @與選擇權共用
                /// </summary>
		public uint 總成交合約數 {
			get;
			internal set;
		}

                /// <summary>
		///   [取得] 選擇權:委託買進總口數 (股票無使用此變數)
                /// </summary>
		public uint 委託買進總口數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 選擇權:委託賣出總口數 (股票無使用此變數)
                /// </summary>
		public uint 委託賣出總口數 {
			get;
			internal set;
		}

                /// <summary>
		///   [取得] 選擇權:總成交筆數 (股票無使用此變數)
                /// </summary>
                public uint   總成交筆數 {
			get;
			internal set;
		}
                
		/// <summary>
		///   [取得] 買進累計委託筆數
		/// </summary>
		public uint 買進累計委託筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 買進累計委託合約量
		/// </summary>
		public uint 買進累計委託合約量 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 賣出累計委託筆數
		/// </summary>
		public uint 賣出累計委託筆數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 賣出累計委託合約量
		/// </summary>
		public uint 賣出累計委託合約量 {
			get;
			internal set;
		}
		
		/// <summary>
		///   [取得] 未平倉合約數
		/// </summary>
		public uint 未平倉合約數 {
			get;
			internal set;
		}
		
                /// <summary>
                ///   [取得] 買進量百分比(賣出量百分比 = 100 - 買進量百分比)
                /// </summary>
		public byte 買進量百分比 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 即時量幅
		/// </summary>
		public float 即時量幅 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 即時均價
		/// </summary>
		public float 即時均價 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 昨日最高價
		/// </summary>
		public float 昨日最高價 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 昨日最低價
		/// </summary>
		public float 昨日最低價 {
			get;
			internal set;
		}
		
		internal int Serial {
			get;
			set;
		}

		internal string 產業別 {
			get;
			set;
		}

		internal string 證券別 {
			get;
			set;
		}

		internal byte 市場分類 {
			get;
			set;
		}

		internal MitakeEntrust 委買委賣資訊 {
			get;
			set;
		}

		internal MitakeQuoteTick 即時資訊 {
			get;
			set;
		}

		/// <summary>
		///   即時個股報價資訊
		/// </summary>
		public MitakeQuote(int capacity) {
			__cKeys = new HashSet<int>();
			__cTicks = new SortedList<int, MitakeQuoteTick>(capacity);

			this.SymbolId = string.Empty;
			this.SymbolName = string.Empty;
			this.即時資訊 = new MitakeQuoteTick();
			this.委買委賣資訊 = new MitakeEntrust();
			this.ComplementStatus = ComplementStatus.NotComplement;
		}

		/// <summary>
		///   取得即時報價Tick
		/// </summary>
		/// <param name="index">索引值(0=最新報價資訊)</param>
		/// <returns>返回值:ITick報價資訊</returns>
		public ITick GetTick(int index) {
			int iIndex = __cTicks.Count - ((index < 0) ? 0 : index) - 1;
			iIndex = ((iIndex < 0) ? 0 : iIndex);
			return __cTicks.Values[iIndex];
		}

                /// <summary>
                ///    取得成交資訊明細
                /// </summary>
                /// <param name="serial">即時資訊序號</param>
                /// <param name="tick">即時成交資訊(ref)</param>
                /// <returns>返回值：false=新建立  true=已存在</returns>
                internal bool GetMitakeTick(int serial, ref MitakeQuoteTick tick) {
                        bool bRet = false;
                        lock (__cKeys) {
                                if (__cKeys.Contains(serial)) {
					int iIndex = __cTicks.IndexOfKey(serial);
					tick = __cTicks.Values[iIndex];
                                        bRet = true;
                                } else {
					if (tick == null) {
						tick = new MitakeQuoteTick();
					}

					tick.Serial = serial;
					__cKeys.Add(serial);
					__cTicks.Add(serial, tick);
				}
                        }
                        return bRet;
                }
		
		internal MitakeQuoteTick GetPreviousTick(int serial) {
			MitakeQuoteTick cTick = null;

			int iIndex = __cTicks.IndexOfKey(serial);
			if (iIndex > 0) {
				cTick = __cTicks.Values[iIndex - 1];
			}
			return cTick;
		}
	}
}