using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using Zeghs.Events;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmWelcome : Form {
		internal frmWelcome() {
			InitializeComponent();
		}

		private void frmWelcome_Load(object sender, EventArgs e) {
			this.TopMost = true;
			labVersion.Text = Application.ProductVersion;
		}

		private void frmWelcome_Shown(object sender, EventArgs e) {
			//讀取使用者自訂策略與模組
			ScriptManager.Manager.onAdditionScript += ScriptManager_onAdditionScript;
			ScriptManager.Manager.onLoadScriptCompleted += ScriptManager_onLoadScriptCompleted;

			labMessage.Text = "Complier script...";

			Task.Factory.StartNew(() => {
				ScriptManager.Manager.LoadScripts();
			});
		}

		private void Timer_onElapsed(object sender, System.Timers.ElapsedEventArgs e) {
			System.Timers.Timer cTimer = sender as System.Timers.Timer;
			cTimer.Dispose();

			this.BeginInvoke((MethodInvoker) delegate {
				this.DialogResult = DialogResult.OK;
			});
		}

		private void ScriptManager_onAdditionScript(object sender, AddationScriptEvent e) {
			labMessage.BeginInvoke((MethodInvoker) delegate {
				labMessage.Text = string.Format("Loading  {0}...", e.ScriptInformation.Name);
			});
		}

		private void ScriptManager_onLoadScriptCompleted(object sender, EventArgs e) {
			labMessage.BeginInvoke((MethodInvoker) delegate {
				labMessage.Text = "Completed";
			});

			System.Timers.Timer cTimer = new System.Timers.Timer(3000);
			cTimer.AutoReset = false;
			cTimer.Elapsed += Timer_onElapsed;
			cTimer.Start();
		}
	}
}