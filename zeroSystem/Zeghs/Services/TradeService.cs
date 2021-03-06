﻿using System;
using System.Timers;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Orders;

namespace Zeghs.Services {
	internal sealed class TradeService : IDisposable {
		internal event EventHandler onUpdate = null;

		private bool __bDisposed = false;
		private bool __bTradeBusy = false;
		private bool __bUpdateBusy = false;
		private Timer __cTradeTimer = null;
		private Timer __cUpdateTimer = null;
		private TradeBoundList __cOpens = null;
		private TradeBoundList __cTrusts = null;
		private Queue<ResponseEvent> __cQueue = null;
		private HistoryBoundList __cCloses = null;

		private object __oLock = new object();

		internal HistoryBoundList Closes {
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
			__cCloses = new HistoryBoundList(512);
			__cOpens = new TradeBoundList(64, true);
			__cTrusts = new TradeBoundList(32);

			__cTradeTimer = new Timer();
			__cTradeTimer.Elapsed += TradeTimer_onElapsed;
			__cTradeTimer.AutoReset = false;
			__cTradeTimer.Interval = 500;
			
			__cUpdateTimer = new Timer();
			__cUpdateTimer.Elapsed += UpdateTimer_onElapsed;
			__cUpdateTimer.AutoReset = false;
			__cUpdateTimer.Interval = 500;
		}

		internal void AddResponse(ResponseEvent response) {
			lock (__cQueue) {
				__cQueue.Enqueue(response);
			}
			
			__cTradeTimer.Start();
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

					__cTradeTimer.Dispose();
					__cUpdateTimer.Dispose();

					__cQueue.Clear();
					__cOpens.Clear();
					__cCloses.Clear();
					__cTrusts.Clear();
				}
			}
		}
		
		private void TradeTimer_onElapsed(object sender, ElapsedEventArgs e) {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bTradeBusy;
				if (!bBusy) {
					__bTradeBusy = true;
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
								int iHistoryCount = cResponse.LatestHistoryCount;
								if (iHistoryCount > 0) {
									int iHistoryIndex = cResponse.LatestHistoryIndex;
									if (iHistoryIndex > -1) {
										for (int i = 0; i < iHistoryCount; i++) {
											ITrade cTrade = cCloseTrades[iHistoryIndex + i];
											__cCloses.Add(new _TradeInfo(cTrade.EntryOrder, sSymbolId, 0));
											__cCloses.Add(new _TradeInfo(cTrade.ExitOrder, sSymbolId, cTrade.Profit));

											if (cTrade.IsTradeClosed) {
												__cOpens.Remove(cTrade.Ticket);
											}
										}
									}
								}
							}
							break;
					}
				}

				if (bUpdate) {
					__cUpdateTimer.Start();
					onUpdate(this, EventArgs.Empty);
				}

				lock (__oLock) {
					__bTradeBusy = false;
				}
			}
		}
		
		private void UpdateTimer_onElapsed(object sender, ElapsedEventArgs e) {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bUpdateBusy;
				if (!bBusy) {
					__bUpdateBusy = true;
				}
			}

			if (!bBusy) {
				__cOpens.SubTotal();
				onUpdate(this, EventArgs.Empty);

				lock (__oLock) {
					__bUpdateBusy = false;
				}
			}
		}
	}
}