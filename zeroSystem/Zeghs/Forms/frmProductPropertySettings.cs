using System;
using System.Windows.Forms;
using System.Collections.Generic;
using SourceGrid;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmProductPropertySettings : Form {
		private string __sDataSource = null;
		private string __sExchangeName = null;
		private Product __cProduct = null;
		private AbstractProductProperty __cProperty = null;

		internal frmProductPropertySettings() {
			InitializeComponent();
			InitializeGridCaution();
			InitializeGridSession();
		}

		internal void SetParameters(string exchangeName, string dataSource, Product product, AbstractProductProperty property) {
			__sExchangeName = exchangeName;
			__sDataSource = dataSource;
			__cProduct = product;
			__cProperty = property;

			//設定保證金資訊
			sourceCautions = new SimpleBoundList<CautionMoney>(__cProperty.CautionMoneys);
			sourceCautions.AllowNew = true;
			sourceCautions.AllowEdit = true;
			sourceCautions.AllowDelete = true;

			//設定開收盤時間
			sourceSessions = new SimpleBoundList<SessionObject>(__cProperty.Sessions);
			sourceSessions.AllowNew = true;
			sourceSessions.AllowEdit = true;
			sourceSessions.AllowDelete = true;
		}

		private string GetRuleName(RuleBase rule) {
			if (rule == null) {
				return string.Empty;
			} else {
				string sClassName = rule.ClassName;
				int iIndex = sClassName.LastIndexOf('.');
				if (iIndex == -1) {
					return sClassName;
				} else {
					return sClassName.Substring(iIndex + 1);
				}
			}
		}

		private void RefreshRules() {
			txtContractTimeRule.Text = GetRuleName(__cProperty.ContractRule);
			txtPriceScaleRule.Text = GetRuleName(__cProperty.PriceScaleRule);
			txtTaxRule.Text = GetRuleName(__cProperty.TaxRule);
		}

		private void frmProductPropertySettings_Load(object sender, EventArgs e) {
			List<AbstractQuoteService> cServices = QuoteManager.Manager.QuoteServices;
			int iCount = cServices.Count;
			for (int i = 0; i < iCount; i++) {
				comboDataSource.Items.Add(cServices[i].DataSource);
			}

			//設定資訊至控制項
			if(__sDataSource != null) {
				int iIndex = comboDataSource.Items.IndexOf(__sDataSource);
				comboDataSource.SelectedIndex = (iIndex == -1) ? 0 : iIndex;
			}
			
			//一般設定頁籤
			txtProduct.Text = __cProduct.SymbolId;
			txtCommodity.Text = __cProduct.CommodityId;
			txtDescription.Text = __cProperty.Description;
			txtBigPointValue.Text = __cProperty.BigPointValue.ToString();
			txtDailyLimit.Text = __cProperty.DailyLimit.ToString();
			gridCaution.DataSource = sourceCautions;

			//規則設定頁籤
			btnContractTimeRule.Tag = ERuleType.Contract;
			btnPriceScaleRule.Tag = ERuleType.PriceScale;
			btnTaxRule.Tag = ERuleType.Tax;
			RefreshRules();

			//交易時段設定頁籤
			gridSession.DataSource = sourceSessions;
		}

		private void btnOK_Click(object sender, EventArgs e) {
			double dValue = 0;
			if(double.TryParse(txtBigPointValue.Text, out dValue)) {
				__cProperty.BigPointValue = dValue;
			}

			if (double.TryParse(txtDailyLimit.Text, out dValue)) {
				__cProperty.DailyLimit = dValue;
			}
			__cProperty.Description = txtDescription.Text;

			string sDataSource = comboDataSource.Text;
			sDataSource = (sDataSource.Length == 0) ? null : sDataSource;
			if (sDataSource == null) {
				DialogResult cResult = MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (cResult == DialogResult.No) {
					this.DialogResult = DialogResult.Cancel;
				}
			}
 
			//變更設定值
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(__sExchangeName);
			cExchange.AddProperty(__cProperty, sDataSource);

			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnCautionCreate_Click(object sender, EventArgs e) {
			CautionMoney cCaution = new CautionMoney();
			cCaution.Description = "Memo";

			sourceCautions.Add(cCaution);
			sourceCautions.Refresh();
		}

		private void btnCautionDelete_Click(object sender, EventArgs e) {
			if (gridCaution.SelectedDataRows.Length > 0) {
				int[] iSelectedRows = gridCaution.Selection.GetSelectionRegion().GetRowsIndex();
				sourceCautions.RemoveAt(iSelectedRows[0] - 1);    //第 0 列為標題所以選定的列會從 1 開始
				sourceCautions.Refresh();
			}
		}

		private void btnSessionCreate_Click(object sender, EventArgs e) {
			SessionObject cSession = new SessionObject();
			cSession.EndDay = DayOfWeek.Monday;
			cSession.EndTime = TimeSpan.Zero;
			cSession.StartDay = DayOfWeek.Monday;
			cSession.StartTime = TimeSpan.Zero;

			sourceSessions.Add(cSession);
			sourceSessions.Refresh();
		}

		private void btnSessionDelete_Click(object sender, EventArgs e) {
			if (gridSession.SelectedDataRows.Length > 0) {
				int[] iSelectedRows = gridSession.Selection.GetSelectionRegion().GetRowsIndex();
				sourceSessions.RemoveAt(iSelectedRows[0] - 1);  //第 0 列為標題所以選定的列會從 1 開始
				sourceSessions.Refresh();
			}
		}

		private void RuleButton_onClick(object sender, EventArgs e) {
			Button cButton = sender as Button;
			ERuleType cRuleType = (ERuleType) cButton.Tag;

			frmProductRuleSettings frmProductRuleSettings = new frmProductRuleSettings(__sExchangeName, cRuleType, __cProperty);
			frmProductRuleSettings.ShowDialog();

			RefreshRules();
		}

		private void checkDaylight_CheckedChanged(object sender, EventArgs e) {
			bool bEnabled = checkDaylight.Checked;
			pickerDaylightStart.Enabled = bEnabled;
			pickerDaylightEnd.Enabled = bEnabled;

			if (gridSession.SelectedDataRows.Length > 0) {
				SessionObject cSession = gridSession.SelectedDataRows[0] as SessionObject;
				DaylightTime cDaylightTime = cSession.Daylight;

				if (bEnabled) {
					if (cDaylightTime == null) {
						cDaylightTime = new DaylightTime();
						cDaylightTime.StartDate = DateTime.UtcNow;
						cDaylightTime.EndDate = DateTime.UtcNow;

						cSession.Daylight = cDaylightTime;
					}
				} else {
					if (cDaylightTime != null) {
						cSession.Daylight = null;
					}
				}
			}
		}

		private void gridSession_onSelectionChanged(object sender, RangeRegionChangedEventArgs e) {
			if (gridSession.SelectedDataRows.Length > 0) {
				SessionObject cSession = gridSession.SelectedDataRows[0] as SessionObject;

				DaylightTime cDaylightTime = cSession.Daylight;
				bool bEnabled = cDaylightTime != null;
				checkDaylight.Checked = bEnabled;

				if (bEnabled) {
					pickerDaylightStart.Value = cDaylightTime.StartDate;
					pickerDaylightEnd.Value = cDaylightTime.EndDate;
				}
			}
		}
	}
}