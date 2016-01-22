using System;
using PowerLanguage;

namespace Netwings.Rules {
	internal sealed class RuleCreater {
		/// <summary>
		///   建立規則
		/// </summary>
		/// <param name="className">規則類別名稱</param>
		/// <returns>返回值:規則類別</returns>
		internal static RuleBase CreateRule(RuleBase rule) {
			RuleBase cRule = null;

			if (rule != null) {
				object oArgs = rule.args;
				string sRuleClass = rule.ClassName;
				if (oArgs == null) {
					cRule = Activator.CreateInstance(Type.GetType(sRuleClass)) as RuleBase;
				} else {
					cRule = Activator.CreateInstance(Type.GetType(sRuleClass), oArgs) as RuleBase;
				}
				cRule.ClassName = sRuleClass;
			}
			return cRule;
		}
	}
}