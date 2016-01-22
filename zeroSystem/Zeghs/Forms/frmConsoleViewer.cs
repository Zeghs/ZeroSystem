using System;
using System.Drawing;
using System.Windows.Forms;
using Zeghs.Events;

namespace Zeghs.Forms {
	internal partial class frmConsoleViewer : Form {
		internal frmConsoleViewer() {
			InitializeComponent();
			InitializeSourceGrid_Log();
			InitializeConsoleOutput();
		}

		private void WriteText(string data, bool isNewLine) {
			if (txtOutput.Text.Length > 10240) {
				txtOutput.Clear();
			}

			txtOutput.AppendText(data);
			if (isNewLine) {
				txtOutput.AppendText(Environment.NewLine);
			}

			if (txtOutput.Visible) {
				txtOutput.SelectionStart = txtOutput.TextLength;
				txtOutput.ScrollToCaret();
			}
		}

		private void frmConsoleViewer_Load(object sender, EventArgs e) {
			__cLogService.onLogEvent += LogService_onLogEvent;
			__iFixGridBottomCount = this.dataGrid_Log.ClientSize.Height / 21 - 2;
		}

		private void frmConsoleViewer_Resize(object sender, EventArgs e) {
			this.dataGrid_Log.Columns[2].Width = pageItem_LOG.ClientSize.Width - 188;
		}

		private void dataGrid_Log_VisibleChanged(object sender, EventArgs e) {
			__cLogService.Logs.Refresh();
			this.dataGrid_Log.VScrollBar.Value = this.dataGrid_Log.VScrollBar.Maximum - __iFixGridBottomCount;  //捲動到最底部
		}

		private void txtOutput_VisibleChanged(object sender, EventArgs e) {
			txtOutput.SelectionStart = txtOutput.TextLength;
			txtOutput.ScrollToCaret();
		}

		private void LogService_onLogEvent(object sender, EventArgs e) {
			if (!this.IsDisposed) {
				if (this.dataGrid_Log.Visible) {
					if (this.dataGrid_Log.InvokeRequired) {
						this.dataGrid_Log.Invoke((MethodInvoker) delegate {
							__cLogService.Logs.Refresh();
							this.dataGrid_Log.VScrollBar.Value = this.dataGrid_Log.VScrollBar.Maximum - __iFixGridBottomCount;  //捲動到最底部
						});
					} else {
						__cLogService.Logs.Refresh();
						this.dataGrid_Log.VScrollBar.Value = this.dataGrid_Log.VScrollBar.Maximum - __iFixGridBottomCount;  //捲動到最底部
					}
				}
			}
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