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
using Zeghs.Rules;
using Zeghs.Events;
using Zeghs.Orders;
using Zeghs.Products;
using Zeghs.Services;
using Netwings.Rules;
using Netwings.Event;
using Netwings.Orders;

namespace Netwings {
	public sealed class SimulateOrderService : AbstractOrderService, IOrderSender, IOrderEntrust {
		private static readonly ILog logger = LogManager.GetLogger(typeof(SimulateOrderService));
		private static int __iDealIndex = 0;
		private static int __iTrustIndex = 0;
		private static Dictionary<ERuleType, List<RulePropertyAttribute>> __cRuleItems = null;

		private int __iPrevious = 0;
		private int __iDecimalPoint = 0;  //小數點位數
		private int __iUsedClosedTempLots = 0;  //目前已使用暫時平倉量
		private bool __bBusy = false;
		private bool __bDisposed = false;
		private PositionSeries __cPositions = null;
		private Queue<string> __cReserves = null;
		private Queue<TradeOrder> __cDeals = null;
		private TradeList<TradeOrder> __cEntrusts = null;
		private MarketPosition __cCurrentPosition = null;
		private HashSet<string> __cNextBarRequires = null;  //存放 NextBar 下單的判斷依據(當要下 NextBar 時會將 name 存入此結構內, 如果還沒到下一根 Bars 就會清除, 並等待下一次又觸發條件時再存入)
		private HashSet<string> __cPNextBarRequires = null; //存放上一個 __cNextBarRequires 的變化, 因為 NextBar 是在下一根下單, 所以要提供的是上一根的最後狀態(提供給下 NextBars 的方法一個是否要下單的依據)
		private AbstractProductProperty __cProperty = null;
		private object __oLock = new object();  //lock 專用的變數

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

