using System;
using System.Windows.Forms;

namespace Taiwan.Forms {
	public partial class frmTaiwanIndexFuturePriceScaleSetting : Form {
		internal double[] PriceScales {
			get;
			set;
		}

		public frmTaiwanIndexFuturePriceScaleSetting() {
			this.PriceScales = new double[] { 1, 1 };

			InitializeComponent();
		}

		private void frmTaiwanIndexFuturePriceScaleSetting_Load(object sender, EventArgs e) {
			txtPriceScale.Text = this.PriceScales[0].ToString();
			txtMinJump.Text = this.PriceScales[1].ToString();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e) {
			double dPriceScale = 0, dMinJump = 0;

			if (!double.TryParse(txtPriceScale.Text, out dPriceScale)) {
				dPriceScale = 1;
			}

			if (!double.TryParse(txtMinJump.Text, out dMinJump)) {
				dMinJump = 1;
			}

			this.PriceScales[0] = dPriceScale;
			this.PriceScales[1] = dMinJump;

			this.DialogResult = DialogResult.OK;
		}
	}
}