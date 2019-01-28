using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Zeghs.Forms {
	internal partial class Loading : Form {
		internal static void Create(Action action) {
			Loading cLoading = new Loading(action);
			cLoading.ShowDialog();
		}

		private Action __cTask = null;

		private Loading(Action task) {
			__cTask = task;

			InitializeComponent();
		}

		private void Loading_Shown(object sender, EventArgs e) {
			Task.Factory.StartNew(() => {
				__cTask();

				this.BeginInvoke((MethodInvoker) delegate() {
					this.Close();
					this.Dispose();
				});
			});
		}
	}
}