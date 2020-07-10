using PowerLanguage;
using Zeghs.Services;
using Netwings.Orders;

namespace Zeghs.Orders {
	/// <summary>
	///   市場單下單交易員類別
	/// </summary>
	internal sealed class OrderMarket : IOrderMarket {
		private IOrderSender __cSender = null;
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
		/// <param name="openNextBar">是否開倉於下一根 Bars</param>
		internal OrderMarket(AbstractOrderService service, SOrderParameters args, bool openNextBar) {
			this.ID = System.Guid.NewGuid().GetHashCode();
			Contracts cContract = args.Lots;
			this.Info = new Order(args.Name, args.Action, OrderCategory.Market, (cContract.IsDefault) ? service.DefaultContracts : args.Lots, openNextBar, args.ExitTypeInfo);

			__cSender = service as IOrderSender;
			__cPositions = service.Positions;
		}
		
		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send() {
			return Send(Info.Name, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(int numLots) {
			return Send(Info.Name, numLots);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(string new_name) {
			return Send(new_name, 0);
		}

		/// <summary>
		///   送出下單命令
		/// </summary>
		/// <param name="new_name">新的下單名稱</param>
		/// <param name="numLots">下單數量</param>
		/// <returns>返回值: true=傳輸成功, false=傳輸失敗</returns>
		public bool Send(string new_name, int numLots) {
			int iPositionLots = GetPositionCount();
			numLots = (Info.Contracts.IsUserSpecified) ? numLots : Info.Contracts.Contract;
			numLots = (Info.IsExit && (!Info.OrderExit.IsTotal || numLots > iPositionLots)) ? iPositionLots : numLots;

			bool bRet = true;
			int iNumLots = (!Info.IsExit) ? iPositionLots : 0;
			if (iNumLots > 0) {
				string sName = string.Format("Close_{0}", ID);
				EOrderAction cAction = (Info.Action == EOrderAction.Buy) ? EOrderAction.BuyToCover : (Info.Action == EOrderAction.SellShort) ? EOrderAction.Sell : Info.Action;
				bRet = __cSender.Send(cAction, Info.Category, 0, iNumLots, true, 0, sName, Info.OnClose);
			}

			if (bRet && numLots > 0 && iNumLots == 0) {
				bRet = __cSender.Send(Info.Action, Info.Category, 0, numLots, false, 0, new_name, Info.OnClose);
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