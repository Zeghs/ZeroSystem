using System;
using System.Threading;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Orders;
using Netwings.Orders;

namespace Netwings {
	public sealed class GridOrderService : RealOrderService, IOrderSender {
		private const double 漲跌幅交易限制 = 0.097d;

		private sealed class _Truster {
			private int __iDealCount = 0;
			private bool __bCanceled = false;
			private TradeOrder __cTrustOrder = null;
			private Dictionary<string, TradeOrder> __cGridPrices = null;

			internal EOrderAction Action {
				get {
					return __cTrustOrder.Action;
				}
			}

			internal bool IsCancel {
				get {
					return __bCanceled;
				}
			}

			internal bool IsClosed {
				get {
					return __cTrustOrder.Action == EOrderAction.Sell || __cTrustOrder.Action == EOrderAction.BuyToCover;
				}
			}

			internal bool IsEmpty {
				get {
					return __cTrustOrder.Contracts == __iDealCount || (__bCanceled && __cGridPrices.Count == 0);
				}
			}

			private int Contracts {
				get {
					return __cTrustOrder.Contracts - __iDealCount;
				}
			}

			internal _Truster(TradeOrder order) {
				__cTrustOrder = order;
				__cGridPrices = new Dictionary<string, TradeOrder>(16);
			}

			internal void Cancel() {
				__bCanceled = true;
			}

			internal void CheckCancel(TradeOrder order) {
				lock (__cGridPrices) {
					string sName = order.Name;
					if (__cGridPrices.ContainsKey(sName)) {
						__cGridPrices.Remove(sName);
					}
				}
			}

			internal void CheckTrust(TradeOrder order) {
				TradeOrder cOrder = null;
				lock (__cGridPrices) {
					string sName = order.Name;
					if (__cGridPrices.TryGetValue(sName, out cOrder)) {
						if (cOrder.Ticket == null) {
							__cGridPrices[sName] = order;
						}
					}
				}
			}

			internal void CheckDeal(TradeOrder order) {
				lock (__cGridPrices) {
					Interlocked.Add(ref __iDealCount, order.Contracts);

					string sName = order.Name;
					if (__cGridPrices.ContainsKey(sName)) {
						if (order.IsDealed) {
							__cGridPrices.Remove(sName);
						}
					}
				}
			}

			internal List<TradeOrder> CreateTrusts(int barNumber, double price, int count, int stepLots, double lowest, double highest, double priceScale) {
				if (__bCanceled) {  //如果取消旗標 = true(使用者取消) 表示此筆委託單不再建立鋪單委託
					return null;
				}

				List<TradeOrder> cResult = null;
				lock (__cGridPrices) {
					foreach (TradeOrder cOrder in __cGridPrices.Values) {
						if (!cOrder.IsTrusted || cOrder.IsCancel || cOrder.Contracts == 0) {
							return null;
						}
					}

					int iTotals = (int) Math.Ceiling((double) this.Contracts / stepLots);
					int iCount = (iTotals < count) ? iTotals : count;
					
					__cTrustOrder.BarNumber = barNumber;

					EOrderAction cAction = __cTrustOrder.Action;
					double dScale = (cAction == EOrderAction.Buy || cAction == EOrderAction.BuyToCover) ? -priceScale : priceScale;

					int iTotalV = 0;
					bool bLimit = false;
					cResult = new List<TradeOrder>(count);
					HashSet<string> cTPrices = new HashSet<string>();
					for (int i = 0; i < iCount; i++) {
						int iLots = this.Contracts - iTotalV;
						if (iLots <= 0) {
							break;
						}

						double dPrice = Math.Round(price + dScale * i, 2);
						if (dPrice <= lowest || dPrice >= highest) {
							bLimit = true;  //如果價格觸碰到漲跌停價格(設定價格極限旗標)
						}

						TradeOrder cTrust = null;
						string sName = string.Format("{0}|{1}", __cTrustOrder.Name, dPrice);
						if (__cGridPrices.TryGetValue(sName, out cTrust)) {
							if (cTrust.IsTrusted && !cTrust.IsCancel) {
								if (bLimit) {  //如果是漲跌停價格的委託單(數量相同就保留, 數量不同直接離開)
									if (cTrust.Contracts == iLots) {
										cTPrices.Add(sName);
									}
									break;
								} else {
									iTotalV += cTrust.Contracts;
									cTPrices.Add(sName);
								}
							}
						} else {
							int iContracts = (bLimit) ? iLots : (iLots < stepLots) ? iLots : stepLots;
							iTotalV += iContracts;

							TradeOrder cOrder = new TradeOrder() {
								Action = __cTrustOrder.Action,
								BarNumber = __cTrustOrder.BarNumber,
								Category = __cTrustOrder.Category,
								Contracts = iContracts,
								Name = sName,
								Price = dPrice,
								IsReverse = false
							};
							
							cResult.Add(cOrder);
							cTPrices.Add(sName);
							__cGridPrices.Add(sName, cOrder);
						}
					}

					//計算欲取消的委託單
					foreach (string sName in __cGridPrices.Keys) {
						if (!cTPrices.Contains(sName)) {
							TradeOrder cTrust = __cGridPrices[sName];
							if (cTrust.IsTrusted && !cTrust.IsCancel) {
								cResult.Add(cTrust);
							}
						}
					}
				}
				return cResult;
			}