		TradeList<TradeOrder> IOrderEntrust.Entrusts {
			get {
				return __cEntrusts;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public SimulateOrderService() {
			__cReserves = new Queue<string>(16);
			__cDeals = new Queue<TradeOrder>(16);
			__cEntrusts = new TradeList<TradeOrder>(64);

			__cNextBarRequires = new HashSet<string>();
			__cPNextBarRequires = new HashSet<string>();

			__cCurrentPosition = new MarketPosition(16);  //建立目前留倉部位
			__cPositions = new PositionSeries();          //建立留倉部位陣列
			__cPositions.Value = __cCurrentPosition;
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
			if (Bars.CurrentBar > __iPrevious) {
				SendNextBars(__iPrevious);  //處理 NextBars 下單程序
				__iPrevious = Bars.CurrentBar;
			}

			HashSet<string> cSwap = __cPNextBarRequires;  //將上一個條件狀態保留起來
			__cPNextBarRequires = __cNextBarRequires;  //將目前條件狀態置換到上一個條件狀態變數內
			__cNextBarRequires = cSwap;  //將上一個狀態置換到目前狀態上(減少使用 new 而造成 GC 負擔)
			__cNextBarRequires.Clear();  //清除狀態並等待下一輪 CalcBar 時儲存新條件狀態

			if (__cEntrusts.Count > 0) {  //如果有委託單尚未成交(就可能是限價委託單)
				List<TradeOrder> cTrades = __cEntrusts.Trades;
				
				int iCount = cTrades.Count;
				for (int i = iCount - 1; i >= 0; i--) {
					TradeOrder cTrust = cTrades[i];
					if (!cTrust.IsDealed && cTrust.Price > 0) {  //市價單 = 0(只有限價單有價格)
						SendLimit(cTrust);
					}
				}
			}

			CalculatePositions();  //計算成交部位狀況
			AsyncCalculateProfits();  //計算留倉部位損益
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
		public bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false) {
			limitPrice = Math.Round(limitPrice, __iDecimalPoint);
			TradeOrder cTrust = __cEntrusts.GetTradeFromName(name);
			if (cTrust != null) {
				if (openNextBar) {
					__cNextBarRequires.Add(name);  //標記 NextBar 時, 可以下單
					return true;
				} else {
					if (cTrust.Price == limitPrice) {  //委託價格一樣就忽略
						return false;
					} else {
						if (cTrust.IsTrusted && !cTrust.IsDealed) {  //如果已經委託完成且尚未成交就取消單號
							this.SendCancel(cTrust);
						} else {  //如果還沒有委託成功
							return false;  //直接離開(可能需要等到委託成功之後才能處理)
						}
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
			cOrder.SymbolId = Bars.Request.Symbol;
			cOrder.Time = Bars.Time.Value;
			cOrder.Ticket = (openNextBar) ? name : GetTrustID();
			__cEntrusts.Add(cOrder);  //加入至委託列表內

			if (openNextBar) {  //如果需要在下一根 Bars 下單, 就先保留 name 在佇列, 以方便比對委託單
				__cReserves.Enqueue(name);
				__cNextBarRequires.Add(name);
			} else {
				if (isReverse) {  //如果有反轉倉單
					CancelLimit(action);  //取消所有反向之前的限價委託單
				}
				
				this.SendTrust(cOrder);  //傳送新委託單(模擬成交)
			}
			return true;
		}

		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="trust">交易訂單資訊</param>
		/// <param name="isCancel">是否要取消此交易訂單(成功委託才能取消訂單)</param>
		/// <returns>返回值: true=成功, false=失敗</returns>
		bool IOrderSender.Send(TradeOrder trust, bool isCancel) {
			if (isCancel) {
				this.SendCancel(trust);
			} else {
				this.SendTrust(trust, Bars.Close.Value);
			}
			return true;
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

			__cProperty = (Bars.Info as InstrumentSettings).Property;  //取得商品屬性
			__cCurrentPosition.SetBigPointValue(__cProperty.BigPointValue);  //設定每一大點的交易金額
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

					__cReserves.Clear();
					__cPositions.Dispose();
					__cCurrentPosition.Clear();
				}
			}
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

				string sTrustID = cDeal.Ticket;
				cDeal.Ticket = GetDealID();  //填入成交單號(自動編號)
				if (logger.IsInfoEnabled) logger.InfoFormat("[Deal] #{0} {1} {2} {3} at {4} {5} @{6}", cDeal.Ticket, cDeal.SymbolId, cDeal.Action, cDeal.Contracts, cDeal.Price, cDeal.Name, cDeal.Time.ToString("yyyy-MM-dd HH:mm:ss"));

				bool bClosed = __cCurrentPosition.CheckPosition(cDeal);  //檢查是否已經平倉完畢
				int iLatestHistoryCount = __cCurrentPosition.LatestHistoryCount;  //取得最新新增的平倉歷史明細個數
				if (bClosed) {  //如果已經平倉完畢
					__cCurrentPosition = new MarketPosition(16);  //重新建立新的留倉部位
					__cCurrentPosition.SetBigPointValue(Bars.Info.BigPointValue);  //設定每一大點的交易金額

					++__cPositions.Current;  //移動目前留倉部位陣列的目前索引值
					__cPositions.Value = __cCurrentPosition;  //指定新的留倉部位至留倉陣列
				}

				if (cDeal.IsDealed) {  //檢查是否戳合完成
					__cEntrusts.Remove(sTrustID);
				}
				OnResponse(cDeal, cDeal.SymbolId, ResponseType.Deal, (bClosed) ? null : __cCurrentPosition, (bClosed) ? __cPositions[1].ClosedTrades : __cCurrentPosition.ClosedTrades, iLatestHistoryCount);
			}

			Interlocked.Exchange(ref __iUsedClosedTempLots, 0);  //處理成交單完畢之後將暫存平倉量歸0(這樣平倉單才能在下單, [開倉量 - 暫存平倉量 >= 下單平倉量] 此單才會被接受並成交)
			if (__cCurrentPosition.OpenLots > 0) {  //檢查是否有開倉(有開倉就要發送這個更新事件, 這樣損益才會被更新)
				OnResponse(null, Bars.Request.Symbol, ResponseType.Update);
			}
		}

		private void CancelLimit(EOrderAction action) {
			if (__cEntrusts.Count > 0) {  //如果有委託單尚未成交(就可能是限價委託單)
				List<TradeOrder> cTrades = __cEntrusts.Trades;

				int iCount = cTrades.Count;
				if (iCount > 0) {
					EOrderAction cAction1 = EOrderAction.Buy, cAction2 = EOrderAction.Sell;
					if (action == EOrderAction.SellShort || action == EOrderAction.BuyToCover) { 
						cAction1 = EOrderAction.SellShort;
						cAction2 = EOrderAction.BuyToCover;
					}

					for (int i = iCount - 1; i >= 0; i--) {  //由下往上檢查(才不會因為 Remove 出現問題)
						TradeOrder cTrust = cTrades[i];
						if (!cTrust.IsDealed && cTrust.Price > 0) {  //尚未成交而且是限價單就處理
							EOrderAction cAction = cTrust.Action;
							if (cAction == cAction1 || cAction == cAction2) {
								this.SendCancel(cTrust);
							}
						}
					}
				}
			}
		}

		private string GetDealID() {
			int iID = Interlocked.Increment(ref __iDealIndex);
			return iID.ToString();
		}

		private string GetTrustID() {
			int iID = Interlocked.Increment(ref __iTrustIndex);
			return iID.ToString();
		}

		private void OrderDeal(TradeOrder trust, double dealPrice = 0) {
			EOrderAction cAction = trust.Action;
			if (cAction == EOrderAction.Sell || cAction == EOrderAction.BuyToCover) {
				int iLots = __cCurrentPosition.OpenLots - __iUsedClosedTempLots;  //檢查平倉數量是否足夠
				if (trust.Contracts > iLots) {   //平倉數量不足(不下單)
					this.SendCancel(trust);  //取消此張平倉單
					return;
				} else {
					Interlocked.Add(ref __iUsedClosedTempLots, trust.Contracts);  //將此平倉量累加至暫存平倉量變數
				}
			}

			if (!trust.IsDealed) {
				trust.IsDealed = true;

				TradeOrder cDeal = trust.Clone();
				cDeal.Price = Math.Round((dealPrice == 0) ? Bars.Close.Value : dealPrice, __iDecimalPoint);
				cDeal.Time = Bars.Time.Value;
				SetTax(cDeal);

				double[] dValues = CalculateCommissions(cDeal);  //計算交易佣金與手續費用(由策略使用者自行決定的佣金與手續費設定所計算出來的價格)
				cDeal.OtherFees = dValues[0];
				cDeal.Fee = dValues[1];

				lock (__cDeals) {
					__cDeals.Enqueue(cDeal);
				}

				trust.Contracts = 0;
				OnResponse(trust, trust.SymbolId, ResponseType.Trust);
			}
		}

		private void SendCancel(TradeOrder trust) {
			if (trust.IsTrusted && !trust.IsDealed) {  //有委託狀態且尚未成交才可以取消訂單
				__cEntrusts.Remove(trust.Ticket);
				if (logger.IsInfoEnabled) logger.InfoFormat("[Cancel] #{0} {1} {2} {3} at {4} {5} @{6}", trust.Ticket, trust.SymbolId, trust.Action, trust.Contracts, trust.Price, trust.Name, trust.Time.ToString("yyyy-MM-dd HH:mm:ss"));
				OnResponse(trust, trust.SymbolId, ResponseType.Cancel);
			}
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
						__cEntrusts.Add(cTrust);  //加入至委託列表內

						if (cTrust.IsReverse) {  //如果有反轉倉單
							CancelLimit(cTrust.Action);  //取消所有反向之前的限價委託單
						}
						
						this.SendTrust(cTrust, Bars.Open.Value);  //發送委託單
					}
				}
			}
		}

