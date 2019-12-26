using System;
using System.Reflection;
using Zeghs.Informations;

namespace Zeghs.Events {
	/// <summary>
	///   新增腳本事件類別
	/// </summary>
	public sealed class AddationScriptEvent : EventArgs {
		/// <summary>
		///   [取得] 腳本組件
		/// </summary>
		public Assembly Assembly {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 腳本資訊類別
		/// </summary>
		public ScriptInformation ScriptInformation {
			get;
			internal set;
		}

		internal AddationScriptEvent(Assembly assembly, ScriptInformation info) {
			this.Assembly = assembly;
			this.ScriptInformation = info;
		}
	}
}