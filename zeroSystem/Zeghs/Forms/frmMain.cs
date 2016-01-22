using System;
using System.Windows.Forms;
using Zeghs.Settings;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmMain : Form {
		private bool __bShown = false;
		private frmQuoteViewer __frmQuoteViewer = null;
		private frmScriptViewer __frmScriptViewer = null;
		private frmConsoleViewer __frmConsoleViewer = null;
		
		internal frmMain() {
			InitializeComponent();

			__frmQuoteViewer = new frmQuoteViewer();
			__frmScriptViewer = new frmScriptViewer();
			__frmConsoleViewer = new frmConsoleViewer();
		}

		private void CreateScriptViewer(ProfileSetting profile) {
			frmSignalViewer frmSignalViewer = new frmSignalViewer();
			frmSignalViewer.SetProfileSetting(profile);
			frmSignalViewer.TopLevel = false;

			panelForms.Controls.Add(frmSignalViewer);
			frmSignalViewer.Show();
		}

		private void OnShowQuoteManager() {
			frmQuoteManager frmQuoteManager = new frmQuoteManager();
			frmQuoteManager.ShowDialog();
			frmQuoteManager.Dispose();
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
			ProfileManager.Manager.onLoadProfile -= ProfileManager_onLoadProfile;
			ProfileManager.Manager.Save("default");

			__frmQuoteViewer.Dispose();
			__frmScriptViewer.Dispose();
			__frmConsoleViewer.Dispose();
		}

		private void frmMain_Load(object sender, EventArgs e) {
			__frmQuoteViewer.MdiParent = this;
			__frmScriptViewer.MdiParent = this;
			__frmConsoleViewer.MdiParent = this;

			__frmQuoteViewer.Show();
			__frmScriptViewer.Show();
			__frmConsoleViewer.Show();

			this.OnResize(null);
		}

		private void frmMain_Move(object sender, EventArgs e) {
			if (__bShown) {
				WindowStatus cWindow = ProfileManager.Manager.MainWindow;
				cWindow.Left = this.Left;
				cWindow.Top = this.Top;
			}
		}

		private void frmMain_Resize(object sender, EventArgs e) {
			int iUseHeight = toolbar.Top + toolbar.Height + statusbar.Height;
			int iViewHeight = (this.ClientSize.Height - iUseHeight - 154) / 2;

			__frmQuoteViewer.Left = 0;
			__frmQuoteViewer.Top = 0;
			__frmQuoteViewer.Height = iViewHeight;

			__frmScriptViewer.Left = 0;
			__frmScriptViewer.Top = __frmQuoteViewer.Height;
			__frmScriptViewer.Height = iViewHeight;

			__frmConsoleViewer.Left = 0;
			__frmConsoleViewer.Top = __frmScriptViewer.Top + __frmScriptViewer.Height;
			__frmConsoleViewer.Width = this.ClientSize.Width - 4;

			panelForms.Left = __frmQuoteViewer.Width + 2;
			panelForms.Top = toolbar.Top + toolbar.Height + 2;
			panelForms.Width = this.ClientSize.Width - panelForms.Left - 4;
			panelForms.Height = this.ClientSize.Height - __frmConsoleViewer.Height - iUseHeight - 7;

			if (__bShown) {
				WindowStatus cWindow = ProfileManager.Manager.MainWindow;
				cWindow.Height = this.Height;
				cWindow.Width = this.Width;
				cWindow.WindowState = this.WindowState;
			}
		}

		private void frmMain_Shown(object sender, EventArgs e) {
			WindowStatus cWindow = ProfileManager.Manager.MainWindow;
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

			OnShowQuoteManager();

			this.OnResize(null);

			ProfileManager.Manager.onLoadProfile += ProfileManager_onLoadProfile;
			ProfileManager.Manager.Load("default");
		}

		private void menuItem_About_Click(object sender, EventArgs e) {
			frmAbout frmAbout = new frmAbout();
			frmAbout.ShowDialog();
			frmAbout.Dispose();
		}

		private void toolItem_productManager_Click(object sender, EventArgs e) {
			frmProductManager frmProductManager = new frmProductManager();
			frmProductManager.ShowDialog();
			frmProductManager.Dispose();
		}

		private void toolItem_quoteManager_Click(object sender, EventArgs e) {
			OnShowQuoteManager();
		}

		private void panelForms_DragDrop(object sender, DragEventArgs e) {
			ScriptInformation cInfo = e.Data.GetData("__script") as ScriptInformation;
			frmFormatObject frmFormatObject = new frmFormatObject();
			frmFormatObject.SetScriptInformation(cInfo);

			DialogResult cResult = frmFormatObject.ShowDialog();
			frmFormatObject.Dispose();

			if (cResult == DialogResult.OK) {
				CreateScriptViewer(frmFormatObject.Profile);
			}
		}

		private void panelForms_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent("__script")) {
				e.Effect = DragDropEffects.Move;
			}
		}

		private void ProfileManager_onLoadProfile(object sender, EventArgs e) {
			ProfileSetting[] cProfiles = ProfileManager.Manager.Profiles;
			if (cProfiles != null) {
				foreach (ProfileSetting cProfile in cProfiles) {
					CreateScriptViewer(cProfile);
				}
			}
		}
	}
}