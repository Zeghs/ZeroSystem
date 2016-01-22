using System;
using System.Windows.Forms;

namespace Taiwan.Forms {
	public partial class frmTaiwanContractSetting : Form {
		internal TimeSpan CloseTime {
			get;
			set;
		}

		public frmTaiwanContractSetting() {
			InitializeComponent();
		}

		private void frmTaiwanContractSetting_Load(object sender, EventArgs e) {
			numericHour.Value = this.CloseTime.Hours;
			numericMinute.Value = this.CloseTime.Minutes;
			numericSecond.Value = this.CloseTime.Seconds;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e) {
			TimeSpan cTime = TimeSpan.Zero;
			
			try {
				cTime = new TimeSpan(Decimal.ToInt32(numericHour.Value), Decimal.ToInt32(numericMinute.Value), Decimal.ToInt32(numericSecond.Value));
				this.CloseTime = cTime;

				this.DialogResult = DialogResult.OK;
			} catch(Exception __errExcep) {
				MessageBox.Show(__errExcep.Message, "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}