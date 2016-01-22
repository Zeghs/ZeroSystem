using System;
using System.Reflection;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   規則屬性類別
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class RulePropertyAttribute : Attribute {
		/// <summary>
		///   取得腳本物件內的所有使用者自訂的必要參數
		/// </summary>
		/// <param name="target">Assembly 組件</param>
		/// <returns>返回值: 規則屬性類別列表(根據 ERuleType 分類)</returns>
		public static Dictionary<ERuleType, List<RulePropertyAttribute>> GetRules(Assembly target) {
			Dictionary<ERuleType, List<RulePropertyAttribute>> cRules = null;
			
			Type[] cTypes = target.GetTypes();
			int iLength = cTypes.Length;
			if (iLength > 0) {
			        cRules = new Dictionary<ERuleType,List<RulePropertyAttribute>>();
			        foreach (Type cType in cTypes) {
			                if (cType.IsClass) {
			                        bool bExist = cType.IsDefined(typeof(RulePropertyAttribute), false);
			                        if (bExist) {
			                                object[] oAttribs = cType.GetCustomAttributes(typeof(RulePropertyAttribute), false);
							foreach (object oAttrib in oAttribs) {
								RulePropertyAttribute cRule = oAttrib as RulePropertyAttribute;

								ERuleType cRuleType = cRule.RuleType;
								List<RulePropertyAttribute> cList = null;
								if (!cRules.TryGetValue(cRuleType, out cList)) {
									cList = new List<RulePropertyAttribute>();
									cRules.Add(cRuleType, cList);
								}
								cList.Add(cRule);
							}
			                        }
			                }
			        }
			}
			return cRules;
		}

		private string __sName = null;
		private string __sComment = null;
		private string __sClassName = null;
		private bool __bNeedSetting = false;
		private ERuleType __cRuleType = ERuleType.None;

		/// <summary>
		///   [取得] 類別名稱
		/// </summary>
		public string ClassName {
			get {
				return __sClassName;
			}
		}

		/// <summary>
		///   [取得] 規則項目註解
		/// </summary>
		public string Comment {
			get {
				return __sComment;
			}
		}

		/// <summary>
		///   [取得] 是否需要設定
		/// </summary>
		public bool IsNeedSetting {
			get {
				return __bNeedSetting;
			}
		}

		/// <summary>
		///   [取得] 規則項目名稱
		/// </summary>
		public string Name {
			get {
				return __sName;
			}
		}

		/// <summary>
		///   [取得] 規則類別
		/// </summary>
		public ERuleType RuleType {
			get {
				return __cRuleType;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="type">規則類型</param>
		/// <param name="name">規則名稱</param>
		/// <param name="className">規則類別名稱</param>
		/// <param name="comment">規則說明</param>
		/// <param name="isNeedSetting">是否需要設定(如果需要設定則需要時做 IRuleSetting 介面)</param>
		public RulePropertyAttribute(ERuleType type, string name, string className, string comment, bool isNeedSetting = false) {
			__cRuleType = type;
			__sName = name;
			__sClassName = className;
			__sComment = comment;
			__bNeedSetting = isNeedSetting;
		}
	}
}