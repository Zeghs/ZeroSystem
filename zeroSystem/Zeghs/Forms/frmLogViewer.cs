using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Zeghs.Events;

namespace Zeghs.Forms {
	internal partial class frmLogViewer : DockContent {
		internal frmLogViewer() {
			InitializeComponent();
			InitializeSourceGrid_Log();
		}

		private void ScrollBottom() {
			int iGridCount = this.dataGrid_Log.Height / 21 - 2;
			if (iGridCount > 0) {
				int iValue = this.dataGrid_Log.VScrollBar.Maximum - iGridCount;
				if (iValue > 0) {
					this.dataGrid_Log.VScrollBar.Value = iValue;  //捲動到最底部
				}
			}
		}

		private void frmLogViewer_Load(object sender, EventArgs e) {
			__cLogService.onLogEvent += LogService_onLogEvent;
		}

		private void frmLogViewer_Resize(object sender, EventArgs e) {
			this.dataGrid_Log.Columns[2].Width = this.dataGrid_Log.ClientSize.Width - 188;
			
			ScrollBottom();
		}

		private void LogService_onLogEvent(object sender, EventArgs e) {
			if (!this.IsDisposed) {
				if (this.dataGrid_Log.InvokeRequired) {
					this.dataGrid_Log.BeginInvoke((MethodInvoker) delegate {
						__cLogService.Logs.Refresh();
						ScrollBottom();
					});
				} else {
					__cLogService.Logs.Refresh();
					ScrollBottom();
				}
			}
		}
	}
}