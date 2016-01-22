using System;
using System.Windows.Forms;

namespace Taiwan.Forms {
	public partial class frmTaiwanTaxSetting : Form {
		internal double TaxRate {
			get;
			set;
		}

		public frmTaiwanTaxSetting(int decimalPlaces, double increment) {
			InitializeComponent();

			numericTax.DecimalPlaces = decimalPlaces;
			numericTax.Increment = new decimal(increment);
		}

		private void frmTaiwanTaxSetting_Load(object sender, EventArgs e) {
			numericTax.Value = new decimal(this.TaxRate);
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}
		
		private void btnOK_Click(object sender, EventArgs e) {
			this.TaxRate = Decimal.ToDouble(numericTax.Value);
			this.DialogResult = DialogResult.OK;
		}
	}
}