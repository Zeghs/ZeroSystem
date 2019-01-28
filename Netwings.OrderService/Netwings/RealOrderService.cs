using System;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Orders;
using Zeghs.Products;
using Zeghs.Services;
using Netwings.Rules;
using Netwings.Event;
using Netwings.Pipes;
using Netwings.Orders;

namespace Netwings {
	public class RealOrderService : AbstractOrderService, IOrderSender, IOrderEntrust {
		private static readonly ILog logger = LogManager.GetLogger(typeof(RealOrderService));
		private static int __iDealIndex = 0;
		private static string __sFullDeal = "完全成交";
		private static string __sClosedString = "平倉";
		private static Dictionary<ERuleType, List<RulePropertyAttribute>> __cRuleItems = null;

		private static DateTime GetExpiration(Instrument bars) {
			InstrumentSettings cSettings = bars.Info as InstrumentSettings;
			cSettings.SetExpirationFromTime(bars.BarUpdateTime);
			return cSettings.Expiration;
		}
		
		private static string ChangeWeekOptionsSymbolId(int year, int month, int day) {
			DateTime cInDT = new DateTime(year, month, day);
			int iFirstWeek = (int) new DateTime(year, month, 1).DayOfWeek;
			int iTotalDay = DateTime.DaysInMonth(year, month);
			int iFirstWed = 0, iOutWeek = 0;
			
			iFirstWed = (7 - iFirstWeek > 3) ? 4 - iFirstWeek : 11 - iFirstWeek;
			if (day - iFirstWed == 0) {
				iOutWeek = 1;
			} else {
				iOutWeek = ((day - iFirstWed) / 7) + 1;
				if ((day - iFirstWed) % 7 > 0) {
					iOutWeek = iOutWeek + 1;
				}
			}
			
			if (iOutWeek > 4 && (iFirstWed + 28) > iTotalDay) {
				iOutWeek = 1;
			}
			
			if (iOutWeek > 5) {
				iOutWeek = 1;
			}

			string ReWeek = (iOutWeek == 3) ? "O" : iOutWeek.ToString();
			return "TX" + ReWeek;
		}

		private int __iPrevious = 0;
		private int __iTrustIndex = 1;
		private int __iDecimalPoint = 0;  //小數點位數
		private int __iMaxTrustIndex = 1;
		private bool __bBusy = false;
		private bool __bDisposed = false;
		private string __sApiPipe = null;
		private string __sLocalPipe = null;
		private TradeOrder __cCloseOrder = null;
		private PositionSeries __cPositions = null;
		private Queue<string> __cReserves = null;
		private Queue<TradeOrder> __cDeals = null;
		private AutoResetEvent __cResetEvent = null;
		private TradeList<TradeOrder> __cEntrusts = null;
		private MarketPosition __cCurrentPosition = null;
		private HashSet<string> __cNextBarRequires = null;  //存放 NextBar 下單的判斷依據(當要下 NextBar 時會將 name 存入此結構內, 如果還沒到下一根 Bars 就會清除, 並等待下一次又觸發條件時再存入)
		private HashSet<string> __cPNextBarRequires = null; //存放上一個 __cNextBarRequires 的變化, 因為 NextBar 是在下一根下單, 所以要提供的是上一根的最後狀態(提供給下 NextBars 的方法一個是否要下單的依據)
		private NetwingsNamedPipeStream __cPipeStream = null;
		private object __oLock = new object();      //lock 專用的變數

		/// <summary>
		///   [取得] 目前留倉部位
		/// </summary>
		public override IMarketPosition CurrentPosition {
			get {
				return __cCurrentPosition;
			}
		}

		/// <summary>
		///   [取得] 留倉部位序列資訊
		/// </summary>
		public override PositionSeries Positions {
			get {
				return __cPositions;
			}
		}

