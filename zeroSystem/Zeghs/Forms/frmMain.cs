using System;
using System.Windows.Forms;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using Zeghs.Actions;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmMain : Form {
		private bool __bShown = false;
		private bool __bLoaded = false;
		private Form __cActivateForm = null;
		private PowerLanguage.PenStyle __cPenStyle = null;
		private Dictionary<string, IDockContent> __cCommons = null;

		internal frmMain() {
			__cCommons = new Dictionary<string, IDockContent>(8);
			__cPenStyle = new PowerLanguage.PenStyle(System.Drawing.Color.Red, 1);

			InitializeComponent();
		}

		internal void SetCustomDrawTools(Dictionary<string, IAction> customs) {
			int iIndex = 4;  //插入索引為第4個(如果有新增 toolbar 項目還需要再修改)
			foreach (IAction cAction in customs.Values) {
				ToolStripButton cButton = new ToolStripButton();
				cButton.Click += toolItem_CustomDrawing_Click;
				cButton.Image = cAction.Icons[0];
				cButton.Text = cAction.Name;
				cButton.ToolTipText = cAction.Comment;
				cButton.DisplayStyle = ToolStripItemDisplayStyle.Image;

				toolbar.Items.Insert(iIndex++, cButton);
			}
			
			ToolStripItem cItem = toolbar.Items[4];
			cItem.Click += toolItem_CustomSelected_Click;
			toolItem_cursor.Tag = cItem;
		}

		private IDockContent GetDockContentFromPersistString(string persistString) {
			IDockContent cContent = null;
			bool bExist = __cCommons.TryGetValue(persistString, out cContent);

			switch (persistString) {
				case "Zeghs.Forms.frmConsoleViewer":
				case "Zeghs.Forms.frmLogViewer":
				case "Zeghs.Forms.frmQuoteViewer":
				case "Zeghs.Forms.frmScriptViewer":
					if (bExist) {
						cContent.DockHandler.Activate();
					} else {
						DockContent cDockContent = Activator.CreateInstance(Type.GetType(persistString), true) as DockContent;
						__cCommons.Add(persistString, cDockContent);

						if (__bLoaded) {
							cDockContent.Show(this.dockPanels, DockState.Float);
						} else {
							cContent = cDockContent;
						}
					}
					break;
			}
			return cContent;
		}

		private void OnShowQuoteManager() {
			frmQuoteManager frmQuoteManager = new frmQuoteManager();
			frmQuoteManager.ShowDialog();
			frmQuoteManager.Dispose();
		}

		private void SetCustomAction(string action) {
			frmSignalViewer frmSignalViewer = __cActivateForm as frmSignalViewer;
			frmSignalViewer.Chart.SetCustomAction(action, __cPenStyle);
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
			ProfileManager.Manager.onLoadProfile -= ProfileManager_onLoadProfile;
			ProfileManager.Manager.Save("default");

			this.dockPanels.SaveAsXml(string.Format("{0}__main.xml", GlobalSettings.Paths.ProfilePath));
			__cCommons.Clear();
		}

		private void frmMain_Load(object sender, EventArgs e) {
			this.dockPanels.LoadFromXml(string.Format("{0}__main.xml", GlobalSettings.Paths.ProfilePath), new DeserializeDockContent(GetDockContentFromPersistString));
			
			this.OnResize(null);

			__bLoaded = true;
		}

		private void frmMain_Move(object sender, EventArgs e) {
			if (__bShown) {
				WindowStatus cWindow = ProfileManager.Manager.MainWindow;
				cWindow.Left = this.Left;
				cWindow.Top = this.Top;
			}
		}

		private void frmMain_Resize(object sender, EventArgs e) {
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
						this.Left = cWindow.Left;
						this.Top = cWindow.Top;
						this.Height = cWindow.Height;
						this.Width = cWindow.Width;
						break;
				}
			}
			__bShown = true;

			OnShowQuoteManager();

			this.OnResize(EventArgs.Empty);

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

		private void toolItem_CustomDrawing_Click(object sender, EventArgs e) {
			ToolStripButton cButton = sender as ToolStripButton;
			SetCustomAction(cButton.Text);
		}

		private void toolItem_CustomSelected_Click(object sender, EventArgs e) {
			ToolStripButton cButton = sender as ToolStripButton;
			cButton.Checked = true;

			if (cButton.Text.Equals("Cursor")) {
				ToolStripButton cCrossButton = cButton.Tag as ToolStripButton;
				if (cCrossButton != null) {
					cCrossButton.Checked = false;
				}
			} else {
				toolItem_cursor.Checked = false;
			}
			SetCustomAction(cButton.Text);
		}

		private void toolItem_DockViewer_Click(object sender, EventArgs e) {
			ToolStripButton cButton = sender as ToolStripButton;
			GetDockContentFromPersistString(cButton.Tag as string);
		}

		private void dockPanels_ContentRemoved(object sender, DockContentEventArgs e) {
			string sFullName = e.Content.DockHandler.Form.GetType().FullName;
			if (__cCommons.ContainsKey(sFullName)) {
				__cCommons.Remove(sFullName);
			}
		}

		private void dockPanels_DragDrop(object sender, DragEventArgs e) {
			ScriptInformation cInfo = e.Data.GetData("__script") as ScriptInformation;
			frmFormatObject.Create(this.dockPanels, cInfo);
		}

		private void dockPanels_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent("__script")) {
				e.Effect = DragDropEffects.Move;
			}
		}

		private void dockPanels_ActiveDocumentChanged(object sender, EventArgs e) {
			if (this.dockPanels.ActiveDocument != null) {
				__cActivateForm = this.dockPanels.ActiveDocument.DockHandler.Form;
			}
		}

		private void ProfileManager_onLoadProfile(object sender, EventArgs e) {
			ProfileManager.Manager.onLoadProfile -= ProfileManager_onLoadProfile;

			ProfileSetting[] cProfiles = ProfileManager.Manager.Profiles;
			if (cProfiles != null) {
				foreach (ProfileSetting cProfile in cProfiles) {
					switch (cProfile.ScriptType) {
						case ScriptType.Script:
							break;
						case ScriptType.Signal:
							frmSignalViewer.Create(this.dockPanels, cProfile);
							break;
					}
				}
			}
		}
	}
}