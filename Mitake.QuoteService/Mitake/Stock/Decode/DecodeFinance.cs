using Mitake.Events;

namespace Mitake.Stock.Decode {
        /// <summary>
        ///    解碼所有悅陽資訊的金融封包
        /// </summary>
        internal sealed class DecodeFinance {
                private object __oLockObj = new object();

                internal void Decode(StockEvent item, bool IsDecode) {
                        item.Type = item.Source.Data[6];
                        item.Serial = (item.Source.Data[4] << 8) + item.Source.Data[5];

                        if (IsDecode) {
                                switch (item.Header) {
                                        case 0x53:  //台灣股票証期權
						lock (__oLockObj) {
							DecodeStock.Decode(item);
						}
                                                break;
                                }
                        }
                }
        }
}