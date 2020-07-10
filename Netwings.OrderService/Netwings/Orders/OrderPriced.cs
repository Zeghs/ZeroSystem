using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Services;
using Netwings.Orders;

namespace Zeghs.Orders {
	/// <summary>
	///   限價下單交易員類別
	/// </summary>
	internal class OrderPriced : IOrderPriced {
		private IOrderSender __cSender = null;
		private TradeList<TradeOrder> __cEntrusts = null;
		private ISeries<IMarketPosition> __cPositions = null;

		/// <summary>
		///   [取得] 下單的代號
		/// </summary>
		public int ID {
			get;
			private set;
		}

		/// <summary>
		///   [取得] 下單資訊
		/// </summary>
		public Order Info {
			get;
			private set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="service">AbstractOrderService 下單服務抽象類別</param>
		/// <param name="args">下單參數</param>
		internal OrderPriced(AbstractOrderService service, SOrderParameters args) {
			this.ID = System.Guid.NewGuid().GetHashCode();
			Contracts cContract = args.Lots;
			this.Info = new Order(args.Name, args.Action, OrderCategory.Limit, (cContract.IsDefault) ? service.DefaultContracts : args.Lots, false, args.ExitTypeInfo);

			__cEntrusts = (service as IOrderEntrust).Entrusts;

			__cSender = service as IOrderSender;
			__cPositions = service.Positions;
		}

		/// <summary>
		///   取消委託中訂單
		/// </summary>
		/// <param name="name">下單名稱(如果 name 為 null 則取消全部委託中的訂單)</param>
		/// <returns>回傳值: 取消委託中訂單的總口數</returns>
		public int Cancel(string name = null) {
			int iCancelLots = 0, iDealCount = 0;
			if (name == null) {
				List<TradeOrder> cTrades = __cEntrusts.Trades;
				int iCount = cTrades.Count;
				for (int i = iCount - 1; i >= 0; i--) {  //由最後一筆往前移動索引值(避免因為移除委託單導致索引指向錯誤)
					TradeOrder cOrder = cTrades[i];
					if ((cOrder.IsSended || cOrder.IsTrusted) && cOrder.Action == Info.Action && cOrder.Category == Info.Category) {
						if (cOrder.Contracts == 0) {
							++iDealCount;  //當已經完成成交後, 已經沒有合約量, 但是因為還沒有經過倉部位計算, 所以還不會移除此筆委託單, 所以還是要認定此筆委託單尚未取消
						} else {
							iCancelLots += cOrder.Contracts;
							if (cOrder.IsTrusted && !cOrder.IsCancel) {
								cOrder.IsCancel = __cSender.Send(cOrder, true);
							}
						}
					}
				}
			} else {
				TradeOrder cOrder = __cEntrusts.GetTradeFromName(name);
				if (cOrder != null && (cOrder.IsSended || cOrder.IsTrusted) && cOrder.Action == Info.Action && cOrder.Category == Info.Category) {
					if (cOrder.Contracts == 0) {
						++iDealCount;  //當已經完成成交後, 已經沒有合約量, 但是因為還沒有經過倉部位計算, 所以還不會移除此筆委託單, 所以還是要認定此筆委託單尚未取消
					} else {
						iCancelLots += cOrder.Contracts;
						if (cOrder.IsTrusted && !cOrder.IsCancel) {
							cOrder.IsCancel = __cSender.Send(cOrder, true);
						}
					}
				}
			}
			return (iCancelLots == 0) ? iDealCount : iCancelLots;
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(double price) {
			return Send(Info.Name, price, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(double price, int numLots) {
			return Send(Info.Name, price, numLots);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(string new_name, double price) {
			return Send(new_name, price, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(string new_name, double price, int numLots) {
			int iPositionLots = GetPositionCount();
			numLots = (Info.Contracts.IsUserSpecified) ? numLots : Info.Contracts.Contract;
			numLots = (Info.IsExit && (!Info.OrderExit.IsTotal || numLots > iPositionLots)) ? iPositionLots : numLots;

			bool bRet = true;
			int iNumLots = (!Info.IsExit) ? iPositionLots : 0;
			if (iNumLots > 0) {
				string sName = string.Format("Close_{0}", ID);
				EOrderAction cAction = (Info.Action == EOrderAction.Buy) ? EOrderAction.BuyToCover : (Info.Action == EOrderAction.SellShort) ? EOrderAction.Sell : Info.Action;
				bRet = __cSender.Send(cAction, OrderCategory.Market, 0, iNumLots, true, 0, sName);
			}

			if (bRet && numLots > 0 && iNumLots == 0) {
				bRet = __cSender.Send(Info.Action, Info.Category, price, numLots, false, 0, new_name);
			}
			return bRet;
		}

		private int GetPositionCount() {
			IMarketPosition cPosition = __cPositions[0];

			int iSide = (int) cPosition.Side;
			int iBSFlag = (Info.Action == EOrderAction.Buy || Info.Action == EOrderAction.BuyToCover) ? 1 : -1;
			int iDiff = iSide + iBSFlag;
			return (iDiff == 0) ? cPosition.OpenLots : 0;
		}
	}
}