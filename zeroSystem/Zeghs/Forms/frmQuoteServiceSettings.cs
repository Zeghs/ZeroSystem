using System;
using System.Windows.Forms;
using Zeghs.Services;

namespace Zeghs.Forms {
	internal partial class frmQuoteServiceSettings : Form {
		private AbstractQuoteService __cQuoteService = null;

		internal frmQuoteServiceSettings(AbstractQuoteService quoteService) {
			__cQuoteService = quoteService;

			InitializeComponent();
		}

		private void frmQuoteServiceSettings_Load(object sender, EventArgs e) {
			txtExchange.Text = __cQuoteService.ExchangeName;
			txtDataSource.Text = __cQuoteService.DataSource;
			txtRemoteIP.Text = __cQuoteService.RemoteIP;
			txtRemotePort.Text = __cQuoteService.RemotePort.ToString();
			txtAccount.Text = __cQuoteService.UserId;
			txtPassword.Text = __cQuoteService.Password;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			__cQuoteService.Dispose();
			__cQuoteService = null;

			this.DialogResult = DialogResult.Cancel;
		}

		private void btnSave_Click(object sender, EventArgs e) {
			__cQuoteService.DataSource = txtDataSource.Text;
			__cQuoteService.RemoteIP = txtRemoteIP.Text;

			int iRemotePort = 0;
			int.TryParse(txtRemotePort.Text, out iRemotePort);
			__cQuoteService.RemotePort = iRemotePort;

			__cQuoteService.UserId = txtAccount.Text;
			__cQuoteService.Password = txtPassword.Text;

			__cQuoteService.Save();  //儲存
			__cQuoteService.Dispose();
			__cQuoteService = null;

			this.DialogResult = DialogResult.OK;
		}
	}
}