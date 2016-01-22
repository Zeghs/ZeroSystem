using Mitake.Stock.Data;
using Mitake.Sockets.Data;  

namespace Mitake.Events {
        /// <summary>
        ///   股票資料接收解碼後產生的事件類別
        /// </summary>
        public class StockEvent {
                /// <summary>
                ///    [取得/設定]封包檔頭型態 0x53=金融封包  0x23=上午時間封包  0x24=下午時間封包
                /// </summary>
		public byte Header {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]股票內部流水號
                /// </summary>
		public int Serial {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]股票資料類型
                /// </summary>
		public byte Type {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]封包資料內容
                /// </summary>
		public PacketBuffer Source {
                        get;
                        set;
                }

                internal StockEvent() { }

                internal StockEvent(byte header, int serial, byte type, PacketBuffer source) {
                        this.Header = header; 
                        this.Serial = serial;
                        this.Type = type;
                        this.Source = source;
                }
        }
}