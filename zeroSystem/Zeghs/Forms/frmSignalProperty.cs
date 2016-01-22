using System;
using System.Windows.Forms;
using SourceGrid;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmSignalProperty : Form {
		private SignalProperty __cProperty = null;

		internal frmSignalProperty() {
			InitializeComponent();
			InitializeSourceGrid();
		}

		internal void SetScriptSetting(ScriptSetting setting) {
			__cProperty = (setting as SignalSetting).Property;
		}

		private void frmSignalProperty_Load(object sender, EventArgs e) {
			OrderServiceInformation[] cOrderInfos = OrderManager.Manager.GetOrderServiceInformations();

			int iLength = cOrderInfos.Length;
			for (int i = 0; i < iLength; i++) {
				source.Add(cOrderInfos[i]);
			}
			source.Refresh();
		}

		private void btnOK_Click(object sender, EventArgs e) {
			int iValue = 0;
			double dValue = 0;
			if (int.TryParse(txtDefaultContracts.Text, out iValue)) {
				__cProperty.DefaultContracts = iValue;
			}

			if (double.TryParse(txtInitCapital.Text, out dValue)) {
				__cProperty.InitialCapital = dValue;
			}

			if (int.TryParse(txtMaxBarsReference.Text, out iValue)) {
				__cProperty.MaximumBarsReference = iValue;
			}

			if (dataGrid.SelectedDataRows.Length > 0) {
				OrderServiceInformation cOrderInfo = dataGrid.SelectedDataRows[0] as OrderServiceInformation;
				__cProperty.OrderSource = string.Format("{0};{1}", cOrderInfo.ModuleName, comboOrderService.Text);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void dataGrid_onSelectionChanged(object sender, RangeRegionChangedEventArgs e) {
			if (dataGrid.SelectedDataRows.Length > 0) {
				OrderServiceInformation cOrderInfo = dataGrid.SelectedDataRows[0] as OrderServiceInformation;

				comboOrderService.Items.Clear();

				string[] sServices = cOrderInfo.Services;
				int iLength = sServices.Length;
				if (iLength == 0) {
					btnOK.Enabled = false;
				} else {
					for (int i = 0; i < iLength; i++) {
						comboOrderService.Items.Add(sServices[i]);
					}
					comboOrderService.SelectedIndex = 0;

					btnOK.Enabled = true;
				}
			}
		}
	}
}