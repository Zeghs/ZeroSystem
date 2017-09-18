using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Zeghs.Data;
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
		private AbstractOrderService __cOrderService = null;  //目前作用中的交易服務類別
		private Dictionary<string, AbstractOrderService> __cTradeServices = null;  //存放交易服務類別字典集合

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
			__cTradeServices = new Dictionary<string, AbstractOrderService>(8);
		}

		/// <summary>
		///   施行信號屬性設定值
		/// </summary>
		/// <param name="property">信號屬性類別</param>
		public void ApplyProperty(SignalProperty property) {
			__cProperty = property;
			this.SetMaximumBarsReference(__cProperty.MaximumBarsReference);
		}

		internal override void CStudyDestroy() {
			onTradeResponse = null;

			lock (__cTradeServices) {
				foreach (var cTradeService in __cTradeServices.Values) {
					cTradeService.Dispose();  //釋放所有交易服務資源
				}
				__cTradeServices.Clear();
			}
			if (log.IsInfoEnabled) log.Info("[SignalObject.CStudyDestory] Object and resource destory...");
		}

		internal override void CStudyInitialize() {
			if (__cProperty.OrderSource != null) {
				IInstrument cBars0 = this.Bars;
				CreateTrader(cBars0, null);  //建立交易服務元件
				SelectTrader(cBars0.Request.Symbol);  //選擇作用中的交易服務元件
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

			lock (__cTradeServices) {
				foreach (var cTradeService in __cTradeServices.Values) {
					cTradeService.OnWork();    //處理下單服務必要的工作
				}
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

		/// <summary>
		///   交易服務建立員
		/// </summary>
		/// <param name="instrument">商品資訊介面</param>
		/// <param name="args">交易服務組件設定參數</param>
		/// <param name="orderSource">交易服務來源名稱[預設:null](format: 交易組件名稱;交易服務類別名稱, null=使用預設交易服務來源類別名稱)</param>
		protected void CreateTrader(IInstrument instrument, object args, string orderSource = null) {
			bool bExist = false;
			string sSymbolId = instrument.Request.Symbol;
			lock (__cTradeServices) {
				bExist = __cTradeServices.ContainsKey(sSymbolId);
			}

			if (bExist) {  //如果交易服務已經存在
				SelectTrader(sSymbolId);  //直接選擇交易服務
				return;
			}

			orderSource = (orderSource == null) ? __cProperty.OrderSource : orderSource;
			AbstractOrderService cOrderService = OrderManager.Manager.CreateOrderService(orderSource);
			cOrderService.onResponse += OrderService_onResponse;
			cOrderService.SetInstrument(instrument as Instrument);
			cOrderService.SetDefaultContracts(__cProperty.DefaultContracts);

			if (args != null) {
				var cParameters = InputAttribute.GetParameters(cOrderService);
				if (cParameters.Count > 0) {
					var cProperies = args.GetType().GetProperties();

					Dictionary<string, object> cArgs = new Dictionary<string, object>(cProperies.Length);
					foreach (var cProperty in cProperies) {
						cArgs.Add(cProperty.Name, cProperty.GetValue(args, null));
					}

					foreach (var cParameter in cParameters) {
						object oValue = null;
						if (cArgs.TryGetValue(cParameter.Name, out oValue)) {
							cParameter.SetValue(oValue);
						}
					}
				}
				cOrderService.Initialize();  //如果有指定參數, 在指定完畢後直接初始化交易服務
			}

			lock (__cTradeServices) {
				__cTradeServices.Add(sSymbolId, cOrderService);
			}
			if (log.IsInfoEnabled) log.InfoFormat("[SignalObject.CreateTrader] Set \"{0}\" and create...  symbolId={1}", orderSource, sSymbolId);
		}

		/// <summary>
		///   釋放交易服務所有資源
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		/// <param name="freeInstrument">是否一併釋放商品資訊類別[預設:false](true=一併釋放, false=不釋放保留於快取內)</param>
		protected void DestroyTrader(string symbolId, bool freeInstrument = false) {
			lock (__cTradeServices) {
				AbstractOrderService cService = null;
				if (__cTradeServices.TryGetValue(symbolId, out cService)) {
					if (freeInstrument) {
						cService.Bars.Dispose();
					}
					cService.Dispose();

					__cTradeServices.Remove(symbolId);
					if (log.IsInfoEnabled) log.InfoFormat("[SignalObject.DestroyTrader] Destroy trade services and disposed...  symbolId={0}, freeInstrument={1}", symbolId, freeInstrument);

					SelectTrader(this.Bars.Request.Symbol);  //選擇預設交易服務
				}
			}
		}

		/// <summary>
		///   選擇作用中的交易服務
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		protected void SelectTrader(string symbolId) {
			lock (__cTradeServices) {
				AbstractOrderService cService = null;
				if (__cTradeServices.TryGetValue(symbolId, out cService)) {
					__cOrderService = cService;
					this.OrderCreator = __cOrderService as IOrderCreator;
					if (log.IsInfoEnabled) log.InfoFormat("[SignalObject.SelectTrader] Select trade services and OrderCreator...  symbolId={0}", symbolId);
				}
			}
		}

		private void OrderService_onResponse(object sender, ResponseEvent e) {
			OnTradeResponse(e);
		}
	}
}  //238行