using System;
using Mitake.Sockets.Data;  

namespace Mitake.Events {
        /// <summary>
        ///    當收到MCP封包後所產生的事件
        /// </summary>
        internal delegate void McpPacketHandler(McpPacketEvent e);

        /// <summary>
        ///   當接收封包資料完成時會觸發此事件
        /// </summary>
        internal delegate void ReceiveHandler(object sender, ReceiveEvent e);
        
        /// <summary>
        ///   當連接成功時則會觸發此事件
        /// </summary>
        internal delegate void ConnectHandler(object sender, ConnectEvent e);

        /// <summary>
        ///   當遠端連線中斷時，則會觸發此事件
        /// </summary>
        internal delegate void CloseHandler(object sender, CloseEvent e);

        /// <summary>
        ///    當收到時間封包解碼後所產生的事件
        /// </summary>
        public delegate void TimerHandler(object sender, TimerEvent e);

        /// <summary>
        ///    當收到股票封包解碼後所產生的事件
        /// </summary>
        public delegate void StockHandler(object sender, StockEvent e);
        
        /// <summary>
        ///   委派原型：當ServiceEngine需要將封包加到所有Session時，就需要使用到此委派
        /// </summary>
        internal delegate void AddPacketHandler(PacketBuffer Buffer);
}