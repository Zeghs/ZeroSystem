using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Services;
using Zeghs.Managers;

namespace PowerLanguage {
	/// <summary>
	///   交易信號物件抽象類別
	/// </summary>
	public abstract class SignalObject : CStudyAbstract {
		/// <summary>
		///   收到委託或是成交回報時需要觸發的事件
		/// </summary>
		public event EventHandler<ResponseEvent> onTradeResponse = null;

		private SignalProperty __cProperty = null;
		private AbstractOrderService __cOrderService = null;

		/// <summary>
		///   [取得] 目前的留倉部位
		/// </summary>
		protected IMarketPosition CurrentPosition {
			get {
				return (__cOrderService == null) ? null : __cOrderService.CurrentPosition;
			}
		}

		/// <summary>
		///   [取得] 留倉部位序列資訊
		/// </summary>
		protected ISeries<IMarketPosition> Positions {
			get {
				return (__cOrderService == null) ? null : __cOrderService.Positions;
			}
		}

		/// <summary>
		///   [取得/設定] 初始本金
		/// </summary>
		protected double InitialCapital {
			get {
				return __cProperty.InitialCapital;
			}
		}

		/// <summary>
		///   [取得] 已平倉的總損益
		/// </summary>
		protected double NetProfit {
			get {
				return (__cOrderService == null) ? 0 : __cOrderService.Positions.NetProfit;
			}
		}

		/// <summary>
		///   [取得] 下單物件建立員
		/// </summary>
		protected IOrderCreator OrderCreator {
			get;
			private set;
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="_ctx">相容 Multicharts 系統</param>
		public SignalObject(object _ctx) {
			__cProperty = new SignalProperty();
		}

		/// <summary>
		///   施行信號屬性設定值
		/// </summary>
		/// <param name="property">信號屬性類別</param>
		public void ApplyProperty(SignalProperty property) {
			__cProperty = property;
			this.SetMaximumBarsReference(__cProperty.MaximumBarsReference);

			if (this.Bars != null) {  //如果已經有資料表示信號腳本已經啟動了
				CStudyInitialize();  //如果已經啟動才能執行初始化(尚未啟動不能執行初始化命令, 因為使用者自訂參數尚未被填入)
			}
		}

		internal override void CStudyDestory() {
			onTradeResponse = null;

			if (__cOrderService != null) {
				__cOrderService.Dispose();
			}
			if (log.IsInfoEnabled) log.Info("[SignalObject.CStudyDestory] Object and resource destory...");
		}

		internal override void CStudyInitialize() {
			if (__cProperty.OrderSource != null) {
				if (__cOrderService != null) {  //如果之前已經建立下單服務元件
					__cOrderService.Dispose();  //釋放下單服務元件
				}

				string sOrderSource = __cProperty.OrderSource;
				__cOrderService = OrderManager.Manager.CreateOrderService(sOrderSource);
				__cOrderService.onResponse += OrderService_onResponse;
				__cOrderService.SetInstrument(this.Instruments[0]);
				__cOrderService.SetDefaultContracts(__cProperty.DefaultContracts);
				
				this.OrderCreator = __cOrderService as IOrderCreator;
				if (log.IsInfoEnabled) log.InfoFormat("[SignalObject.SetOrderSource] Set \"{0}\" and create...", sOrderSource);
			}

			List<InputAttribute> cParameters = new List<InputAttribute>();
			cParameters.AddRange(InputAttribute.GetParameters(this));
			
			if (__cOrderService != null) {
				cParameters.AddRange(InputAttribute.GetParameters(__cOrderService));
			}

			ScriptParametersEvent cScriptParametersEvent = new ScriptParametersEvent();
			cScriptParametersEvent.SetOrderService(__cOrderService);
			cScriptParametersEvent.SetScriptParameters(cParameters);
			OnScriptParameters(cScriptParametersEvent);

			if (__cOrderService != null) {
				__cOrderService.Initialize();  //參數設定完成之後才初始化下單元件
			}
		}

		/// <summary>
		///   當有資料時會觸發這個更新的事件
		/// </summary>
		internal override void OnUpdate() {
			this.OnCalculate();  //執行 Bars 計算

			if (__cOrderService != null) {
				__cOrderService.OnWork();  //處理下單服務必要的工作
			}
			base.OnUpdate();     //發送更新事件
		}

		/// <summary>
		///   觸發交易回報事件
		/// </summary>
		/// <param name="e">ResponseEvent 類別</param>
		protected void OnTradeResponse(ResponseEvent e) {
			if (onTradeResponse != null) {
				onTradeResponse(this, e);
			}
		}

		private void OrderService_onResponse(object sender, ResponseEvent e) {
			OnTradeResponse(e);
		}
	}
}  //156行