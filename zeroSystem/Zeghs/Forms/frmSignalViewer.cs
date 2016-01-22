using System;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Utils;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmSignalViewer : frmChildBase {
		private bool __bShown = false;
		private ProfileSetting __cProfile = null;
		private SignalObject __cSignalObject = null;

		internal frmSignalViewer() {
			this.ShowTitle = true;
			__cTradeService = new TradeService();
			__cTradeService.onUpdate += TradeService_onUpdate;

			InitializeComponent();
			InitializeSourceGrid_Trust();
			InitializeSourceGrid_Trade();
			InitializeSourceGrid_History();
		}

		internal void SetProfileSetting(ProfileSetting profile) {
			__cProfile = profile;
		}

		private void CreateSignalObject() {
			__cSignalObject = ScriptManager.Manager.CreateScript(__cProfile.ScriptName) as SignalObject;
			if (__cSignalObject == null) {

			} else {
				string sTitle = string.Format("{0}[{1}]", __cSignalObject.About.Comment, __cSignalObject.About.Version);
				if (this.InvokeRequired) {
					this.Invoke((MethodInvoker) delegate {
						this.Text = sTitle;
						this.Invalidate(false);
					});
				} else {
					this.Text = sTitle;
					this.Invalidate(false);
				}
				
				__cSignalObject.onScriptParameters += SignalObject_onScriptParameters;
				__cSignalObject.onTradeResponse += SignalObject_onTradeResponse;

				OutputWriter cOutputWriter = __cSignalObject.Output as OutputWriter;
				cOutputWriter.onOutputData += OutputWriter_onOutputData;

				SignalSetting cSignalSetting = __cProfile.Script as SignalSetting;
				__cSignalObject.ApplyProperty(cSignalSetting.Property);
				__cSignalObject.AddDataStreams(RequestSetting.Convert(cSignalSetting.DataRequests));
			}
		}

		private void RefreshGrid(SourceGrid.DataGrid grid, AbstractBoundList<_TradeInfo> source) {
			if (!this.IsDisposed) {
				if (grid.Visible) {
					if (grid.InvokeRequired) {
						grid.Invoke((MethodInvoker) delegate {
							source.Refresh();
						});
					} else {
						source.Refresh();
					}
				}
			}
		}

		private void ScrollBottom(SourceGrid.DataGrid grid) {
			int iValue = grid.VScrollBar.Maximum - __iFixGridBottomCount;
			if (iValue > 0) {
				grid.VScrollBar.Value = iValue;  //捲動到最底部
			}
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

		private void frmSignalViewer_FormClosing(object sender, FormClosingEventArgs e) {
			ProfileManager.Manager.RemoveProfile(__cProfile.ProfileId);  //關閉就移除 Profile 設定檔
		}

		private void frmSignalViewer_Load(object sender, EventArgs e) {
			__iFixGridBottomCount = this.dataGrid_Trade.ClientSize.Height / 21 - 2;

			Task.Factory.StartNew(() => {
				CreateSignalObject();
			});
		}

		private void frmSignalViewer_Move(object sender, EventArgs e) {
			if (__bShown) {
				WindowStatus cWindow = __cProfile.Window;
				cWindow.Left = this.Left;
				cWindow.Top = this.Top;
			}
		}

		private void frmSignalViewer_Resize(object sender, EventArgs e) {
			tabControl.Top = this.ClientSize.Height - tabControl.Height - 1;
			tabControl.Width = this.ClientSize.Width - 1;

			if (__bShown) {
				WindowStatus cWindow = __cProfile.Window;
				cWindow.Height = this.Height;
				cWindow.Width = this.Width;
				cWindow.WindowState = this.WindowState;
			}
		}

		private void frmSignalViewer_Shown(object sender, EventArgs e) {
			WindowStatus cWindow = __cProfile.Window;
			if (cWindow.Height == 0 || cWindow.Width == 0) {
				cWindow.Left = this.Left;
				cWindow.Top = this.Top;
				cWindow.Height = this.Height;
				cWindow.Width = this.Width;
			} else {
				this.Left = cWindow.Left;
				this.Top = cWindow.Top;

				FormWindowState cState = cWindow.WindowState;
				switch (cState) {
					case FormWindowState.Maximized:
						this.WindowState = FormWindowState.Maximized;
						break;
					case FormWindowState.Minimized:
						this.WindowState = FormWindowState.Minimized;
						break;
					case FormWindowState.Normal:
						this.WindowState = FormWindowState.Normal;
						this.Height = cWindow.Height;
						this.Width = cWindow.Width;
						break;
				}
			}
			__bShown = true;
		}

		private void dataGrid_Trust_VisibleChanged(object sender, EventArgs e) {
			RefreshGrid(this.dataGrid_Trust, __cTradeService.Trusts);
			ScrollBottom(this.dataGrid_Trust);
		}

		private void dataGrid_Trade_VisibleChanged(object sender, EventArgs e) {
			RefreshGrid(this.dataGrid_Trade, __cTradeService.Opens);
			ScrollBottom(this.dataGrid_Trade);
		}

		private void dataGrid_History_VisibleChanged(object sender, EventArgs e) {
			RefreshGrid(this.dataGrid_History, __cTradeService.Closes);
			ScrollBottom(this.dataGrid_History);
		}

		private void txtOutput_VisibleChanged(object sender, EventArgs e) {
			txtOutput.SelectionStart = txtOutput.TextLength;
			txtOutput.ScrollToCaret();
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

		private void SignalObject_onTradeResponse(object sender, ResponseEvent e) {
			__cTradeService.AddResponse(e);
		}

		private void TradeService_onUpdate(object sender, EventArgs e) {
			RefreshGrid(this.dataGrid_Trust, __cTradeService.Trusts);
			RefreshGrid(this.dataGrid_Trade, __cTradeService.Opens);
			RefreshGrid(this.dataGrid_History, __cTradeService.Closes);
		}

		private void SignalObject_onScriptParameters(object sender, ScriptParametersEvent e) {
			List<InputAttribute> cParameters = e.ScriptParameters;

			int iCount = cParameters.Count;
			List<string> sArgs = __cProfile.Parameters;
			if (sArgs == null) {
				if (iCount > 0) {
					frmScriptParameters frmScriptParameters = new frmScriptParameters();
					frmScriptParameters.SetParameters(cParameters);
					frmScriptParameters.ShowDialog();

					__cProfile.Parameters = new List<string>(iCount);
					sArgs = __cProfile.Parameters;
					for (int i = 0; i < iCount; i++) {
						sArgs.Add(cParameters[i].Value.ToString());
					}
				}
			} else {
				for (int i = 0; i < iCount; i++) {
					cParameters[i].SetValue(sArgs[i]);
				}
			}
		}
	}
}