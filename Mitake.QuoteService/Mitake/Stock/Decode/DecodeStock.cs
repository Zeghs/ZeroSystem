using Mitake.Events;
using Mitake.Stock.Data;
using Mitake.Sockets.Data;  

namespace Mitake.Stock.Decode {
        /// <summary>
        ///    股票封包解碼器
        /// </summary>
        internal sealed class DecodeStock {
                /// <summary>
                ///   股票封包解碼
                /// </summary>
                /// <param name="item">StockEvent類別</param>
                internal static void Decode(StockEvent item) {
                        switch (item.Type) {
                                case 0x00:
                                        MitakeStorage.Storage.Clear();   //伺服器清盤(客戶端收到也立即清盤)
                                        break;
                                case 0x30:
                                        Decode_S30.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
                                        break;
                                case 0x31:
                                case 0xb1:
                                case 0x3e:
                                case 0xbe:
                                        Decode_S31.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
                                        break;
                                case 0x32:
                                        Decode_S32.Decode(MitakeStorage.Storage.GetIndex(item.Serial), item.Source);
                                        break;
                                case 0x33:
                                        Decode_S33.Decode(MitakeStorage.Storage.GetIndex(item.Serial), item.Source);
                                        break;
                                case 0xb3:
                                        Decode_SB3.Decode(MitakeStorage.Storage.GetIndex(item.Serial), item.Source);
                                        break;
                                case 0x34:
                                        Decode_S34.Decode(MitakeStorage.Storage.GetIndex(item.Serial), item.Source);
                                        break;
                                case 0x36:
                                        Decode_S36.Decode(MitakeStorage.Storage.GetIndex(item.Serial), item.Source);
                                        break;
                                case 0x37:
                                        Decode_S37.Decode(item.Serial, item.Source);
                                        break;
                                case 0x38:
					Decode_S38.Decode(item.Serial, item.Source);
                                        break;
                                case 0x3b:
                                case 0xbb:
                                case 0x3f:
                                case 0xbf:
                                        Decode_S3B.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
                                        break;
                                case 0x3c:
                                        Decode_S3C.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
                                        break;
				case 0x3d:
					Decode_S3D.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
					break;
				case 0xf1:
                                        Decode_SF1.Decode(MitakeStorage.Storage.GetQuote(item.Serial), item.Source);
                                        break;
                                default:
                                        break;
                        }
                }
        }
}