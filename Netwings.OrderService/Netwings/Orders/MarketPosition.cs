using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Orders;

namespace Netwings.Orders {
	/// <summary>
	///   市場留倉部位類別
	/// </summary>
	internal sealed class MarketPosition : TradeList<ITrade>, IMarketPosition {
		private int __iHistoryLots = 0;
		private int __iLatestHistoryCount = 0;
		private double __dBigPointValue = 1;
		private double __dHistoryProfit = 0;
		private List<ITrade> __cHistorys = null;
		private Dictionary<int, Queue<Trade>> __cPositions = null;

		/// <summary>
		///   [取得] 歷史交易明細
		/// </summary>
		public List<ITrade> ClosedTrades {
			get {
				return __cHistorys;
			}
		}

		/// <summary>
		///   [取得] 帳上最大虧損
		/// </summary>
		public double MaxDrawDown {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 帳上最大獲利
		/// </summary>
		public double MaxRunUp {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 開倉單總數量
		/// </summary>
		public int OpenLots {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 開倉單總損益
		/// </summary>
		public double OpenProfit {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 留倉交易明細
		/// </summary>
		public List<ITrade> OpenTrades {
			get {
				return this.Trades;
			}
		}

		/// <summary>
		///   [取得] 已平倉的總損益
		/// </summary>
		public double Profit {
			get {
				return __dHistoryProfit;
			}
		}

		/// <summary>
		///   [取得] 目前未平倉單的平均損益
		/// </summary>
		public double ProfitPerContract {
			get {
				return (this.OpenLots == 0) ? 0 : this.OpenProfit / this.OpenLots;
			}
		}

		/// <summary>
		///   [取得] 目前倉位的方向
		/// </summary>
		public EMarketPositionSide Side {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 總下單數量
		/// </summary>
		public int Value {
			get {
				return __iHistoryLots + this.OpenLots;
			}
		}

		internal int LatestHistoryCount {
			get {
				return __iLatestHistoryCount;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="capacity">擴充的容量大小</param>
		internal MarketPosition(int capacity) : base(capacity) {
			__cHistorys = new List<ITrade>(capacity);
			__cPositions = new Dictionary<int, Queue<Trade>>(4);
		}

		public override void Clear() {
			lock (__cHistorys) {
				__cHistorys.Clear();
			}

			ClearPosition();
			base.Clear();
		}

		internal void CalculateProfits(double price) {
			if (price == 0) {
				return;
			}

			lock (__cPositions) {
				this.OpenProfit = 0;

				int iCount = this.Count;
				for (int i = 0; i < iCount; i++) {
					Trade cTrade = this[i] as Trade;
					cTrade.CalculateProfit(price, __dBigPointValue);
					this.OpenProfit += cTrade.Profit;
				}
			}
		}

		internal bool CheckPosition(TradeOrder deal) {
			bool bClosed = false;
			EOrderAction cAction = deal.Action;
			int iSide = (int) this.Side;
			int iBSFlag = (cAction == EOrderAction.Buy || cAction == EOrderAction.BuyToCover) ? -1 : 1;

			Trade cTrade = new Trade();
			cTrade._EntryOrder = deal.Clone();

			if (iSide == iBSFlag) {
				Queue<Trade> cQueue = null;
				lock (__cPositions) {
					__cPositions.TryGetValue(iBSFlag, out cQueue);
				}

				int iPrevCount = __cHistorys.Count;  //保存目前歷史明細個數
				CalculatePosition(cQueue, cTrade);
				__iLatestHistoryCount = __cHistorys.Count - iPrevCount;  //計算出新增的歷史明細個數

				if (this.OpenLots == 0) {
					bClosed = true;
				}
			} else {
				AddPosition(cTrade);
			}
			return bClosed;
		}

		internal void SetBigPointValue(double bigPointValue) {
			__dBigPointValue = bigPointValue;
		}

		private void AddPosition(Trade trade) {
			EOrderAction cAction = trade.EntryOrder.Action;
			int iBSFlag = (cAction == EOrderAction.Buy || cAction == EOrderAction.BuyToCover) ? 1 : -1;

			Queue<Trade> cQueue = null;
			lock (__cPositions) {
				if (!__cPositions.TryGetValue(iBSFlag, out cQueue)) {
					this.Side = (iBSFlag == 1) ? EMarketPositionSide.Long : (iBSFlag == -1) ? EMarketPositionSide.Short : EMarketPositionSide.Flat;

					cQueue = new Queue<Trade>(16);
					__cPositions.Add(iBSFlag, cQueue);
				}
			}

			lock (cQueue) {
				cQueue.Enqueue(trade);
				this.OpenLots += trade._EntryOrder.Contracts;
			}
			this.Add(trade);
		}

		/// <summary>
		///   計算倉位
		/// </summary>
		/// <param name="queue">儲存留倉資訊的 Queue</param>
		/// <param name="trade">交易資訊</param>
		private void CalculatePosition(Queue<Trade> queue, Trade trade) {
			lock (queue) {
				TradeOrder cTargetOrder = trade._EntryOrder;
				while (queue.Count > 0 && cTargetOrder.Contracts > 0) {
					Trade cTrade = queue.Peek();
					TradeOrder cTradeOrder = cTrade._EntryOrder;

					if (cTradeOrder.Contracts > cTargetOrder.Contracts) {  //如果留倉單數量大於平倉單成交數量
						trade._ExitOrder = cTargetOrder;  //將平倉單直接放入 _ExitOrder
						trade._EntryOrder = SplitOrder(cTradeOrder, cTargetOrder.Contracts);  //從留倉單 Clone 後拆出與平倉單相同的數量, 放入 _EntryOrder
						trade.CalculateProfit(__dBigPointValue);  //計算利潤

						lock (__cHistorys) {
							__cHistorys.Add(trade);  //寫入歷史資訊列表
						}

						SetParameters(trade);  //計算內部其他參數
						break;  //直接離開(因為已經從倉部位扣掉平倉數量, 所以直接離開)
					} else {  //如果留倉單數量小於或等於平倉單數量
						cTrade.IsTradeClosed = true;
						cTrade._ExitOrder = SplitOrder(cTargetOrder, cTradeOrder.Contracts);  //將平倉單 Clone 後拆出與留倉單相同數量, 放入 _ExitOrder
						cTrade.CalculateProfit(__dBigPointValue);  //計算利潤

						lock (__cHistorys) {
							__cHistorys.Add(cTrade);  //寫入歷史資訊列表
						}

						SetParameters(cTrade);  //計算內部其他參數
						queue.Dequeue();  //將此筆留倉單從 queue 內移除(表示此筆留倉單已經被沖銷)
						this.Remove(cTradeOrder.Ticket);  //被沖銷的留倉單就從倉部位內移除掉
					}
				}
			}
		}

		private void ClearPosition() {
			lock (__cPositions) {
				foreach (Queue<Trade> cQueue in __cPositions.Values) {
					cQueue.Clear();
				}
				
				__cPositions.Clear();
			}
		}

		private void SetParameters(Trade trade) {
			double dProfit = trade.Profit;
			if (this.MaxDrawDown > dProfit) {
				this.MaxDrawDown = dProfit;
			}

			if (this.MaxRunUp < dProfit) {
				this.MaxRunUp = dProfit;
			}

			int iLots = trade._ExitOrder.Contracts;
			this.OpenLots -= iLots;
			__iHistoryLots += iLots;
			__dHistoryProfit += dProfit;
		}

		private TradeOrder SplitOrder(TradeOrder source, int splitContracts) {
			TradeOrder cTradeOrder = source.Clone();  //將原始下單資訊 Clone 一份
			cTradeOrder.Contracts = splitContracts;  //修改成要分拆的數量

			int iContracts = source.Contracts;
			if (iContracts > 1) {  //如果原始的量超過 1 就計算手續費跟交易稅的平均值(有可能成交數量 > 1, 手續費與交易稅會合併計算)
				cTradeOrder.Fee = source.Fee / iContracts * splitContracts;
				cTradeOrder.Tax = source.Tax / iContracts * splitContracts;
				source.Fee -= cTradeOrder.Fee;
				source.Tax -= cTradeOrder.Tax;
			}
			
			source.Contracts -= splitContracts;  //分拆後的數量要從原始的下單資訊內扣除
			return cTradeOrder;  //回傳分拆後的下單資訊
		}
	}
}