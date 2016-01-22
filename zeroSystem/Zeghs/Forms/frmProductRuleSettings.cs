using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Products;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmProductRuleSettings : Form {
		private ERuleType __cRuleType = ERuleType.None;
		private AbstractProductProperty __cProperty = null;
		private List<RulePropertyAttribute> __cRules = null;

		internal frmProductRuleSettings(string exchangeName, ERuleType ruleType, AbstractProductProperty property) {
			InitializeComponent();

			__cRuleType = ruleType;
			__cProperty = property;
			
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(exchangeName);
			__cRules = cExchange.GetRuleItems(ruleType);
		}

		private bool SetRule(ref RuleBase rule, string className, bool isSetting) {
			if (rule != null) {
				if (isSetting && className.Equals(rule.ClassName)) {
					DialogResult cResult = MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (cResult == DialogResult.Yes) {
						IRuleSetting cSetting = rule as IRuleSetting;
						cSetting.ShowSetting();
						return false;
					}
				}
			}

			rule = new RuleBase();
			rule.ClassName = className;
			return true;
		}

		private void frmProductRuleSettings_Load(object sender, EventArgs e) {
			if (__cRules != null) {
				int iCount = __cRules.Count;
				for (int i = 0; i < iCount; i++) {
					comboRules.Items.Add(__cRules[i].Name);
				}
				
				comboRules.SelectedIndex = 0;
			}
		}

		private void btnOK_Click(object sender, EventArgs e) {
			int iIndex = comboRules.SelectedIndex;
			RulePropertyAttribute cAttrib = __cRules[iIndex];
			string sClassName = cAttrib.ClassName;
			bool bSetting = cAttrib.IsNeedSetting;

			RuleBase cRule = null;
			switch (__cRuleType) {
				case ERuleType.Contract:
					cRule = __cProperty.ContractRule;
					if (SetRule(ref cRule, sClassName, bSetting)) {
						__cProperty.ContractRule = cRule;
					}
					break;
				case ERuleType.PriceScale:
					cRule = __cProperty.PriceScaleRule;
					if (SetRule(ref cRule, sClassName, bSetting)) {
						__cProperty.PriceScaleRule = cRule;
					}
					break;
				case ERuleType.Tax:
					cRule = __cProperty.TaxRule;
					if (SetRule(ref cRule, sClassName, bSetting)) {
						__cProperty.TaxRule = cRule;
					}
					break;
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void comboRules_SelectedIndexChanged(object sender, EventArgs e) {
			int iIndex = comboRules.SelectedIndex;
			txtMemo.Text = __cRules[iIndex].Comment;
		}
	}
}