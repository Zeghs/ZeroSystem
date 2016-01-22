using System;
using System.IO;
using System.Text;
using System.Net.Sockets;  
using System.Collections.Generic;
using Mitake.Events;
using Mitake.Stock.Data;
using Mitake.Stock.Util;
using Mitake.Sockets;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /// <summary>
        ///   股票封包解碼類別
        /// </summary>
        public sealed class StockDecoder {
                /// <summary>
                ///    當收到股票封包解碼後所產生的事件
                /// </summary>
                public static event StockHandler StockProc;     //股票資料事件

                /// <summary>
                ///    當收到時間封包解碼後所產生的事件
                /// </summary>
                public static event TimerHandler TimerProc;     //時間封包事件

		private static DecodeFinance __cFinance = new DecodeFinance();  //解碼所有國際金融期貨

                /// <summary>
                ///    解碼函式(解碼所有封包)
                /// </summary>
                /// <param name="socket">作用中的Socket類別</param>
                /// <param name="token">SocketToken類別</param>
                /// <param name="isDecode">是否啟動解碼功能</param>
                public static void Decode(Socket socket, SocketToken token, bool isDecode) {
			token.IsStart = false;

			PacketDecode(socket, token, isDecode);
                        
                        if (token.IsDataToTemp) {
				token.IsStart = true;
				
				token.Move();
                                PacketDecode(socket, token, isDecode);
                                token.Reduction(2048, 512);
                        }
                }

		/// <summary>
		///   重置並清除解碼後的所有資訊(只限於 TWSE 與 Mitake)
		/// </summary>
		/// <param name="date">設定日期(當天資料日期)</param>
		public static void Reset(DateTime date) {
			Time.SetToday(date);

			string sLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			string[] sDatas = File.ReadAllLines(sFileName, Encoding.UTF8);

			MitakeSymbolManager.Deserialize(sDatas);
			MitakeSymbolManager.DataSource = "Mitake";
			MitakeSymbolManager.ExchangeName = "TWSE";
			MitakeSymbolManager.Update(date);
			MitakeStorage.Storage.Clear();
		}
		
		private static void PacketDecode(Socket socket, SocketToken Token, bool IsDecode) {
                        int iCheck = 0;
                        PacketBuffer cPacket = Token.ReceiveBuffer;
                        cPacket.Position = -1;
                        while (++cPacket.Position < cPacket.Length) {
                                        iCheck = VerifyPacket(cPacket);
                                        switch (iCheck) {
                                                case -1: //未知封包
							if (Token.IsStart) {
								if (cPacket[0] == 0x0b) {
									if (McpDecoder.McpDecode(socket, Token)) {
										continue;
									}
								}
								Token.IsStart = false;
							}
							Token.AddTemp();
                                                        break;
                                                case 0:  //完整封包
                                                        int iSize = cPacket[3] + 4;
                                                        Token.SetPackage(iSize);
                                                        DecodeFinance(socket, Token.StockEvent, IsDecode);
                                                        cPacket.Position += (iSize - 1);
							Token.IsStart = true;
                                                        break;
                                                case 1:  //末端斷包
                                                        while (++cPacket.Position < cPacket.Length)
                                                                Token.AddTemp();
                                                        break;
                                        }
                                
                        }
                }

                /// <summary>
                ///     檢查金融封包
                /// </summary>
                /// <param name="item">來源封包</param>
                /// <returns>返回值：-1=未知封包  0=成功  1=末端斷包  2=封包長度不完整</returns>
                private static int VerifyPacket(PacketBuffer item) {
                        if (item[0] == 0x01 && item[2] == 0x02) {
                                if ((item.Position + 3) < item.Length) {
                                        int iSize = item[3] + 4;    //取得封包的總長度(包含標頭)
                                        if ((item.Position + iSize) > item.Length) {
                                                --item.Position;
                                                return 1;
                                        } else {
                                                byte bEOF = item[iSize - 1];
                                                if (bEOF == 0x4) { //檢查是否有正確的結束字元
                                                        if (Sockets.MitakePacket.GetChecksum(item, item.Position)) {
                                                                return 0;  //封包正確 
                                                        } else {
                                                                item.Position += (iSize - 1);
                                                                return 2;
                                                        }
                                                } else {
                                                        item.Position += 3; //跳至內容之後開始尋找下一個封包
                                                        while (++item.Position < item.Length)
                                                                if (item[0] == 0x02 && item[-2] == 0x01) break;
                                                        
							item.Position -= 3;
                                                        return 2;
                                                }
                                        }
                                }
                        }
                        return -1;
                }

                /// <summary>
                ///    解碼國際金融封包
                /// </summary>
                /// <param name="socket">作用中的Socket</param>
                /// <param name="item">StockEvent類別</param>
                /// <param name="isDecode">是否啟動解碼功能</param>
                private static void DecodeFinance(Socket socket, StockEvent item, bool isDecode) {
                        switch (item.Header) {
                                case 0x23:  //上午時間封包
                                case 0x24:  //下午時間封包
                                        item.Type = 0x54;  //時間封包 'T'
                                        TimerEvent cTimer = DecodeTime.Decode(item.Source, isDecode);
                                        if (cTimer != null) {
						if (TimerProc != null) {
							TimerProc(socket, cTimer);
						}
                                        }
                                        break;
                                case 0x4d:  //Mcp命令封包回應解碼
                                        DecodeReturn.Decode(item, isDecode);
                                        break;
                                case 0x41:  //國際匯市現價[原編碼]
                                case 0x43:  //國際匯市最高價[原編碼]
                                case 0x44:  //國際匯市最低價[原編碼]
                                case 0x4f:  //國際匯市開盤價[原編碼]
                                case 0x50:  //國際匯市昨收價[原編碼]
                                        item.Type = 1;  //國際匯市
                                        break;
                                case 0x2b:  //國際期貨BID價[原編碼]
                                case 0x2d:  //國際期貨ASK價[原編碼]
                                case 0x31:  //國際期貨現價[原編碼]
                                case 0x35:  //國際期貨收盤價[原編碼]
                                case 0x36:  //國際期貨昨收價[原編碼]
                                case 0x37:  //國際期貨成交量[原編碼]
                                case 0x38:  //國際期貨未平倉量[原編碼]
                                case 0x68:  //國際期貨最高價[原編碼]
                                case 0x6c:  //國際期貨最低價[原編碼]
                                case 0x6f:  //國際期貨開盤價[原編碼]
                                        item.Type = 0;  //國際期貨
                                        break;
                                default:
                                        __cFinance.Decode(item, isDecode);
                                        break;
                        }

                        if (StockProc != null) 
                                StockProc(socket, item);
                }
        }
}