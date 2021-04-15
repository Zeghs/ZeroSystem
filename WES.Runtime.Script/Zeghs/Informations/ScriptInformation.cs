using System;
using PowerLanguage;
using Zeghs.Scripts;

namespace Zeghs.Informations {
	/// <summary>
	///   腳本資訊類別
	/// </summary>
	public sealed class ScriptInformation {
		private Type __cScript = null;
		private ScriptPropertyAttribute __cProperty = null;

		/// <summary>
		///   [取得] 腳本完整名稱
		/// </summary>
		public string FullName {
			get {
				return __cScript.FullName;
			}
		}

		/// <summary>
		///   [取得] 腳本名稱
		/// </summary>
		public string Name {
			get {
				return __cScript.Name;
			}
		}
		
		/// <summary>
		///   [取得] 腳本屬性值
		/// </summary>
		public ScriptPropertyAttribute Property {
			get {
				return __cProperty;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="script">腳本Type型別</param>
		/// <param name="property">腳本屬性</param>
		public ScriptInformation(Type script, ScriptPropertyAttribute property) {
			__cScript = script;
			__cProperty = property;
		}

		/// <summary>
		///   建立腳本
		/// </summary>
		/// <param name="args">腳本所需參數</param>
		/// <returns>返回值:AbstractScript類別</returns>
		public CStudyAbstract CreateScript(object args) {
			return Activator.CreateInstance(__cScript, args) as CStudyAbstract;
		}
	}
}