using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;
using Mitake.Events;
using Mitake.Packets;
using Mitake.Sockets;
using Mitake.Stock.Data;
using Mitake.Stock.Decode;

namespace Mitake {
	/// <summary>
	///   三竹即時報價服務類別
	/// </summary>
        public class QuoteService : AbstractQuoteService {
		private const double TIME_ZONE = 8.0d;
		private static readonly ILog logger = LogManager.GetLogger(typeof(QuoteService));

		private bool __bReset = false;         //判斷是否為清盤指令所觸發的登入登出(收到清盤後, 會重新登入伺服器)
		private bool __bReseted = false;       //判斷是否已經清盤完畢
		private bool __bReLogin = false;
		private bool __bTimeFlag = true;
		private bool __bDisposed = false;
		private int __iNoPacketCount1 = 0;     //封包未送來的累積次數
		private int __iNoPacketCount2 = 0;     //封包未送來的累積次數
		private string __sUserID = null;       //保存登入使用者的ID
                private string __sSessionKey = null;   //Session Key
		private ZSocket __cSocket = null;      //股票Stream資料源
		private ZSocket __cSession = null;     //股票Session資料源
		private Timer __cTimer = null;         //偵測斷線計時器 
		private Timer __cReLoginTimer = null;  //斷線重連計時器
		private Subscribe __cSubscribe = null; //訂閱資訊
		private DateTime __cTradeDate = DateTime.UtcNow; //最後交易日期

		private object __oLock = new object();
                
                /// <summary>
                ///   [取得/設定] 是否啟動解碼器(預設:true)
                /// </summary>
                public bool IsDecode {
                        get;
                        set;
                }

		/// <summary>
		///   [取得] 報價資訊儲存媒體
		/// </summary>
		public override IQuoteStorage Storage {
			get {
				return MitakeStorage.Storage;
			}
		}

		/// <summary>
		///   [取得] 最後交易日期
		/// </summary>
		public override DateTime TradeDate {
			get {
				return __cTradeDate;
			}
		}

		/// <summary>
		///   [取得] 是否需要更新 
		/// </summary>
		private bool IsUpdate {
			get {
				DateTime cDate = DateTime.Now;
				int iNowDate = cDate.Year * 10000 + cDate.Month * 100 + cDate.Day;
				int iUpdateDate = this.UpdateTime.Year * 10000 + this.UpdateTime.Month * 100 + this.UpdateTime.Day;

				return (iNowDate > iUpdateDate);
			}
		}

		/// <summary>
                ///   建構子
                /// </summary>
		public QuoteService() {
                        this.IsDecode = true;  //預設開啟解碼功能

			__cSubscribe = new Subscribe();
			__cSubscribe.Add(0);  //清盤狀態(如果不訂閱不會清盤)

                        __cTimer = new Timer(10000);
                        __cTimer.AutoReset = false;
                        __cTimer.Elapsed += Timer_onElapsed;

			__cReLoginTimer = new Timer(30000);  //建立斷線重連的計時器
			__cReLoginTimer.AutoReset = false;
			__cReLoginTimer.Elapsed += ReLoginTimer_onElapsed;
                }

		/// <summary>
		///   新增報價資訊訂閱
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public override void AddSubscribe(string symbolId) {
			int iSerial = MitakeSymbolManager.ConvertToSerial(symbolId);
			if (iSerial > 0) {
				__cSubscribe.Add(iSerial);
				this.SendSubscribe();
			}
		}

		/// <summary>
		///   新增報價資訊訂閱(可多商品訂閱)
		/// </summary>
		/// <param name="symbolList">商品代號列表</param>
		public override void AddSubscribe(List<string> symbolList) {
			bool bSubscribe = false;
			int iLength = symbolList.Count;
			for (int i = 0; i < iLength; i++) {
				int iSerial = MitakeSymbolManager.ConvertToSerial(symbolList[i]);
				if (iSerial > 0) {
					__cSubscribe.Add(iSerial);
					bSubscribe = true;
				}
			}

			if (bSubscribe) {
				this.SendSubscribe();
			}
		}