			internal TradeOrder[] GetTrustTickets() {
				TradeOrder[] cOrders = null;
				lock (__cGridPrices) {
					int iCount = __cGridPrices.Count;
					if (iCount > 0) {
						cOrders = new TradeOrder[iCount];
						__cGridPrices.Values.CopyTo(cOrders, 0);
					}
				}
				return cOrders;
			}

			internal void SendCompleted(TradeOrder order, bool success) {
				lock (__cGridPrices) {
					if (__cGridPrices.ContainsKey(order.Name)) {
						if (!success) {
							__cGridPrices.Remove(order.Name);
						}
					}
				}
			}
		}

		private bool __bDisposed = false;
		private bool __bClosedFlag = false;
		private Dictionary<string, _Truster> __cTrusts = null;
		private object __oLock = new object();

		/// <summary>
		///   [取得/設定] 每一掛單檔次委託單量
		/// </summary>
		[Input("每一掛單檔次委託單量")]
		private int LotsPerTrust {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 欲委託掛單的檔次個數
		/// </summary>
		[Input("欲委託掛單的檔次個數")]
		private int TrustCount {
			get;
			set;
		}

		public GridOrderService() {
			this.TrustCount = 3;
			this.LotsPerTrust = 1;
			this.useCloseProtect = false;
			this.onResponse += GridOrderService_onResponse;

			__cTrusts = new Dictionary<string, _Truster>(16);
		}

		/// <summary>
		///   建立市價買賣模式(在下一根 Bars 建立之後以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public override IOrderMarket MarketNextBar(SOrderParameters orderParams) {
			return null;
		}

		/// <summary>
		///   建立市價買賣模式(立即以市價送出委託單)
		/// </summary>
		/// <param name="orderParams">下單參數</param>
		/// <returns>返回值: IOrderMarket介面</returns>
		public override IOrderMarket MarketThisBar(SOrderParameters orderParams) {
			return null;
		}

