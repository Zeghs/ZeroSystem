using System;
using System.Windows.Forms;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmWelcome : Form {
		internal frmWelcome() {
			InitializeComponent();
		}

		private void frmWelcome_Load(object sender, EventArgs e) {
			this.TopMost = true;
			labVersion.Text = Application.ProductVersion;

			System.Timers.Timer cTimer = new System.Timers.Timer(3000);
			cTimer.AutoReset = false;
			cTimer.Elapsed += Timer_onElapsed;
			cTimer.Start();
		}

		private void Timer_onElapsed(object sender, System.Timers.ElapsedEventArgs e) {
			System.Timers.Timer cTimer = sender as System.Timers.Timer;
			cTimer.Dispose();

			this.Invoke((MethodInvoker) delegate {
				this.DialogResult = DialogResult.OK;
			});
		}
	}
}