		/// <summary>
		///   回補即時報價今天的所有歷史Tick
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public override void Complement(string symbolId) {
			int iSerial = MitakeSymbolManager.ConvertToSerial(symbolId);
			if (iSerial > 0) {
				switch (iSerial) {
					case 9998:  //OTC上櫃指數
					case 9999:  //TWI加權指數
						MitakeIndex cIndex = MitakeStorage.Storage.GetIndex(iSerial);
						cIndex.ComplementStatus = ComplementStatus.Complementing;
						break;
					default:
						MitakeQuote cQuote = MitakeStorage.Storage.GetQuote(iSerial);
						cQuote.ComplementStatus = ComplementStatus.Complementing;
						break;
				}

				Complement cComplement = new Complement();
				cComplement.Serial = iSerial;
				this.Send(cComplement);
			}
		}

		/// <summary>
		///   讀取即時報價服務的設定
		/// </summary>
		public override void Load() {
			string sLocation = Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			string[] sDatas = File.ReadAllLines(sFileName, Encoding.UTF8);

			JToken cQuoteService = JsonConvert.DeserializeObject<JToken>(sDatas[0]);
			this.DataSource = cQuoteService["DataSource"].Value<string>();
			this.ExchangeName = cQuoteService["ExchangeName"].Value<string>();
			this.IsDecode = cQuoteService["IsDecode"].Value<bool>();
			this.Password = cQuoteService["Password"].Value<string>();
			this.RemoteIP = cQuoteService["RemoteIP"].Value<string>();
			this.RemotePort = cQuoteService["RemotePort"].Value<int>();
			this.UpdateTime = cQuoteService["UpdateTime"].Value<DateTime>();
			this.UserId = cQuoteService["UserId"].Value<string>();

			MitakeSymbolManager.Deserialize(sDatas);
		}

		/// <summary>
		///   登入遠端伺服器
		/// </summary>
		/// <returns>返回值:true=登入程序已成功(等候onLogin事件通知), false=登入程序失敗</returns>
		public override bool Login() {
			if (this.IsLogin) {
				return true;
			}

			int iBufferSize = 1;
			byte[] bArray = new byte[64];

			if (logger.IsInfoEnabled) logger.InfoFormat("[QuoteService.Login] Login signer service... service={0}({1}), userId={2}", this.RemoteIP, this.RemotePort, this.UserId);
			AuthenticationLogin authLogin = new AuthenticationLogin(this.UserId, this.Password, 0, 0);
			try {
				Socket __cLogin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				__cLogin.Connect(new IPEndPoint(IPAddress.Parse(this.RemoteIP), this.RemotePort));
				__cLogin.Send(MitakePacket.ToBuffer(authLogin));
				iBufferSize = __cLogin.Receive(bArray);
				__cLogin.Close();
			} catch (Exception __errExcep) {
				logger.ErrorFormat("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
				return false;
			}

			if (iBufferSize == 0) {
				return false;
			} else {
				AuthenticationReturn authReturn = null;
				authReturn = MitakePacket.ToStructure<AuthenticationReturn>(bArray, iBufferSize);

				__sUserID = this.UserId;
				__sSessionKey = authReturn.SessionKey;

				string sServiceIP = authReturn.RemoteIP;
				int iPort = authReturn.RemotePort;
				if (logger.IsInfoEnabled) logger.InfoFormat("[QuoteService.Login] Register service... service={0}({1}), userId={2}, sessionKey:{3}", sServiceIP, iPort, __sUserID, __sSessionKey);

				if (__cSocket == null) {
					__cSocket = new ZSocket();
					__cSocket.ReceiveProc += SocketClient_onReceive;
					__cSocket.CloseProc += StockClient_onClose;
				}
				
				if (!RegisterServer(__cSocket, sServiceIP, iPort, 0x06)) {
					return false;
				}

				if (__cSession == null) {
					__cSession = new ZSocket();
					__cSession.ReceiveProc += SocketClient_onReceive;
					__cSession.CloseProc += StockClient_onClose;
				}
				
				if (!RegisterServer(__cSession, sServiceIP, iPort, 0x05)) {
					return false;
				}

				lock (__cTimer) {
					__bTimeFlag = true;
					__cTimer.Start();
				}

				StockDecoder.TimerProc += StockClient_onTimer;
				StockDecoder.StockProc += StockClient_onStock;
				McpDecoder.McpPacketProc += StockClient_onMcpPacket;

				MitakeSymbolManager.DataSource = this.DataSource;
				MitakeSymbolManager.ExchangeName = this.ExchangeName;
				StockDecoder.TimerProc += GetTradeDateFromLogin;  //先取得最後交易日期之後再處理股票代號表之類的運作

				this.SendSubscribe();  //登入成功後就送出訂閱資訊
			}
			return true;
		}

		/// <summary>
                ///    登出遠端伺服器
                /// </summary>
                public override void Logout() {
			StockDecoder.StockProc -= StockClient_onStock;
			StockDecoder.TimerProc -= StockClient_onTimer;
			StockDecoder.TimerProc -= GetTradeDateFromLogin;
                        McpDecoder.McpPacketProc -= StockClient_onMcpPacket;

                        try {
                                if (__cSession != null) {
					__cSession.CloseProc -= StockClient_onClose;
					if (__cSession.Connected) {
						__cSession.Send(MitakePacket.ToBuffer(new Logout())); //送出登出訊息
                                        }
                                        __cSession.Close();
                                }
                        } catch(Exception __errExcep1) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.Logout] {0}\r\n{1}", __errExcep1.Message, __errExcep1.StackTrace);
                        }

