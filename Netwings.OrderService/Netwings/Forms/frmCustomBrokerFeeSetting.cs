using System;
using System.Windows.Forms;

namespace Netwings.Forms {
	public partial class frmCustomBrokerFeeSetting : Form {
		internal double Fee {
			get;
			private set;
		}

		public frmCustomBrokerFeeSetting() {
			InitializeComponent();
		}

		private void bOK_Click(object sender, EventArgs e) {
			double dFee = 0;
			double.TryParse(txtFee.Text, out dFee);
			this.Fee = dFee;

			this.DialogResult = DialogResult.OK;
		}

		private void bCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
