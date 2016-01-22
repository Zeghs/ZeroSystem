using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Mitake.Stock.Util;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    大盤資訊類別
        /// </summary>
        public sealed class MitakeIndex : IQuote {
		private HashSet<DateTime> __cKeys = null;
		private SortedList<DateTime, MitakeIndexTick> __cTicks = null;

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
				MitakeEntrust cEntrusts = new MitakeEntrust();
				int iCount = MitakeEntrust.MAX_DOM_COUNT;
				for (int i = 0; i < iCount; i++) {
					cEntrusts.Ask[i] = DOMPrice.EMPTY;
					cEntrusts.Bid[i] = DOMPrice.EMPTY;
				}

				MitakeIndexTick cTick = this.即時資訊;
				cEntrusts.Ask[0] = cTick.Ask;
				cEntrusts.Bid[0] = cTick.Bid;
				return cEntrusts;
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
			get {
				return this.ReferPrices[0]; //0=上市櫃指數昨收價
			}
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
		///   [取得] 大盤指數昨收價(參考價) @請參考上市指數代號表與上櫃指數代號表
		/// </summary>
		public float[] ReferPrices {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 成交總額
		/// </summary>
		public double 成交總額 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 持平家數
		/// </summary>
		public uint 持平家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 未成交家數
		/// </summary>
		public uint 未成交家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 上漲家數
		/// </summary>
		public uint 上漲家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 下跌家數
		/// </summary>
		public uint 下跌家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 漲停家數
		/// </summary>
		public uint 漲停家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 跌停家數
		/// </summary>
		public uint 跌停家數 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 大盤綜合買氣
		/// </summary>
		public uint 大盤綜合買氣 {
			get;
			internal set;
		}

                /// <summary>
                ///  [取得] 大盤買賣比 (委買張/委賣張)
                /// </summary>
		public float 大盤買賣比 {
			get;
			internal set;
		}

		/// <summary>
		///  [取得] 加權指數價差
		/// </summary>
		public float 加權指數價差 {
			get;
			internal set;
		}

		/// <summary>
		///  [取得] 加權指數漲跌幅
		/// </summary>
		public float 加權指數漲跌幅 {
			get;
			internal set;
		}

		/// <summary>
		///  [取得] 不含金融價差
		/// </summary>
		public float 不含金融價差 {
			get;
			internal set;
		}

		/// <summary>
		///  [取得] 不含金融漲跌幅
		/// </summary>
		public float 不含金融漲跌幅 {
			get;
			internal set;
		}

		/// <summary>
		///  [取得] OTC指數價差
		/// </summary>
		public float OTC指數價差 {
			get;
			internal set;
		}
		
		/// <summary>
		///  [取得] OTC指數漲跌幅
		/// </summary>
		public float OTC指數漲跌幅 {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 類股綜合買氣(請參閱上市指數代號表，以取得綜合買氣值) @只有加權指數有此數值
		/// </summary>
		public Dictionary<int, uint> 類股綜合買氣 {
			get;
			internal set;
		}

		internal int Serial {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 即時資訊:最新大盤即時資訊，揭示板可以由此更新最新資料
		/// </summary>
		internal MitakeIndexTick 即時資訊 {
			get;
			set;
		}

                /// <summary>
		///   大盤資訊類別
                /// </summary>
		public MitakeIndex() {
                        __cKeys = new HashSet<DateTime>();
			__cTicks = new SortedList<DateTime, MitakeIndexTick>(4096);

			this.ReferPrices = new float[64];
			this.即時資訊 = new MitakeIndexTick();
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
                /// <param name="time">即時時間</param>
                /// <param name="tick">成交資訊明細(ref)</param>
                /// <returns>返回值：false=新建立  true=已存在</returns>
                internal bool GetMitakeTick(DateTime time, ref MitakeIndexTick tick) {
                        bool bRet = false;
                        lock (__cKeys) {
                                if (__cKeys.Contains(time)) {
					int iIndex = __cTicks.IndexOfKey(time);
					tick = __cTicks.Values[iIndex];
                                        bRet = true;
                                } else {
					if (tick == null) {
						tick = new MitakeIndexTick();
					}

                                        tick.Time = time;
					__cKeys.Add(time);
					__cTicks.Add(time, tick);
                                }
                        }
                        return bRet;
                }
		
		internal MitakeIndexTick GetPreviousTick(DateTime time, int flag) {
			MitakeIndexTick cTick = null;
			int iIndex = __cTicks.IndexOfKey(time);
			if (iIndex > 0) {
				int iFlag = 0;
				while (iFlag == 0 && iIndex > 0) {
					cTick = __cTicks.Values[--iIndex];
					iFlag = cTick.Flag & flag;
				}
			}
			return cTick;
		}
	}
}