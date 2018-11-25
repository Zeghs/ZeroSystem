using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Services;
using Netwings.Orders;

namespace Zeghs.Orders {
	/// <summary>
	///   限價下單交易員類別
	/// </summary>
	internal sealed class OrderPriced : IOrderPriced {
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
		/// <returns>回傳值: true=成功, false=失敗</returns>
		public bool Cancel(string name = null) {
			bool bRet = false;
			if (name == null) {
				List<TradeOrder> cTrades = __cEntrusts.Trades;
				int iCount = cTrades.Count;
				for (int i = iCount - 1; i >= 0; i--) {
					TradeOrder cOrder = cTrades[i];
					if (cOrder.IsTrusted && cOrder.Action == Info.Action && cOrder.Category == Info.Category) {
						bRet = __cSender.Send(cOrder, true);

						if(!bRet) {
							return false;
						}
					}
				}
			} else {
				TradeOrder cOrder = __cEntrusts.GetTradeFromName(name);
				if (cOrder != null) {
					bRet = __cSender.Send(cOrder, true);
				}
			}
			return bRet;
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		public void Send(double price) {
			Send(Info.Name, price, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		public void Send(double price, int numLots) {
			Send(Info.Name, price, numLots);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		public void Send(string new_name, double price) {
			Send(new_name, price, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="price">指定價格</param>
		/// <param name="numLots">下單數量</param>
		public void Send(string new_name, double price, int numLots) {
			int iPositionLots = GetPositionCount();
			numLots = (Info.Contracts.IsUserSpecified) ? numLots : Info.Contracts.Contract;
			numLots = (Info.IsExit && (!Info.OrderExit.IsTotal || numLots > iPositionLots)) ? iPositionLots : numLots;

			bool bRet = true;
			int iNumLots = (!Info.IsExit) ? iPositionLots : 0;
			if (iNumLots > 0) {
				string sName = string.Format("Close_{0}", ID);
				EOrderAction cAction = (Info.Action == EOrderAction.Buy) ? EOrderAction.BuyToCover : (Info.Action == EOrderAction.SellShort) ? EOrderAction.Sell : Info.Action;
				bRet = __cSender.Send(cAction, OrderCategory.Market, 0, iNumLots, false, 0, sName);
			}

			if (bRet && numLots > 0) {
				__cSender.Send(Info.Action, Info.Category, price, numLots, iNumLots > 0, 0, new_name);
			}
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