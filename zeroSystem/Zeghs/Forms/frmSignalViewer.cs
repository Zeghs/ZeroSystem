﻿using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Utils;
using Zeghs.Chart;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmSignalViewer : DockContent {
		private static bool __bCustomsLoaded = false;
		internal static void Create(DockPanel dockPanel, ProfileSetting profile) {
			frmSignalViewer frmSignalViewer = new frmSignalViewer();
			frmSignalViewer.SetProfileSetting(profile);

			WindowStatus cWindow = profile.Window;
			if (cWindow.IsDock) {
				frmSignalViewer.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
			} else {
				frmSignalViewer.Show(dockPanel, new System.Drawing.Rectangle(cWindow.Left, cWindow.Top, cWindow.Width, cWindow.Height));
			}

			if (!__bCustomsLoaded) {  //自訂繪圖工具是否已經載入
				__bCustomsLoaded = true;
				frmMain frmMain = dockPanel.Parent as frmMain;
				frmMain.SetCustomDrawTools(frmSignalViewer.Chart.CustomDrawTools);
			}
		}

		private bool __bShown = false;
		private bool __bLoaded = false;
		private bool __bBindMoved = false;
		private bool __bShowTradeView = false;
		private bool __bForceShowParameters = false;
		private ZChart __cChart = null;
		private ProfileSetting __cProfile = null;
		private SignalObject __cSignalObject = null;
		private List<InputAttribute> __cParameters = null;
		private AbstractOrderService __cOrderService = null;

		internal ZChart Chart {
			get {
				return __cChart;
			}
		}

		internal frmSignalViewer() {
			__cTradeService = new TradeService();
			__cTradeService.onUpdate += TradeService_onUpdate;

			InitializeComponent();
			InitializeSourceGrid_Trust();
			InitializeSourceGrid_Trade();
			InitializeSourceGrid_History();

			__bShowTradeView = GlobalSettings.Base.ShowTradeView;
			menuItemTradeDetails.Checked = __bShowTradeView;
		}

		internal void ConnectQuoteServer() {
			if (__cSignalObject != null) {
				__cSignalObject.ConnectQuoteServer();
			}
		}

		internal void SaveCSV(string file) {
			CsvReport.Save(__cTradeService.Opens, __cTradeService.Closes, file);
		}

		internal void SaveJSON(string file) {
			JsonReport.Save(__cTradeService.Opens, __cTradeService.Closes, file);
		}

		internal void SetProfileSetting(ProfileSetting profile) {
			__cProfile = profile;
		}

		internal void ShowScriptParameters() {
			frmScriptParameters frmScriptParameters = new frmScriptParameters();
			frmScriptParameters.SetParameters(__cOrderService, __cParameters);
			DialogResult cResult = frmScriptParameters.ShowDialog();
			if (cResult == DialogResult.OK) {
				__cSignalObject.UpdateParameters();
			}

			if (__cParameters != null) {
				int iCount = __cParameters.Count;
				if (iCount > 0) {
					__cProfile.Parameters = new List<string>(iCount);
					List<string> sArgs = __cProfile.Parameters;
					for (int i = 0; i < iCount; i++) {
						sArgs.Add(__cParameters[i].Value.ToString());
					}
				}
			}
		}

		protected override void OnDockStateChanged(EventArgs e) {
			base.OnDockStateChanged(e);

			if (this.IsHandleCreated && __bLoaded) {
				this.chart.Parent = FindForm();  //當表單重新建立後, 在指定新的 Parent 給 chart 控制項
			}

			__cProfile.Window.IsDock = !this.IsFloat;
			if (!__bBindMoved && this.IsFloat) {
				__bBindMoved = true;
				this.DockHandler.FloatPane.FloatWindow.Move += frmSignalViewer_Move;
			}
		}

		protected override void OnHandleDestroyed(EventArgs e) {
			if (__bLoaded) {
				this.chart.Parent = null;  //解決 DockPanel 在 Handle Destory 時 chart Handle 也會被 Destory 的問題(如果 Parent 不為 null 會一併 Destory 掉 chart Handle)
			}
			base.OnHandleDestroyed(e);
		}

		private void CreateChartEngine() {
			int iEngineIndex = GlobalSettings.ChartProfile.ChartEngineIndex;
			__cChart = new ZChart(this.chart, PaintManager.Manager.GetChartEngineInformations()[iEngineIndex]);
			__cChart.SetChartProperty(__cProfile.ChartProperty);
		}

		private void CreateSignalObject() {
			__cSignalObject = ScriptManager.Manager.CreateScript(__cProfile.ScriptName, __cChart) as SignalObject;
			if (__cSignalObject != null) {
				string sTitle = string.Format("{0}", __cSignalObject.About.Comment);
				if (this.InvokeRequired) {
					this.BeginInvoke((MethodInvoker) delegate {
						this.Text = sTitle;
						this.Invalidate(false);
					});
				} else {
					this.Text = sTitle;
					this.Invalidate(false);
				}

				__cSignalObject.onReady += SignalObject_onReady;
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
						grid.BeginInvoke((MethodInvoker) delegate {
							source.Refresh();
						});
					} else {
						source.Refresh();
					}
				}
			}
		}

		private void ReSize() {
			tabControl.Visible = __bShowTradeView;
			chart.Width = this.ClientSize.Width;
			chart.Height = this.ClientSize.Height - chart.Top - 1;
			chart.Focus();

			if (__bShowTradeView) {
				tabControl.Height = this.ClientSize.Height / 2;
				tabControl.Top = this.ClientSize.Height - tabControl.Height - 1;
				tabControl.Width = this.ClientSize.Width - 1;

				if (tabControl.Dock == DockStyle.None) {
					chart.Height = tabControl.Top - chart.Top - 1;
				}
			}

			if (__bShown) {
				WindowStatus cWindow = __cProfile.Window;
				cWindow.Height = this.Height;
				cWindow.Width = this.Width;
				cWindow.WindowState = this.WindowState;
				cWindow.IsDock = !this.IsFloat;

				if (this.IsFloat) {
					FloatWindow cFloatWindow = this.DockHandler.FloatPane.FloatWindow;
					cWindow.Left = cFloatWindow.Left;
					cWindow.Top = cFloatWindow.Top;
				}
			}
		}

		private void ScrollBottom() {
			txtOutput.SelectionStart = txtOutput.TextLength;
			txtOutput.ScrollToCaret();
		}

		private void ScrollBottom(SourceGrid.DataGrid grid) {
			int iFixGridBottomCount = grid.ClientSize.Height / 21 - ((grid is Zeghs.Controls.CustomGrid) ? 3 : 2);
			int iValue = grid.VScrollBar.Maximum - iFixGridBottomCount;
			if (iValue > 0) {
				//捲動到最底部
				if (grid.VScrollBar.InvokeRequired) {
					grid.VScrollBar.BeginInvoke((MethodInvoker) delegate {
						VScrollBar cBar = grid.VScrollBar;
						if (iValue >= cBar.Minimum && iValue <= cBar.Maximum) {
							grid.VScrollBar.Value = iValue;
						} else {
							grid.VScrollBar.Minimum = 0;
							grid.VScrollBar.Maximum = 100;
							grid.VScrollBar.Value = 0;
						}
					});
				} else {
					VScrollBar cBar = grid.VScrollBar;
					if (iValue >= cBar.Minimum && iValue <= cBar.Maximum) {
						grid.VScrollBar.Value = iValue;
					} else {
						grid.VScrollBar.Minimum = 0;
						grid.VScrollBar.Maximum = 100;
						grid.VScrollBar.Value = 0;
					}
				}
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
				ScrollBottom();
			}
		}

		private void chart_onMouseUp(object sender, MouseEventArgs e) {
			this.DockHandler.Activate();
		}

		private void frmSignalViewer_DragDrop(object sender, DragEventArgs e) {
			ScriptInformation cInfo = e.Data.GetData("__script") as ScriptInformation;
			frmFormatObject.Create(this.DockPanel, cInfo);
		}

		private void frmSignalViewer_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent("__script")) {
				e.Effect = DragDropEffects.Move;
			}
		}

		private void frmSignalViewer_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {  //如果是使用者關閉
				ProfileManager.Manager.RemoveProfile(__cProfile.ProfileId);  //關閉就移除 Profile 設定檔
			}
		}

		private void frmSignalViewer_Load(object sender, EventArgs e) {
			CreateChartEngine();

			Task.Factory.StartNew(() => {
				CreateSignalObject();
			});

			comboMode.SelectedIndex = 0;

			__bLoaded = true;
		}

		private void frmSignalViewer_Move(object sender, EventArgs e) {
			if (__bShown) {
				WindowStatus cWindow = __cProfile.Window;
				FloatWindow cFloatWindow = this.DockHandler.FloatPane.FloatWindow;

				cWindow.Left = cFloatWindow.Left;
				cWindow.Top = cFloatWindow.Top;
			}
		}

		private void frmSignalViewer_Resize(object sender, EventArgs e) {
			ReSize();
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
				this.Height = cWindow.Height;
				this.Width = cWindow.Width;
			}

			__bShown = true;
		}

		private void menuItemParameters_Click(object sender, EventArgs e) {
			ShowScriptParameters();
		}

		private void menuItemDataSource_Click(object sender, EventArgs e) {
			frmFormatObject frmFormatObject = new frmFormatObject(__cProfile, () => {
				__cChart.Dispose();
				__cSignalObject.Dispose();
			});
			DialogResult cResult = frmFormatObject.ShowDialog();
			frmFormatObject.Dispose();
			
			if (cResult == DialogResult.OK) {
				if (frmFormatObject.ModifyDataRequest) {
					CreateChartEngine();
					CreateSignalObject();
				}
			}
		}

		private void menuItemReLoad_Click(object sender, EventArgs e) {
			if (__cChart != null) {
				__cChart.Dispose();
			}

			if (__cSignalObject != null) {
				__cSignalObject.Dispose();
			}

			__bForceShowParameters = true;
			CreateChartEngine();
			CreateSignalObject();
		}

		private void menuItemRemove_Click(object sender, EventArgs e) {
			DialogResult cResult = MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (cResult == DialogResult.Yes) {
				this.Close();
			}
		}

		private void menuItemTradeDetails_Click(object sender, EventArgs e) {
			__bShowTradeView = !__bShowTradeView;
			menuItemTradeDetails.Checked = __bShowTradeView;
			
			ReSize();
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

		private void tabControl_DoubleClick(object sender, EventArgs e) {
			if (tabControl.Dock == DockStyle.Fill) {
				tabControl.Dock = DockStyle.None;
				tabControl.Top = this.ClientSize.Height - tabControl.Height - 1;
			} else {
				tabControl.Dock = DockStyle.Fill;
			}
		}

		private void comboMode_SelectedIndexChanged(object sender, EventArgs e) {
			int iIndex = comboMode.SelectedIndex;
			switch (iIndex) {
				case 0:
				case 1:
					labChar.Visible = false;
					datePickerStart.Visible = false;
					datePickerStop.Visible = false;
					btnQuery.Left = comboMode.Left + comboMode.Width + 3;
					break;
				case 2:
					labChar.Visible = true;
					datePickerStart.Visible = true;
					datePickerStop.Visible = true;
					btnQuery.Left = datePickerStop.Left + datePickerStop.Width + 3;
					break;
			}
		}

		private void btnQuery_Click(object sender, EventArgs e) {
			HistoryBoundList cHistoryList = __cTradeService.Closes;

			int iIndex = comboMode.SelectedIndex;
			switch (iIndex) {
				case 0:
					cHistoryList.Filter(txtSymbol.Text, DateTime.MinValue, DateTime.MaxValue);
					break;
				case 1:
					DateTime cStart = DateTime.Today;
					DateTime cStop = cStart.AddSeconds(86399);
					datePickerStart.Value = cStart;
					datePickerStop.Value = cStart;
					cHistoryList.Filter(txtSymbol.Text, cStart, cStop);
					break;
				case 2:
					cHistoryList.Filter(txtSymbol.Text, datePickerStart.Value, datePickerStop.Value.AddSeconds(86399));
					break;
			}
		}

		private void txtOutput_VisibleChanged(object sender, EventArgs e) {
			ScrollBottom();
		}

		private void OutputWriter_onOutputData(object sender, OutputDataEvent e) {
			if (!this.IsDisposed) {
				if (txtOutput.InvokeRequired) {
					txtOutput.BeginInvoke((MethodInvoker) delegate {
						WriteText(e.Data, e.IsNewLine);
					});
				} else {
					WriteText(e.Data, e.IsNewLine);
				}
			}
		}

		private void SignalObject_onReady(object sender, EventArgs e) {
			SignalObject cSignalObject = sender as SignalObject;
			cSignalObject.onReady -= SignalObject_onReady;
			cSignalObject.onUpdate += __cChart.onUpdate;
			cSignalObject.onTradeResponse += __cChart.onTradeResponse;
		}

		private void SignalObject_onScriptParameters(object sender, ScriptParametersEvent e) {
			__cOrderService = e.OrderService;
			__cParameters = e.ScriptParameters;

			int iCount = __cParameters.Count;
			List<string> sArgs = __cProfile.Parameters;
			if (sArgs == null && iCount > 0) {
				ShowScriptParameters();
			} else {
				if (__bForceShowParameters) {
					__bForceShowParameters = false;
					ShowScriptParameters();
				} else {
					try {
						for (int i = 0; i < iCount; i++) {
							__cParameters[i].SetValue(sArgs[i]);
						}

						__cSignalObject.UpdateParameters();
					} catch {
						ShowScriptParameters();
					}
				}
			}
		}
		
		private void SignalObject_onTradeResponse(object sender, ResponseEvent e) {
			__cTradeService.AddResponse(e);

			if (__bShowTradeView) {
				if (GlobalSettings.Testing.AutoSwitchTradePage) {
					ITradeOrder cOrder = e.TradeOrder;
					switch (e.ResponseType) {
						case Zeghs.Orders.ResponseType.Trust:  //委託回報
							tabControl.BeginInvoke((MethodInvoker) delegate {
								tabControl.SelectedIndex = 0;
							});
							break;
						case Zeghs.Orders.ResponseType.Deal:  //成交回報
							EOrderAction cAction = cOrder.Action;
							if (cAction == EOrderAction.Buy || cAction == EOrderAction.SellShort) {
								tabControl.BeginInvoke((MethodInvoker) delegate {
									tabControl.SelectedIndex = 1;
								});
							} else {
								tabControl.BeginInvoke((MethodInvoker) delegate {
									tabControl.SelectedIndex = 2;
								});
							}
							break;
					}
				}
			}
		}

		private void TradeService_onUpdate(object sender, EventArgs e) {
			RefreshGrid(this.dataGrid_Trust, __cTradeService.Trusts);
			RefreshGrid(this.dataGrid_Trade, __cTradeService.Opens);
			RefreshGrid(this.dataGrid_History, __cTradeService.Closes);
		}
	}
}