using System;
using System.Timers;
using System.Threading;
using System.Collections.Generic;
using log4net;
using PowerLanguage;
using Zeghs.Events;
using Zeghs.Orders;
using Netwings.Orders;

namespace Netwings {
	public sealed class LargeOrderService : RealOrderService {
		private const int TIMER_INTERVAL = 100;  //計時器基本觸發周期

		private int __iTimeCount = 0;
		private bool __bReverse = false;
		private bool __bDisposed = false;
		private TradeOrder __cTemp = null;
		private TradeOrder __cCurrent = null;
		private System.Timers.Timer __cTimer = null;

		private object __oLock = new object();

		/// <summary>
		///   [取得/設定] 重新送出限價單的時間間隔(單位:毫秒)
		/// </summary>
		[Input("重新送出限價單的時間間隔(單位:毫秒)")]
		private double ReSendLimit_Interval {
			get;
			set;
		}

		public LargeOrderService() {
			this.ReSendLimit_Interval = 5000;
			this.onResponse += LargeOrderService_onResponse;

			__cTimer = new System.Timers.Timer();
			__cTimer.AutoReset = false;
			__cTimer.Interval = TIMER_INTERVAL;
			__cTimer.Elapsed += Timer_onElapsed;
			__cTimer.Start();
		}

		public override bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false) {
			__bReverse = isReverse;  //取得反轉旗標

			bool bRet = base.Send(action, category, (category == OrderCategory.Market) ? AdjustPrice(action) : limitPrice, lots, isReverse, touchPrice, name, openNextBar);
			if (bRet) {
				__iTimeCount = 0;
			}
			return bRet;
		}

		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;

				if (disposing) {
					base.Dispose(disposing);

					__cTimer.Dispose();
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
			__iTimeCount += TIMER_INTERVAL;
			if (__iTimeCount >= this.ReSendLimit_Interval) {
				lock (__oLock) {
					if (__cCurrent != null && __cCurrent.Contracts > 0) {
						if (__cCurrent.IsTrusted && !__cCurrent.IsCancel) {
							__cCurrent.IsCancel = true;
							SendTrust(__cCurrent, __cCurrent.Ticket);
						}
					} else if (__cTemp != null) {
						bool bSuccess = this.Send(__cTemp.Action, __cTemp.Category, 0, __cTemp.Contracts, false, 0, __cTemp.Name);
						if (bSuccess) {
							__iTimeCount = 0;
						}
					}
				}
			}

			__cTimer.Start();
		}

		private void LargeOrderService_onResponse(object sender, ResponseEvent e) {
			switch (e.ResponseType) {
				case ResponseType.Cancel:
					lock (__oLock) {
						if (__cCurrent != null) {
							EOrderAction cAction = __cCurrent.Action;
							__cTemp = (__bReverse && (cAction == EOrderAction.Sell || cAction == EOrderAction.BuyToCover)) ? null : __cCurrent;
						}
						__cCurrent = null;
					}
					break;
				case ResponseType.Deal:
					lock (__oLock) {
					        if (__cCurrent != null && __cCurrent.Contracts == 0) {
							__cTemp = null;
							__cCurrent = null;
					        }
					}
					break;
				case ResponseType.Trust:
					lock (__oLock) {
						if (__cCurrent == null) {
							__cTemp = null;
							__cCurrent = e.TradeOrder as TradeOrder;
						}
					}
					break;
			}
		}
	}
}  //125行