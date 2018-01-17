using System;
using System.Threading;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Orders;

namespace Zeghs.Services {
	/// <summary>
	///   下單服務類別(所有下單模組都必須繼承此服務類別)
	/// </summary>
	public abstract class AbstractOrderService : IOrderCreator, IDisposable {
		private static int __iOrderId = 0;

		/// <summary>
		///   收到委託或是成交回報時需要觸發的事件
		/// </summary>
		public event EventHandler<ResponseEvent> onResponse = null;

		private int __iDataStream = 0;
		private bool __bDisposed = false;
		private List<ICommission> __cCommissions = null;
		private Contracts __cDefaultContracts = Contracts.Default; //預設的下單數量

		/// <summary>
		///   [取得] 主要的 Bars 物件
		/// </summary>
		public Instrument Bars {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 佣金規則列表
		/// </summary>
		public List<ICommission> Commissions {
			get {
				return __cCommissions;
			}
		}

		/// <summary>
		///   [取得] 目前留倉部位
		/// </summary>
		public abstract IMarketPosition CurrentPosition {
			get;
		}

		/// <summary>
		///   [取得] Bars 物件的資料串流編號
		/// </summary>
		public int DataStream {
			get {
				return __iDataStream;
			}
		}

		/// <summary>
		///   [取得] 留倉部位序列資訊
		/// </summary>
		public abstract PositionSeries Positions {
			get;
		}

		internal Contracts DefaultContracts {
			get {
				return __cDefaultContracts;
			}
		}

		/// <summary>
		///   建立佣金規則介面
		/// </summary>
		/// <param name="rule">基礎規則類別</param>
		/// <returns>返回值: ICommission 介面</returns>
		public abstract ICommission CreateCommission(RuleBase rule);

		/// <summary>
		///   下單服務初始化(初始化會在參數設定之後呼叫)
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		///   取得可用的規則項目列表
		/// </summary>
		/// <param name="ruleType">規則型態(由型態區分規則項目)</param>
		/// <returns>返回值:規則項目列表</returns>
		public abstract List<RulePropertyAttribute> GetRuleItems(ERuleType ruleType);

		/// <summary>
		///   讀取下單服務的參數
		/// </summary>
		public abstract void Load();

		/// <summary>
		///   下單元件需要工作的使用者邏輯須在此實作(每次計算 Bars 之後會執行一次此方法)
		/// </summary>
		public abstract void OnWork();

		/// <summary>
		///   寫入下單服務的參數
		/// </summary>
		public abstract void Save();

		/// <summary>
		///   釋放報價服務的所有資源
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   建立限價買賣模式(送出使用者指定的價格的委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced介面</returns>
		public IOrderPriced Limit(SOrderParameters orderParams) {
			return new OrderPriced(this, orderParams);
		}

		/// <summary>
		///   建立市價買賣模式(在下一根 Bars 建立之後以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public IOrderMarket MarketNextBar(SOrderParameters orderParams) {
			return new OrderMarket(this, orderParams, true);
		}

		/// <summary>
		///   建立市價買賣模式(立即以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public IOrderMarket MarketThisBar(SOrderParameters orderParams) {
			return new OrderMarket(this, orderParams, false);
		}

		/// <summary>
		///   設定預設的下單數量規模
		/// </summary>
		/// <param name="lots">下單數量</param>
		public void SetDefaultContracts(int lots) {
			__cDefaultContracts = Contracts.CreateUserSpecified(lots);
		}

		/// <summary>
		///   設定佣金規則列表
		/// </summary>
		/// <param name="commissions">佣金規則列表</param>
		public void SetCommissions(List<ICommission> commissions) {
			if (__cCommissions != null) {
				__cCommissions.Clear();
			}
			__cCommissions = commissions;
		}

		/// <summary>
		///   設定 Instrument 資訊
		/// </summary>
		/// <param name="bars">Instrument 類別</param>
		/// <param name="data_stream">資料串流編號</param>
		public virtual void SetInstrument(Instrument bars, int data_stream) {
			this.Bars = bars;
			this.__iDataStream = data_stream;
		}

		/// <summary>
		///   建立停損模式(觸發到停損點後，以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderPriced介面</returns>
		public IOrderPriced Stop(SOrderParameters orderParams) {
			return null;
		}

		/// <summary>
		///   建立停損限價單模式(觸發到停損點後，以使用者指定的價格送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderStopLimit介面</returns>
		public IOrderStopLimit StopLimit(SOrderParameters orderParams) {
			return null;
		}

		internal int GetOrderID() {
			return Interlocked.Increment(ref __iOrderId);
		}

		/// <summary>
		///   計算所有傭金規則
		/// </summary>
		/// <param name="order">ITradeOrder 介面</param>
		/// <returns>返回值: double[](0=佣金, 1=手續費)</returns>
		protected double[] CalculateCommissions(ITradeOrder order) {
			double[] dValues = new double[2]; 
			if (__cCommissions != null) {
				int iCount = __cCommissions.Count;
				for (int i = 0; i < iCount; i++) {
					ICommission cCommission = __cCommissions[i];
					int iIndex = ((int) cCommission.RuleType) - 128;  //佣金起始為 128(減去 128 可得到陣列的索引值)
					dValues[iIndex] += cCommission.Calculate(order.Price, order.Contracts);
				}
			}
			return dValues;
		}

		/// <summary>
		///   釋放下單服務的所有資源(繼承後可複寫方法)
		/// </summary>
		/// <param name="disposing">是否處理受託管的資源</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					onResponse = null;

					if (__cCommissions != null) {
						__cCommissions.Clear();
					}
				}
			}
		}

		/// <summary>
		///   發送回報事件
		/// </summary>
		/// <param name="order">下單資訊</param>
		/// <param name="symbolId">商品代號</param>
		/// <param name="type">回報類型</param>
		/// <param name="openTrades">開倉交易單列表</param>
		/// <param name="closeTrades">已平倉交易單列表</param>
		protected void OnResponse(ITradeOrder order, string symbolId, ResponseType type, TradeList<ITrade> openTrades = null, List<ITrade> closeTrades = null) {
			if (onResponse != null) {
				onResponse(this, new ResponseEvent(order, symbolId, type, openTrades, closeTrades));
			}
		}
	}
}  //242行