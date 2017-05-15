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
		private double RequestLimitInterval {
			get;
			set;
		}

		public LargeOrderService() {
			this.RequestLimitInterval = 5000;
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
				lock (__oLock) {
					__iTimeCount = 0;  //如果送單成功就歸 0 重新計時(當達到設定值才需要重新處理取消與發送單, 尚未達到就讓委託單在市場上可以多成交幾張)
				}
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
			lock (__oLock) {
				__iTimeCount += TIMER_INTERVAL;  //累加時間個數(如果達到 RequestLimitInterval 設定的值才處理)
				if (__iTimeCount >= this.RequestLimitInterval) {
					if (__cCurrent != null && __cCurrent.Contracts > 0) {
						if (__cCurrent.IsTrusted && !__cCurrent.IsCancel) {
							__cCurrent.IsCancel = true;
							SendTrust(__cCurrent, __cCurrent.Ticket);
						}
					} else if (__cTemp != null) {
						bool bSuccess = this.Send(__cTemp.Action, __cTemp.Category, 0, __cTemp.Contracts, false, 0, __cTemp.Name);
						if (bSuccess) {
							__iTimeCount = 0;  //歸 0 重新計時(當達到設定值才需要重新處理取消與發送單, 尚未達到就讓委託單在市場上可以多成交幾張)
						}
					}

					__iTimeCount -= TIMER_INTERVAL;  //當達到設定值就不需要再重新累積(加快處理重新送單與取消單, 直到又再次成功送單在歸 0 重新計時)
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

							//如果有反轉信號單且取消的單是平倉單就不用再把單放到 __cTemp 內(因為父類別 RealOrderService 在平倉單取消之後, 後面尚未傳送的單都會全部取消掉)
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
}  //130行