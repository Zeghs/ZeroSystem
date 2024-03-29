﻿using System;
using System.Windows.Forms;
using SourceGrid;
using Zeghs.Data;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmQuoteManager : Form {
		private bool __bSetupChanged = false;

		internal bool SetupChanged {
			get {
				return __bSetupChanged;
			}
		}

		internal frmQuoteManager() {
			InitializeComponent();
			InitializeSourceGrid();
		}

		private void CheckAndUpdateAccount(_QuoteServiceInfo info) {
			if (info.Enabled) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(info.DataSource);
				info.LogonId = cService.UserId;
			} else {
				info.LogonId = string.Empty;
			}
		}

		private void RefreshQuotePlugin() {
			source.Clear();

			QuoteManager.Manager.Refresh("plugins\\quotes");
			QuoteServiceInformation[] cQuoteInfos = QuoteManager.Manager.GetQuoteServiceInformations();
			foreach (QuoteServiceInformation cQuoteInfo in cQuoteInfos) {
				_QuoteServiceInfo cData = new _QuoteServiceInfo(cQuoteInfo);
				CheckAndUpdateAccount(cData);

				source.Add(cData);
			}

			this.dataGrid.Selection.SelectRow(1, true);
			source.Refresh();
		}

		private void frmQuoteManager_Load(object sender, EventArgs e) {
			RefreshQuotePlugin();

			timer.Enabled = true;
		}

		private void btnClose_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnEnabled_Click(object sender, EventArgs e) {
			_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;
			if (!cQuoteInfo.Enabled) {
				bool bLogin = QuoteManager.Manager.StartQuoteService(cQuoteInfo.GetInformation());
				if (bLogin) {
					btnEnabled.Visible = !cQuoteInfo.Enabled;
					btnDisabled.Visible = cQuoteInfo.Enabled;
					btnReLogin.Enabled = cQuoteInfo.Enabled;
					btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;
					
					CheckAndUpdateAccount(cQuoteInfo);
					source.Refresh();
				
					__bSetupChanged = true;
				}
			}
		}

		private void btnDisabled_Click(object sender, EventArgs e) {
			DialogResult cResult = MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (cResult == DialogResult.Yes) {
				_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;
				if (cQuoteInfo.Enabled) {
					QuoteManager.Manager.StopQuoteService(cQuoteInfo.GetInformation());

					btnEnabled.Visible = !cQuoteInfo.Enabled;
					btnDisabled.Visible = cQuoteInfo.Enabled;
					btnReLogin.Enabled = cQuoteInfo.Enabled;
					btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;

					CheckAndUpdateAccount(cQuoteInfo);
					source.Refresh();

					__bSetupChanged = true;
				}
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e) {
			RefreshQuotePlugin();
		}

		private void btnRefreshSymbol_Click(object sender, EventArgs e) {
			_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;
			if (cQuoteInfo.Enabled) {
				AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(cQuoteInfo.DataSource);
				if (cService != null) {
					cService.SymbolUpdate();
				}
			}
		}

		private void btnSetting_Click(object sender, EventArgs e) {
			_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;
			frmQuoteServiceSettings cQuoteServiceSettings = new frmQuoteServiceSettings(QuoteManager.Manager.GetQuoteService(cQuoteInfo.GetInformation()));
			
			DialogResult cResult = cQuoteServiceSettings.ShowDialog();
			if (cResult == DialogResult.OK && cQuoteInfo.Enabled) {
				QuoteManager.Manager.StopQuoteService(cQuoteInfo.GetInformation());
				
				btnEnabled.Visible = !cQuoteInfo.Enabled;
				btnDisabled.Visible = cQuoteInfo.Enabled;
				btnReLogin.Enabled = cQuoteInfo.Enabled;
				btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;

				__bSetupChanged = true;

				CheckAndUpdateAccount(cQuoteInfo);
				source.Refresh();
			}
		}

		private void btnReLogin_Click(object sender, EventArgs e) {
			_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;
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
				_QuoteServiceInfo cQuoteInfo = dataGrid.SelectedDataRows[0] as _QuoteServiceInfo;

				txtDescription.Text = cQuoteInfo.Description;
				btnEnabled.Visible = !cQuoteInfo.Enabled;
				btnDisabled.Visible = cQuoteInfo.Enabled;
				btnReLogin.Enabled = cQuoteInfo.Enabled;
				btnRefreshSymbol.Enabled = cQuoteInfo.Enabled;
			}
		}

		private void timer_Tick(object sender, EventArgs e) {
			timer.Enabled = false;

			int iCount = source.Count;
			for (int i = 0; i < iCount; i++) {
				_QuoteServiceInfo cQuoteInfo = source.GetItemAt(i);
				if (cQuoteInfo.Enabled) {
					AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(cQuoteInfo.DataSource);
					if(cService != null) {
						long lPacketCount = cService.PacketCount;
						cQuoteInfo.SetPacketCount(lPacketCount);
					}
				}
			}

			source.Refresh();
			timer.Enabled = true;
		}
	}
}