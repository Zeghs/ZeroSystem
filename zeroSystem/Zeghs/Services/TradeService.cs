using System;
using System.Timers;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Orders;

namespace Zeghs.Services {
	internal sealed class TradeService : IDisposable {
		internal event EventHandler onUpdate = null;

		private bool __bBusy = false;
		private bool __bDisposed = false;
		private int __iCloseTradesIndex = 0;
		private Timer __cTimer = null;
		private TradeBoundList __cOpens = null;
		private TradeBoundList __cTrusts = null;
		private Queue<ResponseEvent> __cQueue = null;
		private SimpleBoundList<_TradeInfo> __cCloses = null;

		private object __oLock = new object();

		internal SimpleBoundList<_TradeInfo> Closes {
			get {
				return __cCloses;
			}
		}

		internal TradeBoundList Opens {
			get {
				return __cOpens;
			}
		}

		internal TradeBoundList Trusts {
			get {
				return __cTrusts;
			}
		}

		internal TradeService() {
			__cQueue = new Queue<ResponseEvent>(64);
			__cCloses = new SimpleBoundList<_TradeInfo>(512);
			__cOpens = new TradeBoundList(64);
			__cTrusts = new TradeBoundList(32);

			__cTimer = new Timer();
			__cTimer.Elapsed += Timer_onElapsed;
			__cTimer.AutoReset = false;
			__cTimer.Interval = 500;
		}

		internal void AddResponse(ResponseEvent response) {
			lock (__cQueue) {
				__cQueue.Enqueue(response);
			}
			__cTimer.Start();
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onUpdate = null;

					__cTimer.Dispose();

					__cQueue.Clear();
					__cOpens.Clear();
					__cCloses.Clear();
					__cTrusts.Clear();
				}
			}
		}
		
		private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bBusy;
				if (!bBusy) {
					__bBusy = true;
				}
			}

			if (!bBusy) {
				bool bUpdate = __cQueue.Count > 0;
				while (__cQueue.Count > 0) {
					ResponseEvent cResponse = null;
					lock (__cQueue) {
						cResponse = __cQueue.Dequeue();
					}

					ResponseType cType = cResponse.ResponseType;
					if (cType == ResponseType.Update) {
						continue;
					}

					string sSymbolId = cResponse.SymbolId;
					ITradeOrder cOrder = cResponse.TradeOrder;
					string sTicket = cOrder.Ticket;
					switch (cType) {
						case ResponseType.Cancel:  //取消委託
						case ResponseType.Trust:  //委託回報
							if (cType == ResponseType.Cancel) {
								__cTrusts.Remove(sTicket);
							} else {
								if (cOrder.Contracts == 0) {
									__cTrusts.Remove(sTicket);
								} else {
									__cTrusts.Add(new _TradeInfo(cOrder, sSymbolId));
								}
							}
							break;
						case ResponseType.Deal:  //成交回報
							EOrderAction cAction = cOrder.Action;
							if (cAction == EOrderAction.Buy || cAction == EOrderAction.SellShort) {
								TradeList<ITrade> cOpenTrades = cResponse.OpenTrades;
								if (cOpenTrades != null && cOpenTrades.Count > 0) {
									ITrade cTrade = cOpenTrades.GetTrade(sTicket);
									if (cTrade != null) {
										__cOpens.Add(new _TradeInfo(cTrade, sSymbolId));
									}
								}
							} else {
								List<ITrade> cCloseTrades = cResponse.CloseTrades;
								int iCount = cCloseTrades.Count;
								if (iCount > 0) {
									for (int i = __iCloseTradesIndex; i < iCount; i++) {
										ITrade cTrade = cCloseTrades[i];
										ITradeOrder cExitOrder = cTrade.ExitOrder;

										__cCloses.Add(new _TradeInfo(cTrade.EntryOrder, sSymbolId, 0));
										__cCloses.Add(new _TradeInfo(cExitOrder, sSymbolId, cTrade.Profit));

										if (cOrder != cExitOrder) {  //如果參考不同(表示經過 Clone 後, 修正下單數量並平倉, 所以需要移除目前開倉資訊)
											__cOpens.Remove(cTrade.Ticket);
										}
									}
									__iCloseTradesIndex = (cResponse.OpenTrades == null) ? 0 : iCount;
								}
							}
							break;
					}
				}

				if (bUpdate) {
					onUpdate(this, EventArgs.Empty);
				}

				lock (__oLock) {
					__bBusy = false;
				}
			}
		}
	}
}