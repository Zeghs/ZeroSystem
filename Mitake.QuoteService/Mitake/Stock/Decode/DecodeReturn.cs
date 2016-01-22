using Mitake.Events;
using Mitake.Stock.Data;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /// <summary>
        ///    Mcp命令封包回應解碼器
        /// </summary>
        internal sealed class DecodeReturn {
                /// <summary>
                ///   Mcp命令封包回應解碼
                /// </summary>
                /// <param name="item">StockEvent類別</param>
                /// <param name="IsDecode">是否啟動解碼功能</param>
                internal static void Decode(StockEvent item, bool IsDecode) {
                        item.Type = item.Source.Data[6];
                        item.Serial = (item.Source.Data[4] << 8) + item.Source.Data[5];
                }
        }
}