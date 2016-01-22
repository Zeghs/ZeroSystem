using System;
using System.Linq;
using System.Collections.Generic;
using Zeghs.Data;

namespace Mitake.Stock.Data {
        /// <summary>
        ///    分價類別
        /// </summary>
        public sealed class MitakePriceClassify : IPriceClassify {
                /// <summary>
                ///    取得個股分價表
                /// </summary>
                internal static List<IPriceClassify> GetPrices(IEnumerable<ITick> items) {
                        List<IPriceClassify> cPriceList = null;
                        cPriceList = (from cTurnover in items
                                      where cTurnover.Price > 0
                                      let fTPrice = Math.Round(cTurnover.Price, 2)
                                      group cTurnover by new { Price = Math.Round(fTPrice, 2) } into grpPrice
                                      select new MitakePriceClassify {
                                              Price = Math.Round(grpPrice.Key.Price, 2),
                                              Volume = (uint)grpPrice.Sum(cTurnover => cTurnover.Single)
                                      }).ToList<IPriceClassify>();

                        return cPriceList;
                }

		/// <summary>
		///   [取得] 分價價格
		/// </summary>
		public double Price {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 分價價格內的統計總量
		/// </summary>
		public double Volume {
			get;
			internal set;
		}
        }
}