                        try {
                                if (__cSocket != null) {
					__cSocket.CloseProc -= StockClient_onClose;
					if (__cSocket.Connected) {
                                                __cSocket.Send(MitakePacket.ToBuffer(new Logout())); //送出登出訊息
                                        }
                                        __cSocket.Close();
                                }
                        } catch (Exception __errExcep2) {
				if (logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.Logout] {0}\r\n{1}", __errExcep2.Message, __errExcep2.StackTrace);
                        }

			lock (__cTimer) {
				__bTimeFlag = false;
				__cTimer.Stop();  //關閉Timer
			}

			lock (__cReLoginTimer) {
				__bReLogin = false;
				__cReLoginTimer.Stop();
			}
			
			__cSession = null;
                        __cSocket = null;

			this.IsLogin = false;
			if (logger.IsInfoEnabled) logger.Info("[QuoteService.Logout] Service logout success...");
		}

		/// <summary>
		///   取消報價資訊訂閱
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public override void RemoveSubscribe(string symbolId) {
			int iSerial = MitakeSymbolManager.ConvertToSerial(symbolId);
			if (iSerial > 0) {
				__cSubscribe.Remove(iSerial);
				this.SendSubscribe();
			}
		}

		/// <summary>
		///   儲存即時報價服務的設定
		/// </summary>
		public override void Save() {
			string sLocation = Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			this.UpdateTime = DateTime.Now;
			string sJSONSettings = JsonConvert.SerializeObject(this);
			string sJSONSymbolInfos = MitakeSymbolManager.Serialize();

			int iStringCount = sJSONSettings.Length + sJSONSymbolInfos.Length;
			StringBuilder cBuilder = new StringBuilder(iStringCount + 128);
			cBuilder.AppendLine(sJSONSettings);
			cBuilder.AppendLine(sJSONSymbolInfos);

			File.WriteAllText(sFileName, cBuilder.ToString(), Encoding.UTF8);
		}

		/// <summary>
		///   更新商品代號資訊
		/// </summary>
		public override void SymbolUpdate() {
			this.UpdateTime = DateTime.Today.AddDays(-2);  //修改更新時間到兩天前(強迫更新)
			MitakeSymbolManager.Update(DateTime.UtcNow.AddHours(TIME_ZONE)); //更新所有商品資訊

			LoadQuote cLoadQuote = new LoadQuote();  //請求所有股票代號表
			cLoadQuote.InfoName = 0x38;
			this.Send(cLoadQuote);
			if (logger.IsInfoEnabled) logger.Info("[QuoteService.SymbolUpdate] Request symbol list...");
		}

