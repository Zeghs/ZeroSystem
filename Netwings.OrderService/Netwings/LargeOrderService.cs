using System;
using System.Timers;
using System.Collections.Generic;
using log4net;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Orders;
using Netwings.Orders;

namespace Netwings {
	public sealed class LargeOrderService : RealOrderService {
		private const int TIMER_INTERVAL = 1000;  //計時器基本觸發周期

		private int __iTimeCount = 0;
		private bool __bDisposed = false;
		private System.Timers.Timer __cTimer = null;
		private Dictionary<string, TradeOrder> __cTrades = null;

		private object __oLock = new object();
		private object __oTimerLock = new object();

		/// <summary>
		///   [取得/設定] 重新送出交易單的時間間隔(單位:毫秒)
		/// </summary>
		[Input("重新送出交易單的時間間隔(單位:毫秒)")]
		private int RequestOrderInterval {
			get;
			set;
		}

		public LargeOrderService() {
			this.RequestOrderInterval = 5000;
			this.onResponse += LargeOrderService_onResponse;
			
			__cTrades = new Dictionary<string, TradeOrder>(64);

			__cTimer = new System.Timers.Timer();
			__cTimer.AutoReset = false;
			__cTimer.Interval = TIMER_INTERVAL;
			__cTimer.Elapsed += Timer_onElapsed;
			__cTimer.Start();
		}

		public override bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false) {
			bool bRet = false;
			lock (__oLock) {
				bRet = base.Send(action, category, (category == OrderCategory.Market) ? AdjustPrice(action) : limitPrice, lots, isReverse, touchPrice, name, openNextBar);
			}
			
			return bRet;
		}

		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					base.Dispose(disposing);

					lock (__oTimerLock) {
						__cTimer.Dispose();
					}
				}
			}
		}

		private double AdjustPrice(EOrderAction action) {
			IDOMData cDOM = Bars.DOM;
			double dASK = (cDOM == null) ? 0 : cDOM.Ask[0].Price;
			double dBID = (cDOM == null) ? 0 : cDOM.Bid[0].Price;

			return (action == EOrderAction.Buy || action == EOrderAction.BuyToCover) ? dASK : (action == EOrderAction.Sell || action == EOrderAction.SellShort) ? dBID : 0;
		}

		private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
			__iTimeCount += TIMER_INTERVAL;  //累加時間個數(如果達到 RequestOrderInterval 設定的值才處理)
			if (__iTimeCount >= this.RequestOrderInterval) {
				if (__cTrades.Count > 0) {
					lock (__cTrades) {
						foreach (TradeOrder cOrder in __cTrades.Values) {
							if (cOrder.IsTrusted && !cOrder.IsCancel && cOrder.BarNumber > 0) {
								this.Send(cOrder.Action, cOrder.Category, cOrder.Price, cOrder.Contracts, cOrder.IsReverse, 0, cOrder.Name);
								
								if (cOrder.IsCancel) {  //如果是取消單, 就設定 BarNumber = -1 表示是大單模組這裡定時器下單的(會在取消事件接收那裏再補下)
									cOrder.BarNumber = -1;
								}
							}
						}
					}
				}

				__iTimeCount = 0;
			}

			lock (__oTimerLock) {
				__cTimer.Start();
			}
		}

		private void LargeOrderService_onResponse(object sender, ResponseEvent e) {
			switch (e.ResponseType) {
				case ResponseType.Cancel:
					TradeOrder cCancel = e.TradeOrder as TradeOrder;
					lock (__oLock) {
						lock (__cTrades) {
							string sName = cCancel.Name;
							if (__cTrades.ContainsKey(sName)) {
								__cTrades.Remove(sName);
							}
						}

						if (cCancel.BarNumber < 0 && cCancel.Contracts > 0) {
							base.Send(cCancel.Action, cCancel.Category, (cCancel.Category == OrderCategory.Market) ? AdjustPrice(cCancel.Action) : cCancel.Price, cCancel.Contracts, cCancel.IsReverse, 0, cCancel.Name);
						}
					}
					break;
				case ResponseType.Deal:
					TradeOrder cDeal = e.TradeOrder as TradeOrder;
					lock (__cTrades) {
						TradeOrder cOrder = null;
						string sName = cDeal.Name;
						if (__cTrades.TryGetValue(sName, out cOrder)) {
							if (cOrder.Contracts == 0) {
								__cTrades.Remove(sName);
							}
						}
					}
					break;
				case ResponseType.Trust:
					TradeOrder cTrust = e.TradeOrder as TradeOrder;
					lock (__cTrades) {
						string sName = cTrust.Name;
						if (__cTrades.ContainsKey(sName)) {
							__cTrades.Remove(sName);
						}
						__cTrades.Add(sName, cTrust);
					}
					break;
			}
		}
	}
}  //140行