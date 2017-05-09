using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Zeghs.Events;

namespace Zeghs.Forms {
	internal partial class frmConsoleViewer : DockContent {
		internal frmConsoleViewer() {
			InitializeComponent();
			InitializeConsoleOutput();
		}

		private void ScrollBottom() {
			txtOutput.SelectionStart = txtOutput.TextLength;
			txtOutput.ScrollToCaret();
		}

		private void WriteText(string data, bool isNewLine) {
			if (txtOutput.Text.Length > 10240) {
				txtOutput.Clear();
			}

			txtOutput.AppendText(data);
			if (isNewLine) {
				txtOutput.AppendText(Environment.NewLine);
			}

			ScrollBottom();
		}

		private void txtOutput_VisibleChanged(object sender, EventArgs e) {
			ScrollBottom();
		}

		private void OutputWriter_onOutputData(object sender, OutputDataEvent e) {
			if (!this.IsDisposed) {
				if (txtOutput.InvokeRequired) {
					txtOutput.Invoke((MethodInvoker) delegate {
						WriteText(e.Data, e.IsNewLine);
					});
				} else {
					WriteText(e.Data, e.IsNewLine);
				}
			}
		}
	}
}