		/// <summary>
		///   釋放報價服務的所有資源(繼承後可複寫方法)
		/// </summary>
		/// <param name="disposing">是否處理受託管的資源</param>
		protected override void Dispose(bool disposing) {
			if (!__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					this.Logout();  //登出

					lock (__cTimer) {
						__bTimeFlag = false;
						__cTimer.Dispose();  //釋放Timer
					}

					lock (__cReLoginTimer) {
						__bReLogin = false;
						__cReLoginTimer.Dispose();
					}
				}
			}
			base.Dispose(disposing);
		} 
		
                /// <summary>
                ///    傳送MCP命令給伺服器
                /// </summary>
                /// <param name="mcpObject">MCP命令類別</param>
                internal void Send(object mcpObject) {
                        if (__cSession != null) {
                                if (__cSession.Connected) {
					try {
						__cSession.Send(MitakePacket.ToBuffer(mcpObject));
					} catch (Exception __errExcep) {
						if (logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.Send] {0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
					}
                                }
                        }
                }

		/// <summary>
                ///    傳送訂閱資料給伺服器
                /// </summary>
		internal void SendSubscribe() {
                        if (__cSocket != null) {
                                if (__cSocket.Connected) {
					try {
						__cSocket.Send(MitakePacket.ToBuffer(__cSubscribe));
					} catch (Exception __errExcep) {
						if (logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.SendSubscribe] {0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
					}
                                }
                        }
                        
			if (__cSession != null) {
                                if (__cSession.Connected) {
					try {
						__cSession.Send(MitakePacket.ToBuffer(__cSubscribe));
					} catch (Exception __errExcep) {
						if (logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.SendSubscribe] {0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
					}
                                }
                        }
                }

		private void GetTradeDateFromLogin(object sender, TimerEvent e) {
			StockDecoder.TimerProc -= GetTradeDateFromLogin;
			__cTradeDate = e.TradeDate;
			Mitake.Stock.Util.Time.SetToday(__cTradeDate);

			Task.Factory.StartNew(() => {
				if (__bReset) {
					this.IsLogin = true;
					if (!__bReseted) {  //如果還沒有清盤完畢, 就執行清盤動作
						MitakeSymbolManager.Update(DateTime.UtcNow.AddHours(TIME_ZONE), false);
						OnReset(new QuoteResetEvent(this.DataSource));
						if (logger.IsInfoEnabled) logger.InfoFormat("[QuoteService.Reset] Service \"{0}\" data reset success...", this.DataSource);

						__bReseted = true;  //設定已經清盤完畢的旗標
					}
				} else {
					if (this.IsUpdate) {
						SymbolUpdate();  //回補股票代號(每天只回補一次股票代號)
					} else {
						this.IsLogin = true;
						OnLoginCompleted();
					}
				}
			});
		}
		
		private bool RegisterServer(ZSocket socket, string serviceIP, int servicePort, byte command) {
                        SessionRegister sessionRegister = new SessionRegister(command, __sUserID, 0, __sSessionKey);
                        if (socket.Connect(serviceIP, servicePort)) {
                                try {
                                        socket.Send(MitakePacket.ToBuffer(sessionRegister));
                                        return true;
                                } catch(Exception __errExcep) {
                                        if(logger.IsErrorEnabled) logger.ErrorFormat("[QuoteService.RegisterServer] Register service fail... command={0}\r\n{1}\r\n{2}", command, __errExcep.StackTrace, __errExcep.Message);
				}
                        }
                        return false;
                }

		private void ReLogin() {
			bool bReLogin = false;
			lock (__oLock) {
				bReLogin = __bReLogin;
				__bReLogin = true;
			}

			if (!bReLogin) {
				this.Logout(); //登出
				ReLoginTimer_onElapsed(__cReLoginTimer, null);
			}
		}

                private void StockClient_onTimer(object sender, TimerEvent e) {
			OnQuoteDateTime(this.DataSource, e.QuoteDateTime);
                }

                private void StockClient_onStock(object sender, StockEvent e) {
			switch (e.Header) {
				case 0x4d:  //Mcp命令封包回應解碼
					switch (e.Type) {
						case 0xf0:  //訂閱完成通知回應
						case 0xf1:  //回補完成通知回應
							int iSerial = e.Serial;
							if (iSerial > 0) {  //有股票流水號
								IQuote cQuote = null;
								switch(iSerial) {
									case 9998:  //OTC上櫃指數
									case 9999:  //TWI加權指數
										cQuote = MitakeStorage.Storage.GetIndex(iSerial);
										if (e.Type == 0xf1) {
											(cQuote as MitakeIndex).ComplementStatus = ComplementStatus.Complemented;
										}
										break;
									default:
										cQuote = MitakeStorage.Storage.GetQuote(iSerial);
										if (e.Type == 0xf1) {
											(cQuote as MitakeQuote).ComplementStatus = ComplementStatus.Complemented;
										}
										break;
								}
								
								if (cQuote != null) {
									if (e.Type == 0xf0) {
										OnSubscribeCompleted(new QuoteComplementCompletedEvent(this.ExchangeName, this.DataSource, cQuote.SymbolId));
									} else {
										OnComplementCompleted(new QuoteComplementCompletedEvent(this.ExchangeName, this.DataSource, cQuote.SymbolId));
									}
								}
							}
							break;
						case 0xf8:  //股票代號回補完畢通知回應
							this.IsLogin = true;  //設定登入完成的旗標
							if (this.IsUpdate) {  //是否要更新
								this.Save();  //更新報價服務器的設定值
							}
							
							string sName = this.ExchangeName;
							AbstractExchange cExchange = ProductManager.Manager.GetExchange(sName);
							if (cExchange.IsUpdate) {  //檢查是否需要更新交易所商品代號表
								cExchange.Save();  //如果要更新則儲存更新後的結果
							}
							
							if (logger.IsInfoEnabled) logger.Info("[QuoteService.Login] Login service success...");
							OnLoginCompleted();
							break;
					}
					break;
				case 0x53:  //股票資訊封包
					switch (e.Type) {
						case 0x00:  //收到清盤資訊
							__bReset = true;     //如果收到清盤指令, 設定旗標
							__bReseted = false;  //將已經清盤完畢的旗標清除

							ReLogin(); //重新登入伺服器
							break;
						case 0x31:  //即時成交資訊
						case 0xb1:  //即時成交資訊
						case 0x3e:  //即時成交資訊
						case 0xbe:  //即時成交資訊
							if (sender == __cSocket) { //比較是否為即時Socket送來的資訊
								IQuote cQuote = MitakeStorage.Storage.GetQuote(e.Serial);
								if (cQuote != null) {
									OnQuote(new QuoteEvent(this.ExchangeName, this.DataSource, cQuote));
								}
							}
							break;
						case 0x32:  //大盤指數資訊
						case 0x33:  //大盤成交金額資訊
						case 0x34:  //大盤委買委賣資訊
							if (sender == __cSocket) { //比較是否為即時Socket送來的資訊
								IQuote cIndex = MitakeStorage.Storage.GetIndex(e.Serial);
								if (cIndex != null) {
									OnQuote(new QuoteEvent(this.ExchangeName, this.DataSource, cIndex));
								}
							}
							break;
						case 0x35:  //即時公告資訊(三竹傳來的新聞或是公告)
							MitakeNotice cNotice = Decode_S35.Decode(e.Serial, e.Source);
							if (cNotice != null) {
								OnNotice(new QuoteNoticeEvent(this.DataSource, cNotice));
							}
							break;
					}
					break;
			}
		}

                private void StockClient_onMcpPacket(McpPacketEvent e) {
			if (e.Type == 0x03 && e.Command == 0xf0) { //即時小訊息公告(可能是伺服器發送的公告或訊息)
				MitakeNotice cNotice = Decode_MF0.Decode(e.Source);
				if (cNotice != null) {
					OnNotice(new QuoteNoticeEvent(this.DataSource, cNotice));
				}
			}
                }

                private void SocketClient_onReceive(object sender, ReceiveEvent e) {
                        if (__cSocket == e.ActiveSocket) {
                                __iNoPacketCount1 = 0;
                        }
                        
			if (__cSession == e.ActiveSocket) {
                                __iNoPacketCount2 = 0;
                        }
                        StockDecoder.Decode(e.ActiveSocket, e.Token, this.IsDecode);
                }

                private void StockClient_onClose(object sender, CloseEvent e) {
			if (logger.IsWarnEnabled) logger.WarnFormat("[QuoteService.onClose] QuoteService disconnected... remoteIP={0}, remotePort={1}", e.RemoteIP, e.Port);
			e.Message = "與伺服器中斷連線，請檢查網路設定。";

			OnDisconnect(new QuoteDisconnectEvent(this.DataSource, e.RemoteIP, e.Port));

			ReLogin(); //重新登入伺服器
		}

                private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
                        ++__iNoPacketCount1;
                        ++__iNoPacketCount2;

                        if (__iNoPacketCount1 > 6 || __iNoPacketCount2 > 6) {
				ReLogin(); //重新登入伺服器
                        } else {
				lock (__cTimer) {
					if (__bTimeFlag) {
						__cTimer.Start();
					}
				}
                        }
                }
	
		private void ReLoginTimer_onElapsed(object sender, ElapsedEventArgs e) {
			if (Login()) {  //如果登入成功
				lock (__oLock) {
					__bReLogin = false;  //將旗標設定成 false
				}
			} else {  //如果登入不成功(間隔三十秒重新登入一次)
				lock (__cReLoginTimer) {
					if (__bReLogin) {
						__cReLoginTimer.Start();
					}
				}
			}
		}
	}
} //630行