using PowerLanguage;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    委買委賣資訊
        /// </summary>
        public sealed class MitakeEntrust : IDOMData {
		internal const int MAX_DOM_COUNT = 7;

                /// <summary>
                ///    比較價位(比較委買委賣價位，判斷是買盤還是賣盤)
                /// </summary>
                /// <param name="current">目前即時成交資訊</param>
                /// <param name="old">上一筆即時成交資訊</param>
                internal static void ComparePrice(MitakeQuoteTick current, MitakeQuoteTick old) {
                        if (old != null) {
                                if (current.Price > old.Price) {
                                        current.買賣盤 = 1;
                                } else if (current.Price < old.Price) {
                                        current.買賣盤 = 2;
                                } else {
                                        if (current.Ask.Price > old.Ask.Price) {
                                                current.買賣盤 = 1;
                                        } else if (current.Ask.Price < old.Ask.Price) {
                                                current.買賣盤 = 2;
                                        } else {
                                                if (current.Price == current.Bid.Price) {
                                                        current.買賣盤 = 2;
                                                } else if (current.Price == current.Ask.Price) {
                                                        current.買賣盤 = 1;
                                                }
                                        }
                                }
                        }
                }
		
		/// <summary>
		///   叫價資訊陣列
		/// </summary>
		public DOMPrice[] Ask {
			get;
			internal set;
		}

		/// <summary>
		///   詢價資訊陣列
		/// </summary>
		public DOMPrice[] Bid {
			get;
			internal set;
		}

		internal MitakeEntrust() {
			this.Ask = new DOMPrice[MAX_DOM_COUNT];
			this.Bid = new DOMPrice[MAX_DOM_COUNT];
		}
	}
}