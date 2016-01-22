using System;
using System.Windows.Forms;
using SourceGrid;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmQuoteManager : Form {
		internal frmQuoteManager() {
			InitializeComponent();
			InitializeSourceGrid();
		}

		private void frmQuoteManager_Load(object sender, EventArgs e) {
			RefreshQuotePlugin();
		}

		private void RefreshQuotePlugin() {
			source.Clear();

			QuoteManager.Manager.Refresh("plugins\\quotes");
			QuoteServiceInformation[] cQuoteInfos = QuoteManager.Manager.GetQuoteServiceInformations();
			foreach (QuoteServiceInformation cQuoteInfo in cQuoteInfos) {
				source.Add(cQuoteInfo);
			}

			this.dataGrid.Selection.SelectRow(1, true);
			source.Refresh();
		}

		private void btnClose_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnEnabled_Click(object sender, EventArgs e) {
			QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;
			if (!cQuoteInfo.Enabled) {
				bool bLogin = QuoteManager.Manager.StartQuoteService(cQuoteInfo);
				if (bLogin) {
					btnEnabled.Visible = !cQuoteInfo.Enabled;
					btnDisabled.Visible = cQuoteInfo.Enabled;
					btnReLogin.Enabled = cQuoteInfo.Enabled;
					btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;
					source.Refresh();
				}
			}
		}

		private void btnDisabled_Click(object sender, EventArgs e) {
			QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;
			if (cQuoteInfo.Enabled) {
				QuoteManager.Manager.StopQuoteService(cQuoteInfo);
				
				btnEnabled.Visible = !cQuoteInfo.Enabled;
				btnDisabled.Visible = cQuoteInfo.Enabled;
				btnReLogin.Enabled = cQuoteInfo.Enabled;
				btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;
				source.Refresh();
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e) {
			RefreshQuotePlugin();
		}

		private void btnRefreshSymbol_Click(object sender, EventArgs e) {
			QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;
			if (cQuoteInfo.Enabled) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(cQuoteInfo.DataSource);
				if (cService != null) {
					cService.SymbolUpdate();
				}
			}
		}

		private void btnSetting_Click(object sender, EventArgs e) {
			QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;
			frmQuoteServiceSettings cQuoteServiceSettings = new frmQuoteServiceSettings(QuoteManager.Manager.GetQuoteService(cQuoteInfo));
			cQuoteServiceSettings.ShowDialog();
		}

		private void btnReLogin_Click(object sender, EventArgs e) {
			QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;
			if (cQuoteInfo.Enabled) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(cQuoteInfo.DataSource);
				if (cService != null) {
					cService.Logout();
					cService.Login();
				}
			}
		}

		private void dataGrid_onSelectionChanged(object sender, RangeRegionChangedEventArgs e) {
			if (dataGrid.SelectedDataRows.Length > 0) {
				QuoteServiceInformation cQuoteInfo = dataGrid.SelectedDataRows[0] as QuoteServiceInformation;

				txtDescription.Text = cQuoteInfo.Description;
				btnEnabled.Visible = !cQuoteInfo.Enabled;
				btnDisabled.Visible = cQuoteInfo.Enabled;
				btnReLogin.Enabled = cQuoteInfo.Enabled;
				btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;
			}
		}
	}
}