		private void SendLimit(TradeOrder trust) {
			double dPrice = trust.Price;
			double dClosePrice = Bars.Close.Value;
			double dDealPrice = (dPrice >= Bars.Low.Value && dPrice <= Bars.High.Value) ? dPrice : ((trust.Action == EOrderAction.Buy || trust.Action == EOrderAction.BuyToCover) && dPrice >= dClosePrice) ? dClosePrice : ((trust.Action == EOrderAction.SellShort || trust.Action == EOrderAction.Sell) && dPrice <= dClosePrice) ? dClosePrice : 0;
			if (dDealPrice == 0) {
				OnResponse(trust, trust.SymbolId, ResponseType.Trust);
			} else {
				OrderDeal(trust, dDealPrice);
			}
		}

		private void SendTrust(TradeOrder trust, double dealPrice = 0) {
			trust.IsTrusted = true;
			if (logger.IsInfoEnabled) logger.InfoFormat("[Trust] #{0} {1} {2} {3} at {4} {5} @{6}", trust.Ticket, trust.SymbolId, trust.Action, trust.Contracts, trust.Price, trust.Name, trust.Time.ToString("yyyy-MM-dd HH:mm:ss"));

			if (trust.Price == 0) {  //市價成交
				OrderDeal(trust, dealPrice);
			} else {
				SendLimit(trust);
			}
		}

		private void SetTax(TradeOrder deal) {
			double dTotals = deal.Price * __cProperty.BigPointValue * deal.Contracts;
			ITax cTax = __cProperty.TaxRule as ITax;
			deal.Tax = cTax.GetTax(deal.Action, dTotals);
		}
	}
} //478行