		/// <summary>
		///   傳送下單命令
		/// </summary>
		/// <param name="trust">交易訂單資訊</param>
		/// <param name="isCancel">是否要取消此交易訂單(成功委託才能取消訂單)</param>
		/// <returns>返回值: true=成功, false=失敗</returns>
		bool IOrderSender.Send(TradeOrder trust, bool isCancel) {
			if (isCancel) {
				string[] sNames = trust.Name.Split('|');

				_Truster cTruster = null;
				lock (__cTrusts) {
					__cTrusts.TryGetValue(sNames[0], out cTruster);
				}

				if (cTruster != null && !cTruster.IsCancel) {
					cTruster.Cancel();
				}
				return base.SendTrust(trust, true);
			} else {
				return base.SendTrust(trust, false);
			}
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
		public override bool Send(EOrderAction action, OrderCategory category, double limitPrice, int lots, bool isReverse, double touchPrice = 0, string name = null, bool openNextBar = false) {
			if (this.Bars.IsLastBars) {
				double dPrice = Math.Round((limitPrice == 0) ? ((action == EOrderAction.Buy || action == EOrderAction.BuyToCover) ? Bars.DOM.Ask[0].Price : Bars.DOM.Bid[0].Price) : limitPrice, 2);

				//檢查是否為平倉單(如果是平倉單, 會把倉部位內所有的委託新倉單全部取消之後才會下平倉單)
				if (action == EOrderAction.Sell || action == EOrderAction.BuyToCover) {
					int iCancelCount = 0;
					lock (__cTrusts) {
						foreach (_Truster cTemp in __cTrusts.Values) {
							if (!cTemp.IsClosed) {
								TradeOrder[] cOrders = cTemp.GetTrustTickets();
								if (cOrders != null) {
									cTemp.Cancel();

									foreach (TradeOrder cOrder in cOrders) {
										if (cOrder.IsSended || cOrder.IsTrusted) {
											if (!cOrder.IsCancel && cOrder.Contracts > 0) {
												cOrder.IsCancel = this.SendTrust(cOrder, true);
											}
											++iCancelCount;
										}
									}
								}
							}
						}
					}

					if (iCancelCount > 0) {
						return false;
					}
					__bClosedFlag = true;
				}

				if (__bClosedFlag && (action == EOrderAction.Buy || action == EOrderAction.SellShort)) {
					return false;
				}

				_Truster cTruster = null;
				lock (__cTrusts) {
					if (!__cTrusts.TryGetValue(name, out cTruster)) {
						TradeOrder cOrder = new TradeOrder();
						cOrder.Action = action;
						cOrder.BarNumber = Bars.CurrentBar;
						cOrder.Category = category;
						cOrder.Contracts = lots;
						cOrder.IsReverse = false;
						cOrder.Name = name;

						cTruster = new _Truster(cOrder);
						__cTrusts.Add(name, cTruster);
					}
				}
				SendTrust(cTruster, dPrice);
				return true;
			}
			return false;
		}

		protected override void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					base.Dispose(disposing);

					__cTrusts.Clear();
				}
			}
		}

		private double[] GetLimitPrice(double priceScale) {
			IQuote cQuote = this.Bars.Quotes;
			double dReferPrice = (cQuote == null) ? this.Bars.Close.Value : cQuote.ReferPrice;
			double dPrice1 = dReferPrice * (1 - 漲跌幅交易限制);
			double dPrice2 = dReferPrice * (1 + 漲跌幅交易限制);
			return new double[] { Math.Round((Math.Floor(dPrice1 / priceScale) + 1) * priceScale, 2), Math.Round(Math.Floor(dPrice2 / priceScale) * priceScale, 2) };
		}

		private bool SendTrust(_Truster truster, double price) {
			bool bRet = false;
			lock (__oLock) {
				double dPriceScale = this.Bars.Info.PriceScale;
				double[] dLimitPrices = GetLimitPrice(dPriceScale);
				
				List<TradeOrder> cOrders = truster.CreateTrusts(Bars.CurrentBar, price, this.TrustCount, this.LotsPerTrust, dLimitPrices[0], dLimitPrices[1], dPriceScale);
				if (cOrders != null) {
					foreach (TradeOrder cOrder in cOrders) {
						if (!cOrder.IsCancel) {
							if (cOrder.Ticket == null) {
								bRet = base.Send(cOrder.Action, cOrder.Category, cOrder.Price, cOrder.Contracts, cOrder.IsReverse, 0, cOrder.Name, false);
								truster.SendCompleted(cOrder, bRet);
							} else {
								cOrder.IsCancel = this.SendTrust(cOrder, true);
								truster.SendCompleted(cOrder, cOrder.IsCancel);
							}
						}
					}
				}
			}
			return bRet;
		}

		private void GridOrderService_onResponse(object sender, ResponseEvent e) {
			if (e.ResponseType == ResponseType.Update) {  //回報類型為 Update 都忽略(此為盈利更新通知)
				return;
			}

			TradeOrder cOrder = e.TradeOrder as TradeOrder;
			string[] sNames = cOrder.Name.Split('|');
			
			_Truster cTruster = null;
			lock (__cTrusts) {
				__cTrusts.TryGetValue(sNames[0], out cTruster);
			}

			if (cTruster != null) {
				switch (e.ResponseType) {
					case ResponseType.Cancel:
					case ResponseType.Deal:
						if (e.ResponseType == ResponseType.Cancel) {
							cTruster.CheckCancel(cOrder);
						} else {
							cTruster.CheckDeal(cOrder);
						}

						if (cTruster.IsEmpty) { //如果已經交易完畢
							lock (__cTrusts) {
								__cTrusts.Remove(sNames[0]);
							}

							if (cTruster.IsClosed && __bClosedFlag) {
								__bClosedFlag = false;
							}
						}
						break;
					case ResponseType.Trust:
						cTruster.CheckTrust(cOrder);
						break;
				}
			}
		}
	}
} //425行