using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///   股票資料儲存空間(類別唯一化物件)
        /// </summary>
        public sealed class MitakeStorage : IQuoteStorage {
                internal const int BASE_STOCK_CAPACITY = 32768;
                private static readonly MitakeStorage __current = new MitakeStorage();

		private List<MitakeQuote> __cQuotes = null;                 //股票資料陣列
		private Dictionary<int, int> __cKeys = null;                //股票資料索引
		private Dictionary<int, MitakeIndex> __cIndexs = null;      //大盤資料陣列

                /// <summary>
		///   [取得] MitakeStorage類別
                /// </summary>
                public static MitakeStorage Storage {
                        get {
                                return __current;
                        }
                }

                /// <summary>
		///   [取得] 所有報價資訊列表
                /// </summary>
                public List<MitakeQuote> Quotes {
                        get {
                                return __cQuotes;
                        }
                }

                private MitakeStorage() {
			__cQuotes = new List<MitakeQuote>(512);
			__cKeys = new Dictionary<int, int>(512);
			__cIndexs = new Dictionary<int, MitakeIndex>(32);
                }

		internal void Clear() {
			__cKeys.Clear();
			__cIndexs.Clear();
			__cQuotes.Clear();
		}

		public bool IsSymbolExist(string symbolId) {
			return MitakeSymbolManager.IsExist(symbolId);
		}

		public IQuote GetQuote(string symbolId) {
			int iSerial = MitakeSymbolManager.ConvertToSerial(symbolId);
			switch (iSerial) {
				case 9998: //上櫃指數
				case 9999: //上市指數
					return GetIndex(iSerial);
				default:
					return GetQuote(iSerial);
			}
		}

		/// <summary>
		///   取得陣列內的MitakeIndex類別資料
                /// </summary>
                /// <param name="serial">股票流水號</param>
		/// <returns>返回值: MitakeIndex類別</returns>
                internal MitakeIndex GetIndex(int serial) {
			MitakeIndex cMitakeIndex = null;
			if (!__cIndexs.TryGetValue(serial, out cMitakeIndex)) {
				Product cProduct = MitakeSymbolManager.GetIndexSymbolInformation(serial);
				
				cMitakeIndex = new MitakeIndex();
				cMitakeIndex.Serial = serial;

				if (cProduct != null) {
					cMitakeIndex.SymbolId = cProduct.SymbolId;
					cMitakeIndex.SymbolName = cProduct.SymbolName;
				}

                                lock (__cIndexs) {
					__cIndexs.Add(serial, cMitakeIndex);
                                }
                        }
                        return cMitakeIndex;
                }

                /// <summary>
		///   取得陣列內的MitakeQuote類別資料
                /// </summary>
                /// <param name="serial">股票流水號</param>
		/// <returns>返回值: MitakeQuote類別</returns>
		internal MitakeQuote GetQuote(int serial) {
			MitakeQuote cMitakeQuote = null;
			int iIndex = 0;
			lock (__cKeys) {
				if (__cKeys.TryGetValue(serial, out iIndex)) {
					cMitakeQuote = __cQuotes[iIndex];

					if (cMitakeQuote.SymbolId.Length == 0) {
						SetSymbolInformation(cMitakeQuote, MitakeSymbolManager.GetQuoteSymbolInformation(serial));
					}
				} else {
					MitakeSymbolInformation cSymbolInfo = MitakeSymbolManager.GetQuoteSymbolInformation(serial);

					int iMarketType = ((cSymbolInfo == null) ? 0 : cSymbolInfo.市場別);
					int iCapacity = ((iMarketType == 2) ? 16384 : 1024);
					cMitakeQuote = new MitakeQuote(iCapacity);
					cMitakeQuote.Serial = serial;

					SetSymbolInformation(cMitakeQuote, cSymbolInfo);

					iIndex = __cQuotes.Count;
					__cKeys.Add(serial, iIndex);
					__cQuotes.Add(cMitakeQuote);
				}
			}
			return cMitakeQuote;
		}

		private void SetSymbolInformation(MitakeQuote quote, MitakeSymbolInformation info) {
			if (info != null) {
				quote.FutureMark = info.FutureMark;
				quote.SymbolId = info.SymbolId;
				quote.SymbolName = info.SymbolName;
				quote.市場別 = info.市場別;
				quote.警示 = info.警示;
				quote.下市 = info.下市;
				quote.市場分類 = info.市場分類;
				quote.產業別 = info.產業別;
				quote.證券別 = info.證券別;
			}
		}
        }
}