		/// <summary>
		///   [取得/設定] 遠端 API 下單機編號
		/// </summary>
		[Input("遠端API下單機編號")]
		protected int ApiNumber {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 本地下單模組通道編號
		/// </summary>
		[Input("本地下單模組通道編號")]
		protected int PipeNumber {
			get;
			set;
		}

		TradeList<TradeOrder> IOrderEntrust.Entrusts {
			get {
				return __cEntrusts;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public RealOrderService() {
			this.ApiNumber = 1;  //預設通道編號
			this.PipeNumber = 1;  //預設通道編號

			__cDeals = new Queue<TradeOrder>(16);
			__cReserves = new Queue<string>(16);
			__cEntrusts = new TradeList<TradeOrder>(64);

			__cNextBarRequires = new HashSet<string>();
			__cPNextBarRequires = new HashSet<string>();

			__cCurrentPosition = new MarketPosition(16);  //建立目前留倉部位
			__cPositions = new PositionSeries();          //建立留倉部位陣列
			__cPositions.Value = __cCurrentPosition;

			__cResetEvent = new AutoResetEvent(false);
		}

		/// <summary>
		///   建立佣金規則介面
		/// </summary>
		/// <param name="rule">基礎規則類別</param>
		/// <returns>返回值: ICommission 介面</returns>
		public override ICommission CreateCommission(RuleBase rule) {
			return RuleCreater.CreateRule(rule) as ICommission;
		}

		/// <summary>
		///   下單服務初始化(初始化會在參數設定之後呼叫)
		/// </summary>
		public override void Initialize() {
			__sApiPipe = string.Format("A{0}", this.ApiNumber);
			__sLocalPipe = string.Format("S{0}", this.PipeNumber);

			__cPipeStream = new NetwingsNamedPipeStream(__sLocalPipe);
			__cPipeStream.onMessage += PipeStream_onMessage;
			__cPipeStream.Listen();
			if (logger.IsInfoEnabled) logger.InfoFormat("[RealOrderService.Initialize] Create NamedPipeStream... LocalPipe={0}, ApiPipe={1}", __sLocalPipe, __sApiPipe);

			string sCommand = string.Format("REPORT,{0},{1},{2}", __sLocalPipe, GetOrderSymbol(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			if (logger.IsInfoEnabled) logger.InfoFormat("[RealOrderService.Initialize] Send \"{0}\" command to API... ApiPipe={1}", sCommand, __sApiPipe);
			__cPipeStream.Send(__sApiPipe, sCommand);  //傳送命令以取得目前下單回報

			if (logger.IsInfoEnabled) logger.Info("[RealOrderService.Initialize] Waiting order data response...");
			__cResetEvent.WaitOne();  //等待接收未平倉資訊
		}

		public override List<RulePropertyAttribute> GetRuleItems(ERuleType ruleType) {
			List<RulePropertyAttribute> cRuleItems = null;
			__cRuleItems.TryGetValue(ruleType, out cRuleItems);
			return cRuleItems;
		}

		/// <summary>
		///   建立限價買賣模式(送出使用者指定的價格的委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced介面</returns>
		public override IOrderPriced Limit(SOrderParameters orderParams) {
			return new OrderPriced(this, orderParams);
		}

		/// <summary>
		///   讀取下單服務的參數
		/// </summary>
		public override void Load() {
			if (__cRuleItems == null) {
				Assembly cAssembly = Assembly.GetExecutingAssembly();
				__cRuleItems = RulePropertyAttribute.GetRules(cAssembly);
			}
		}

		/// <summary>
		///   建立市價買賣模式(在下一根 Bars 建立之後以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public override IOrderMarket MarketNextBar(SOrderParameters orderParams) {
			return new OrderMarket(this, orderParams, true);
		}

		/// <summary>
		///   建立市價買賣模式(立即以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public override IOrderMarket MarketThisBar(SOrderParameters orderParams) {
			return new OrderMarket(this, orderParams, false);
		}

		public override void OnWork() {
			if (Bars.IsLastBars) {
				if (Bars.CurrentBar > __iPrevious) {
					SendNextBars(__iPrevious);  //處理 NextBars 下單程序
					__iPrevious = Bars.CurrentBar;
				}

				HashSet<string> cSwap = __cPNextBarRequires;  //將上一個條件狀態保留起來
				__cPNextBarRequires = __cNextBarRequires;  //將目前條件狀態置換到上一個條件狀態變數內
				__cNextBarRequires = cSwap;  //將上一個狀態置換到目前狀態上(減少使用 new 而造成 GC 負擔)
				__cNextBarRequires.Clear();  //清除狀態並等待下一輪 CalcBar 時儲存新條件狀態

				if (__cCloseOrder != null && (__cCloseOrder.IsCancel || __cCloseOrder.Contracts == 0)) {
					bool bClear = true;
					int iCount = __cEntrusts.Count;
					if (iCount > 0) {
						bool bCancel =  __cCloseOrder.IsCancel;
						for (int i = iCount - 1; i >= 0; i--) {  //迴圈由最後一筆往前取得(避免 GetWaiting 取出後刪單造成取得順序問題)
							TradeOrder cTemp = __cEntrusts[i];
							if (bCancel && !cTemp.IsSended) {  //如果目前的平倉單被取消, 而且此單在倉閘內尚未傳送
								GetWaiting(cTemp.SymbolId, cTemp.Price);  //直接從委託倉閘內取出(放棄此單, 因為尚未送出可以直接取出並放棄)
								continue;
							} else if (!cTemp.IsSended && cTemp.Ticket != cTemp.Name) {  //如果此單尚未送出而且 Ticket 與 Name 都不同(相同表示可能是下 NextBar 單而且尚未傳送, 這些單還不能傳送)
								SendTrust(cTemp);
							}
							
							if (bClear) {
								bClear = cTemp.IsTrusted && cTemp.IsDealed;  //平倉完成後需要檢查委託倉內是否全部的單都有成交動作(如果有反向單則成交了才能改變倉位方向, 所以必須檢查是否全部委託單是否已經開始成交)
							}
						}
					}

					if (bClear) {  //如果所有的單都已經開始成交或是已經無單, 表示可以清除此平倉變數
						__cCloseOrder = null;
					}
				}

				if (__cCloseOrder == null) {  //沒有平倉內容才可以計算成交部位(這樣倉位方向才不會算錯)
					CalculatePositions();  //計算成交部位狀況
				}
				AsyncCalculateProfits();  //計算留倉部位損益
			}
		}

		/// <summary>
		///   寫入下單服務的參數
		/// </summary>
		public override void Save() {
		}

		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="action">下單進出場動作</param>
		/// <param name="category">下單類型</param>
		/// <param name="limitPrice">限價價格(市價=0)</param>
		/// <param name="lots">下單數量</param>
		/// <param name="isReverse">是否需反轉倉位</param>
		/// <param name="touchPrice">觸發或停損價格</param>
		/// <param name="name">下單註解</param>
		/// <param name="openNextBar">是否開倉在下一根 Bars</param>
		public virtual bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false) {
			if (this.Bars.IsLastBars) {
				//檢查是否下單類型是平倉單(如果是平倉單需要將委託倉內的所有同向平倉單都取消, 全部取消完畢才可以在下平倉單)
				if (action == EOrderAction.Sell || action == EOrderAction.BuyToCover) {
					bool bRet = false;
					int iCount = __cEntrusts.Count;
					if (iCount > 0) {
						for (int i = 0; i < iCount; i++) {
							TradeOrder cTemp = __cEntrusts[i];
							if (cTemp.IsTrusted && cTemp.Price > 0 && cTemp.Contracts > 0 && cTemp.Action == action) {
								if (!cTemp.IsCancel) {
									cTemp.IsCancel = SendTrust(cTemp, true);  //送出取消委託單命令
								}
								bRet = true;
							}
						}
					}

					if (bRet || __cDeals.Count > 0) {
						return false;
					}
				}

				limitPrice = Math.Round(limitPrice, __iDecimalPoint);
				TradeOrder cTrust = __cEntrusts.GetTradeFromName(name);
				if (cTrust != null) {
					if (openNextBar) {
						if (!cTrust.IsSended) {
							cTrust.Price = limitPrice;  //支援可以下出 NextBar 的限價單(沒有指定會以 0 送出)
							__cNextBarRequires.Add(name);  //標記 NextBar 時, 可以下單
							return true;
						}
						return false;
					} else {
						if (cTrust.Price == limitPrice) {  //委託價格一樣就忽略
							return false;
						} else {
							if (cTrust.IsTrusted && !cTrust.IsCancel) {  //如果已經委託完成就取消單號
								cTrust.IsCancel = SendTrust(cTrust, true);  //向下單機傳送取消委託單的命令
							}
							return false;
						}
					}
				}

				TradeOrder cOrder = new TradeOrder();  //建立預約委託單的資訊
				cOrder.Action = action;
				cOrder.BarNumber = Bars.CurrentBar;
				cOrder.Category = category;
				cOrder.Contracts = lots;
				cOrder.Name = name;
				cOrder.Price = limitPrice;
				cOrder.IsReverse = isReverse;
				cOrder.SymbolId = GetOrderSymbol();
				cOrder.Time = DateTime.Now;
				cOrder.Ticket = (openNextBar) ? name : GetTrustID();
				__cEntrusts.Add(cOrder);  //加入至委託列表內

				if (openNextBar) {  //如果需要在下一根 Bars 下單, 就先保留 name 在佇列, 以方便比對委託單
					__cReserves.Enqueue(name);
					__cNextBarRequires.Add(name);
				} else {
					if (__cCloseOrder == null) {
						SendTrust(cOrder);  //傳送新委託單給下單機
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="trust">交易訂單資訊</param>
		/// <param name="isCancel">是否要取消此交易訂單(成功委託才能取消訂單)</param>
		/// <returns>返回值: true=成功, false=失敗</returns>
		bool IOrderSender.Send(TradeOrder trust, bool isCancel) {
			return this.SendTrust(trust, isCancel);
		}

		/// <summary>
		///   設定 Instrument 資訊
		/// </summary>
		/// <param name="bars">Instrument 類別</param>
		/// <param name="data_stream">資料串流編號</param>
		public override void SetInstrument(Instrument bars, int data_stream) {
			base.SetInstrument(bars, data_stream);

			string sDecimal = Bars.Info.PriceScale.ToString();  //取得價格座標並轉換成字串
			int iIndex = sDecimal.IndexOf(".") + 1;  //搜尋小數點位數
			if (iIndex > 0) {
				__iDecimalPoint = sDecimal.Length - iIndex;  //計算小數位數
			}
			__cCurrentPosition.SetBigPointValue(Bars.Info.BigPointValue);  //設定每一大點的交易金額
		}

		/// <summary>
		///   建立停損模式(觸發到停損點後，以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced介面</returns>
		public override IOrderPriced Stop(SOrderParameters orderParams) {
			return null;
		}

		/// <summary>
		///   建立停損限價單模式(觸發到停損點後，以使用者指定的價格送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderStopLimit介面</returns>
		public override IOrderStopLimit StopLimit(SOrderParameters orderParams) {
			return null;
		}

		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					base.Dispose(disposing);

					__cDeals.Clear();
					__cEntrusts.Clear();
					__cNextBarRequires.Clear();
					__cPNextBarRequires.Clear();

					__cReserves.Clear();
					__cPositions.Dispose();
					__cPipeStream.Dispose();
					__cCurrentPosition.Clear();

					__cResetEvent.Dispose();
				}
			}
		}

		internal bool SendTrust(TradeOrder trust, bool isCancel = false) {
			EOrderAction cAction = trust.Action;
			bool bClose = cAction == EOrderAction.Sell || cAction == EOrderAction.BuyToCover;
			StringBuilder cBuilder = new StringBuilder(128);
			cBuilder.Append(__sLocalPipe).Append(",")
				.Append(trust.SymbolId).Append(",")
				.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Append(",")
				.Append((cAction == EOrderAction.Buy || cAction == EOrderAction.BuyToCover) ? 'B' : (cAction == EOrderAction.Sell || cAction == EOrderAction.SellShort) ? 'S' : ' ').Append(",")
				.Append(trust.Contracts).Append(",")
				.Append(trust.Price).Append(",")
				.Append((isCancel) ? -1 : (bClose) ? 0 : 1).Append(",")
				.Append(Math.Round(Bars.Close.Value, __iDecimalPoint)).Append(",")
				.Append((isCancel) ? trust.Ticket : (trust.IsReverse) ? "1" : "0");

			string sCommand = cBuilder.ToString();
			bool bSuccess = __cPipeStream.Send(__sApiPipe, sCommand);  //發送委託單命令給下單機
			if (logger.IsInfoEnabled) logger.InfoFormat("[RealOrderService.Send] {0}", sCommand);

			if (bSuccess) {  //如果傳送成功
				trust.IsSended = true;  //設定已經傳送完畢
				if (bClose) {  //如果是平倉單
					__cCloseOrder = trust;  //指定平倉單至變數內(模組會監測平倉單完畢後才會啟動之後的單做下單動作, 避免倉部位錯亂)
				}
			} else {
				__cEntrusts.Remove(trust.Ticket);  //移除此筆尚未委託的委託單
			}
			return bSuccess;
		}

		private void AsyncCalculateProfits() {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bBusy;
				if (!bBusy) {
					__bBusy = true;
				}
			}

			if (!bBusy) {
				Task.Factory.StartNew(() => {
					if (__cCurrentPosition.OpenLots > 0) {  //如果有留倉部位
						__cCurrentPosition.CalculateProfits(Bars.Close.Value);  //計算目前損益
					}

					lock (__oLock) {
						__bBusy = false;
					}
				});
			}
		}

		private void CalculatePositions() {
			while (__cDeals.Count > 0) {
				TradeOrder cDeal = null;
				lock (__cDeals) {
					cDeal = __cDeals.Dequeue();
				}

				string sTicketID = cDeal.Ticket;
				cDeal.Ticket = GetDealID();  //填入成交單號(自動編號)
				bool bClosed = __cCurrentPosition.CheckPosition(cDeal);  //檢查是否已經平倉完畢
				int iLatestHistoryCount = __cCurrentPosition.LatestHistoryCount;  //取得最新新增的平倉歷史明細個數
				if (bClosed) {  //如果已經平倉完畢
					__cCurrentPosition = new MarketPosition(16);  //重新建立新的留倉部位
					__cCurrentPosition.SetBigPointValue(Bars.Info.BigPointValue);  //設定每一大點的交易金額

					++__cPositions.Current;  //移動目前留倉部位陣列的目前索引值
					__cPositions.Value = __cCurrentPosition;  //指定新的留倉部位至留倉陣列
				}

				if (cDeal.IsDealed) {  //最後一張成交單成交完畢則表示委託單的下單數量已經全部成交完畢(最後一張成交單成交完畢後委託單數量會為 0 且最後一張成交單的 IsDealed 為真)
					__cEntrusts.Remove(sTicketID);  //當完全成交完畢後就移除委託單
				}
				OnResponse(cDeal, cDeal.SymbolId, ResponseType.Deal, (bClosed) ? null : __cCurrentPosition, (bClosed) ? __cPositions[1].ClosedTrades : __cCurrentPosition.ClosedTrades, iLatestHistoryCount);
			}

			if (__cCurrentPosition.OpenLots > 0) {  //檢查是否有開倉(有開倉就要發送這個更新事件, 這樣損益才會被更新)
				OnResponse(null, Bars.Request.Symbol, ResponseType.Update);
			}
		}

		private bool CheckSymbol(string symbol1, string symbol2) {
			symbol1 = symbol1.Split('_')[0];
			symbol2 = symbol2.Split('_')[0];
			return symbol1.Equals(symbol2);
		}

		private string GetDealID() {
			int iID = Interlocked.Increment(ref __iDealIndex);
			return iID.ToString();
		}

		private string GetOrderSymbol() {
			Product cSymbol = Bars.Info.ASymbolInfo2;
			string sSymbolId = cSymbol.SymbolId;
			switch (cSymbol.Category) {
				case ESymbolCategory.IndexOption:
					DateTime cIOExpiration = GetExpiration(Bars);
					int iIndex = sSymbolId.LastIndexOf(".");
					sSymbolId = string.Format("{0}_{1}{2}{3}", (sSymbolId[2] == 'W') ? ChangeWeekOptionsSymbolId(cIOExpiration.Year, cIOExpiration.Month, cIOExpiration.Day) : sSymbolId.Substring(0, 3), cIOExpiration.Year, cIOExpiration.Month.ToString("00"), sSymbolId.Substring(4, iIndex - 4));
					break;
				case ESymbolCategory.Future:
					DateTime cFExpiration = GetExpiration(Bars);
					sSymbolId = string.Format("{0}_{1}{2}", sSymbolId.Substring(0, 3), cFExpiration.Year, cFExpiration.Month.ToString("00"));
					break;
			}
			return sSymbolId;
		}

		private string GetTrustID() {
			int iID = __iMaxTrustIndex;
			Interlocked.Increment(ref __iMaxTrustIndex);
			return iID.ToString();
		}

		private TradeOrder GetWaiting(string symbolId, double price) {
			TradeOrder cTrust = null;
			if (__iMaxTrustIndex > __iTrustIndex) {
				string sTrustTicket = __iTrustIndex.ToString();
				cTrust = __cEntrusts.GetTrade(sTrustTicket);
				if (cTrust != null) {
					if (cTrust.SymbolId.Equals(symbolId) && cTrust.Price == price) {
						Interlocked.Increment(ref __iTrustIndex);
						__cEntrusts.Remove(sTrustTicket);
					} else {
						cTrust = null;
					}
				}
			}
			return cTrust;
		}

		private void SendNextBars(int previousBar) {
			while (__cReserves.Count > 0) {
				string sTicket = __cReserves.Peek();  //取出以 name 當作 ticket 的委託單字串(NextBars 都先以 name 當作委託單的 ticket 號碼)
				TradeOrder cTrust = __cEntrusts.GetTrade(sTicket);  //取得委託單資訊
				if (cTrust.BarNumber > previousBar) {
					break;
				} else {
					__cReserves.Dequeue();   //移除保留的 name
					__cEntrusts.Remove(sTicket);   //移除暫存的委託單

					if (__cPNextBarRequires.Contains(sTicket)) {  //檢查有符合條件才下單
						cTrust.Ticket = GetTrustID();  //設定委託ID
						cTrust.BarNumber = Bars.CurrentBar;  //重新設定下單的 BarNumber(NextBar 如果條件成立會在下一個 Bar 下單)
						__cEntrusts.Add(cTrust);  //加入至委託列表內

						if (__cCloseOrder == null) {
							this.SendTrust(cTrust);  //發送委託單
						}
					}
				}
			}
		}

		private void PipeStream_onMessage(object sender, MessageEvent e) {
			if (e.Buffer[0] == 0x7b) {  //檢查是否為 JSON 開頭符號
				JToken cToken = JsonConvert.DeserializeObject<JToken>(e.Message);
				int iDataType = cToken["Type"].Value<int>();

				switch (iDataType) {
					case 0: //接收 REPOTR 完畢的回報類型(如果沒收到此命令會無窮盡等待)
						__cResetEvent.Set();
						if (logger.IsInfoEnabled) logger.InfoFormat("[Report] #{0} Order data response completed...", this.Bars.Request.Symbol);
						break;
					case 1: //委託回報
						string sOrderSymbolId = null;
						OrderTrust[] cTrusts = cToken["Report"].ToObject<OrderTrust[]>();
						foreach (OrderTrust cTrust in cTrusts) {
							string sTrustId =  cTrust.委託書號;
							int iTrustCount = cTrust.未成交數量;
							
							TradeOrder cTrustOrder = __cEntrusts.GetTrade(sTrustId);  //取得委託單
							if (cTrustOrder == null && iTrustCount > 0) {  //委託未成交數量大於0才處理
								string sSymbolId = cTrust.商品代號;
								
								cTrustOrder = GetWaiting(sSymbolId, cTrust.委託價格);  //取得等待委託回報的委託單(這些委託單都要等待委託單號)
								if (cTrustOrder == null) {
									if (__iMaxTrustIndex == __iTrustIndex) {  //相等表示沒有下出任何的委託單, 可能是留在下單機內的委託單
										if (sOrderSymbolId == null) {
											sOrderSymbolId = GetOrderSymbol();
										}

										if (CheckSymbol(sSymbolId, sOrderSymbolId)) {  //比對兩個商品代號是否相同, 相同才會被加入委託倉內
											cTrustOrder = new TradeOrder();
											cTrustOrder.IsSended = true;
											cTrustOrder.IsTrusted = true;
											cTrustOrder.Name = sTrustId;
											cTrustOrder.Ticket = sTrustId;
											cTrustOrder.SymbolId = sSymbolId;
											cTrustOrder.Contracts = iTrustCount;
											cTrustOrder.Time = cTrust.委託時間;
											cTrustOrder.Price = Math.Round(cTrust.委託價格, __iDecimalPoint);
											cTrustOrder.Category = (cTrustOrder.Price == 0) ? OrderCategory.Market : OrderCategory.Limit;
											
											if (cTrust.倉別.Equals(__sClosedString)) {
												cTrustOrder.Action = (cTrust.買賣別) ? EOrderAction.BuyToCover : EOrderAction.Sell;
											} else {
												cTrustOrder.Action = (cTrust.買賣別) ? EOrderAction.Buy : EOrderAction.SellShort;
											}
											__cEntrusts.Add(cTrustOrder);  //儲存至委託陣列內
										}
									} else {
										continue;
									}
								} else {
									cTrustOrder.IsTrusted = true;   //true=委託成功
									cTrustOrder.Ticket = sTrustId;  //填入回報來的委託單號
									cTrustOrder.Time = cTrust.委託時間;
									
									__cEntrusts.Add(cTrustOrder);   //儲存至委託陣列內
								}
							}

							if (cTrustOrder != null) {
								bool bDelete = cTrust.是否刪單;
								if (bDelete) {
									__cEntrusts.Remove(sTrustId);  //從委託陣列內移除委託
								}

								OnResponse(cTrustOrder, cTrustOrder.SymbolId, (bDelete) ? ResponseType.Cancel : ResponseType.Trust);
								if (logger.IsInfoEnabled) logger.InfoFormat("[Trust] #{0} {1} {2} {3} at {4} {5} @{6}", cTrustOrder.Ticket, cTrustOrder.SymbolId, cTrustOrder.Action, (bDelete) ? cTrustOrder.Contracts : iTrustCount, cTrustOrder.Price, (bDelete) ? cTrust.備註 : (iTrustCount == 0) ? __sFullDeal : cTrustOrder.Name, cTrustOrder.Time.ToString("yyyy-MM-dd HH:mm:ss"));
							}
						}
						break;
					case 2: //成交回報
						OrderDeal cDeal = cToken["Report"].ToObject<OrderDeal>();
						string sDealId = cDeal.成交書號;  //成交書號 == 委託書號

						int iDealLots = cDeal.成交數量;
						TradeOrder cTempTrust = __cEntrusts.GetTrade(sDealId);
						if (cTempTrust != null && cTempTrust.Contracts >= iDealLots) {  //檢查委託陣列內是否有相同的成交書號(委託書號)
							cTempTrust.Contracts -= iDealLots;
							bool bDealed = cTempTrust.Contracts == 0;
							if (bDealed) {
								OnResponse(cTempTrust, cTempTrust.SymbolId, ResponseType.Trust);
								if (logger.IsInfoEnabled) logger.InfoFormat("[Trust] #{0} {1} {2} {3} at {4} {5} @{6}", cTempTrust.Ticket, cTempTrust.SymbolId, cTempTrust.Action, 0, cTempTrust.Price, __sFullDeal, cTempTrust.Time.ToString("yyyy-MM-dd HH:mm:ss"));
							}

							TradeOrder cDealOrder = cTempTrust.Clone();
							cDealOrder.IsCancel = false;
							cDealOrder.IsDealed = bDealed;
							cDealOrder.Time = cDeal.成交時間;
							cDealOrder.Fee = cDeal.手續費;
							cDealOrder.Tax = cDeal.交易稅;
							cDealOrder.Contracts = iDealLots;
							cDealOrder.Price = Math.Round(cDeal.成交價格, __iDecimalPoint);
							
							double[] dValues = CalculateCommissions(cDealOrder);  //計算其他佣金與手續費(通常真實交易會回報交易稅與眷商手續費, 這裡還可以計算其他的附加費用或是附加的手續費)
							cDealOrder.OtherFees = dValues[0];
							cDealOrder.Fee += dValues[1];
							if (logger.IsInfoEnabled) logger.InfoFormat("[Deal] #{0} {1} {2} {3} at {4} {5} @{6}", cDealOrder.Ticket, cDealOrder.SymbolId, cDealOrder.Action, cDealOrder.Contracts, cDealOrder.Price, cDealOrder.Name, cDealOrder.Time.ToString("yyyy-MM-dd HH:mm:ss"));
							
							lock (__cDeals) {
								__cDeals.Enqueue(cDealOrder);
							}
							cTempTrust.IsDealed = true;
						}
						break;
				}
			}
		}
	}
} //678行