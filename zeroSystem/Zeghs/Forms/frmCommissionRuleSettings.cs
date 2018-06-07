using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Orders;
using Zeghs.Services;

namespace Zeghs.Forms {
	internal partial class frmCommissionRuleSettings : Form {
		private ERuleType __cRuleType = ERuleType.None;
		private ICommission __cCommission = null;
		private AbstractOrderService __cOrderService = null;
		private List<RulePropertyAttribute> __cRules = null;

		/// <summary>
		///   [取得] 使用者選取傭金/手續費規則類別
		/// </summary>
		internal ICommission Commission {
			get {
				return __cCommission;
			}
		}

		internal frmCommissionRuleSettings(AbstractOrderService orderService) {
			InitializeComponent();

			__cOrderService = orderService;
		}

		private void LoadRuls() {
			if (__cRules != null) {
				comboRules.Items.Clear();
				txtMemo.Text = string.Empty;
			}

			__cRules = __cOrderService.GetRuleItems(__cRuleType);
			if (__cRules != null) {
				int iCount = __cRules.Count;
				if (iCount > 0) {
					for (int i = 0; i < iCount; i++) {
						comboRules.Items.Add(__cRules[i].Name);
					}

					comboRules.SelectedIndex = 0;
				}
			}
		}

		private void frmCommissionRuleSettings_Load(object sender, EventArgs e) {
			comboType.SelectedIndex = 0;
		}

		private void btnOK_Click(object sender, EventArgs e) {
			int iIndex = comboRules.SelectedIndex;
			
			RuleBase cRule = new RuleBase();
			cRule.ClassName = __cRules[iIndex].ClassName;

			__cCommission = __cOrderService.CreateCommission(cRule);
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void comboRules_SelectedIndexChanged(object sender, EventArgs e) {
			int iIndex = comboRules.SelectedIndex;
			txtMemo.Text = __cRules[iIndex].Comment;
		}

		private void comboType_SelectedIndexChanged(object sender, EventArgs e) {
			int iIndex = comboType.SelectedIndex;
			switch (iIndex) {
				case 0:
					__cRuleType = ERuleType.Commission;
					break;
				case 1:
					__cRuleType = ERuleType.Fee;
					break;
			}

			LoadRuls();
		}
	}
}