using System;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Services;

namespace Zeghs.Events {
	/// <summary>
	///   腳本初始化事件類別
	/// </summary>
	public sealed class ScriptParametersEvent : EventArgs {
		private AbstractOrderService __cOrderService = null;
		private List<InputAttribute> __cScriptParameters = null;

		/// <summary>
		///   [取得] 下單服務元件(設定佣金參數)
		/// </summary>
		public AbstractOrderService OrderService {
			get {
				return __cOrderService;
			}
		}

		/// <summary>
		///   [取得] 腳本參數列表
		/// </summary>
		public List<InputAttribute> ScriptParameters {
			get {
				return __cScriptParameters;
			}
		}

		/// <summary>
		///   設定下單服務元件(需要設定佣金參數, 所以必須傳出)
		/// </summary>
		/// <param name="service">下單服務元件</param>
		internal void SetOrderService(AbstractOrderService service) {
			__cOrderService = service;
		}

		/// <summary>
		///   設定腳本參數列表
		/// </summary>
		/// <param name="args">腳本參數列表</param>
		internal void SetScriptParameters(List<InputAttribute> args) {
			__cScriptParameters = args;
		}
	}
}