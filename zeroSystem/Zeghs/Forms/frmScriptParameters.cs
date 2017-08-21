﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Orders;
using Zeghs.Services;

namespace Zeghs.Forms {
	public partial class frmScriptParameters : Form {
		private List<InputAttribute> __cParameters = null;
		private AbstractOrderService __cOrderService = null;

		public frmScriptParameters() {
			InitializeComponent();
			InitializeSourceGrid();
		}

		internal void SetParameters(AbstractOrderService orderService, List<InputAttribute> args) {
			__cParameters = args;
			__cOrderService = orderService;

			int iCount = 0;
			List<ICommission> cCommissions = __cOrderService.Commissions;
			if (cCommissions != null) {
				iCount = cCommissions.Count;
				for (int i = 0; i < iCount; i++) {
				}
			}

			iCount = args.Count;
			for (int i = 0; i < iCount; i++) {
				InputAttribute cInput = args[i];

				_ParameterInfo cParameter = new _ParameterInfo();
			 	cParameter.Comment = (cInput.Comment == null) ? cInput.Name : cInput.Comment;
				cParameter.Value = cInput.Value.ToString();

				source.Add(cParameter);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e) {
			int iCount = __cParameters.Count;
			for (int i = 0; i < iCount; i++) {
				InputAttribute cInput = __cParameters[i];
				_ParameterInfo cParameter = source[i] as _ParameterInfo;

				cInput.SetValue(cParameter.Value);
			}

			this.DialogResult = DialogResult.OK;
		}

		private void toolItem_Create_Click(object sender, EventArgs e) {
			frmCommissionRuleSettings frmCommissionRuleSettings = new frmCommissionRuleSettings(__cOrderService);
			DialogResult cResult = frmCommissionRuleSettings.ShowDialog();
			if (cResult == DialogResult.OK) {

			}
		}
	}
}