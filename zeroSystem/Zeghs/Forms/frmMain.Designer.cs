namespace Zeghs.Forms {
	partial class frmMain {
		private static readonly string __sMessageHeader_001 = "匯入成功";
		private static readonly string __sMessageHeader_002 = "匯出成功";
		private static readonly string __sMessageHeader_003 = "是否離開?";
		private static readonly string __sMessageContent_001 = "使用者設定檔匯入成功並已加入目前使用者設定。";
		private static readonly string __sMessageContent_002 = "使用者設定檔已轉出並匯出設定至檔案完成。";
		private static readonly string __sMessageContent_003 = "是否要停止交易並離開 ZERO System？";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.menubar = new System.Windows.Forms.MenuStrip();
			this.menuItem_File = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_ImportProfile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_ExportProfile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_line01 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItem_ExportReport = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_CsvReport = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_JsonReport = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem_About = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolItem_quoteManager = new System.Windows.Forms.ToolStripButton();
			this.toolItem_productManager = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolItem_cursor = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolItem_quote = new System.Windows.Forms.ToolStripButton();
			this.toolItem_script = new System.Windows.Forms.ToolStripButton();
			this.toolItem_logger = new System.Windows.Forms.ToolStripButton();
			this.toolItem_console = new System.Windows.Forms.ToolStripButton();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.dockPanels = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			this.openDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveDialog = new System.Windows.Forms.SaveFileDialog();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolItem_params = new System.Windows.Forms.ToolStripButton();
			this.menubar.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// menubar
			// 
			this.menubar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_File,
            this.menuItem_About});
			this.menubar.Location = new System.Drawing.Point(0, 0);
			this.menubar.Name = "menubar";
			this.menubar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menubar.Size = new System.Drawing.Size(792, 24);
			this.menubar.TabIndex = 1;
			this.menubar.Text = "menuStrip1";
			// 
			// menuItem_File
			// 
			this.menuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_ImportProfile,
            this.menuItem_ExportProfile,
            this.menuItem_line01,
            this.menuItem_ExportReport});
			this.menuItem_File.Name = "menuItem_File";
			this.menuItem_File.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
			this.menuItem_File.Size = new System.Drawing.Size(55, 20);
			this.menuItem_File.Text = "檔案(F)";
			// 
			// menuItem_ImportProfile
			// 
			this.menuItem_ImportProfile.Image = global::ZeroSystem.Properties.Resources.frmMain_menubar_menuItem_importProfile;
			this.menuItem_ImportProfile.Name = "menuItem_ImportProfile";
			this.menuItem_ImportProfile.Size = new System.Drawing.Size(166, 22);
			this.menuItem_ImportProfile.Text = "匯入使用者設定檔";
			this.menuItem_ImportProfile.Click += new System.EventHandler(this.menuItem_ImportProfile_Click);
			// 
			// menuItem_ExportProfile
			// 
			this.menuItem_ExportProfile.Image = global::ZeroSystem.Properties.Resources.frmMain_menubar_menuItem_exportProfile;
			this.menuItem_ExportProfile.Name = "menuItem_ExportProfile";
			this.menuItem_ExportProfile.Size = new System.Drawing.Size(166, 22);
			this.menuItem_ExportProfile.Text = "匯出使用者設定檔";
			this.menuItem_ExportProfile.Click += new System.EventHandler(this.menuItem_ExportProfile_Click);
			// 
			// menuItem_line01
			// 
			this.menuItem_line01.Name = "menuItem_line01";
			this.menuItem_line01.Size = new System.Drawing.Size(163, 6);
			// 
			// menuItem_ExportReport
			// 
			this.menuItem_ExportReport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_CsvReport,
            this.menuItem_JsonReport});
			this.menuItem_ExportReport.Image = global::ZeroSystem.Properties.Resources.frmMain_menubar_menuItem_exportReport;
			this.menuItem_ExportReport.Name = "menuItem_ExportReport";
			this.menuItem_ExportReport.Size = new System.Drawing.Size(166, 22);
			this.menuItem_ExportReport.Text = "匯出交易報表";
			// 
			// menuItem_CsvReport
			// 
			this.menuItem_CsvReport.Image = global::ZeroSystem.Properties.Resources.frmMain_menubar_menuItem_exportCSV;
			this.menuItem_CsvReport.Name = "menuItem_CsvReport";
			this.menuItem_CsvReport.Size = new System.Drawing.Size(120, 22);
			this.menuItem_CsvReport.Text = "CSV格式";
			this.menuItem_CsvReport.Click += new System.EventHandler(this.menuItem_CsvReport_Click);
			// 
			// menuItem_JsonReport
			// 
			this.menuItem_JsonReport.Image = global::ZeroSystem.Properties.Resources.frmMain_menubar_menuItem_exportJSON;
			this.menuItem_JsonReport.Name = "menuItem_JsonReport";
			this.menuItem_JsonReport.Size = new System.Drawing.Size(120, 22);
			this.menuItem_JsonReport.Text = "JSON格式";
			this.menuItem_JsonReport.Click += new System.EventHandler(this.menuItem_JsonReport_Click);
			// 
			// menuItem_About
			// 
			this.menuItem_About.Name = "menuItem_About";
			this.menuItem_About.Size = new System.Drawing.Size(41, 20);
			this.menuItem_About.Text = "關於";
			this.menuItem_About.Click += new System.EventHandler(this.menuItem_About_Click);
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolItem_quoteManager,
            this.toolItem_productManager,
            this.toolStripSeparator1,
            this.toolItem_cursor,
            this.toolStripSeparator2,
            this.toolItem_quote,
            this.toolItem_script,
            this.toolItem_logger,
            this.toolItem_console,
            this.toolStripSeparator3,
            this.toolItem_params});
			this.toolbar.Location = new System.Drawing.Point(0, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolbar.Size = new System.Drawing.Size(792, 25);
			this.toolbar.TabIndex = 2;
			this.toolbar.Text = "toolStrip1";
			// 
			// toolItem_quoteManager
			// 
			this.toolItem_quoteManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_quoteManager.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_quoteManager;
			this.toolItem_quoteManager.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_quoteManager.Name = "toolItem_quoteManager";
			this.toolItem_quoteManager.Size = new System.Drawing.Size(23, 22);
			this.toolItem_quoteManager.ToolTipText = "啟動報價服務管理員";
			this.toolItem_quoteManager.Click += new System.EventHandler(this.toolItem_quoteManager_Click);
			// 
			// toolItem_productManager
			// 
			this.toolItem_productManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_productManager.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_productManager;
			this.toolItem_productManager.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_productManager.Name = "toolItem_productManager";
			this.toolItem_productManager.Size = new System.Drawing.Size(23, 22);
			this.toolItem_productManager.ToolTipText = "啟動商品資訊管理員";
			this.toolItem_productManager.Click += new System.EventHandler(this.toolItem_productManager_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolItem_cursor
			// 
			this.toolItem_cursor.Checked = true;
			this.toolItem_cursor.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolItem_cursor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_cursor.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_cursor;
			this.toolItem_cursor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_cursor.Name = "toolItem_cursor";
			this.toolItem_cursor.Size = new System.Drawing.Size(23, 22);
			this.toolItem_cursor.Tag = "";
			this.toolItem_cursor.Text = "Cursor";
			this.toolItem_cursor.ToolTipText = "Cursor";
			this.toolItem_cursor.Click += new System.EventHandler(this.toolItem_CustomSelected_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolItem_quote
			// 
			this.toolItem_quote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_quote.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_quote;
			this.toolItem_quote.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_quote.Name = "toolItem_quote";
			this.toolItem_quote.Size = new System.Drawing.Size(23, 22);
			this.toolItem_quote.Tag = "Zeghs.Forms.frmQuoteViewer";
			this.toolItem_quote.ToolTipText = "報價檢視";
			this.toolItem_quote.Click += new System.EventHandler(this.toolItem_DockViewer_Click);
			// 
			// toolItem_script
			// 
			this.toolItem_script.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_script.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_script;
			this.toolItem_script.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_script.Name = "toolItem_script";
			this.toolItem_script.Size = new System.Drawing.Size(23, 22);
			this.toolItem_script.Tag = "Zeghs.Forms.frmScriptViewer";
			this.toolItem_script.ToolTipText = "腳本檢視";
			this.toolItem_script.Click += new System.EventHandler(this.toolItem_DockViewer_Click);
			// 
			// toolItem_logger
			// 
			this.toolItem_logger.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_logger.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_logger;
			this.toolItem_logger.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_logger.Name = "toolItem_logger";
			this.toolItem_logger.Size = new System.Drawing.Size(23, 22);
			this.toolItem_logger.Tag = "Zeghs.Forms.frmLogViewer";
			this.toolItem_logger.ToolTipText = "日誌檢視";
			this.toolItem_logger.Click += new System.EventHandler(this.toolItem_DockViewer_Click);
			// 
			// toolItem_console
			// 
			this.toolItem_console.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_console.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_console;
			this.toolItem_console.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_console.Name = "toolItem_console";
			this.toolItem_console.Size = new System.Drawing.Size(23, 22);
			this.toolItem_console.Tag = "Zeghs.Forms.frmConsoleViewer";
			this.toolItem_console.ToolTipText = "輸出視窗";
			this.toolItem_console.Click += new System.EventHandler(this.toolItem_DockViewer_Click);
			// 
			// statusbar
			// 
			this.statusbar.Location = new System.Drawing.Point(0, 544);
			this.statusbar.Name = "statusbar";
			this.statusbar.Size = new System.Drawing.Size(792, 22);
			this.statusbar.TabIndex = 3;
			this.statusbar.Text = "statusStrip1";
			// 
			// dockPanels
			// 
			this.dockPanels.AllowDrop = true;
			this.dockPanels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockPanels.DockBackColor = System.Drawing.SystemColors.AppWorkspace;
			this.dockPanels.Location = new System.Drawing.Point(0, 49);
			this.dockPanels.Name = "dockPanels";
			this.dockPanels.Size = new System.Drawing.Size(792, 495);
			this.dockPanels.TabIndex = 6;
			this.dockPanels.ContentRemoved += new System.EventHandler<WeifenLuo.WinFormsUI.Docking.DockContentEventArgs>(this.dockPanels_ContentRemoved);
			this.dockPanels.ActiveContentChanged += new System.EventHandler(this.dockPanels_ActiveDocumentChanged);
			this.dockPanels.DragDrop += new System.Windows.Forms.DragEventHandler(this.dockPanels_DragDrop);
			this.dockPanels.DragEnter += new System.Windows.Forms.DragEventHandler(this.dockPanels_DragEnter);
			// 
			// openDialog
			// 
			this.openDialog.RestoreDirectory = true;
			// 
			// saveDialog
			// 
			this.saveDialog.RestoreDirectory = true;
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolItem_params
			// 
			this.toolItem_params.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_params.Image = global::ZeroSystem.Properties.Resources.frmMain_toolbar_toolItem_params;
			this.toolItem_params.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_params.Name = "toolItem_params";
			this.toolItem_params.Size = new System.Drawing.Size(23, 22);
			this.toolItem_params.ToolTipText = "設定參數";
			this.toolItem_params.Click += new System.EventHandler(this.toolItem_params_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.dockPanels);
			this.Controls.Add(this.statusbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.menubar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menubar;
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.Move += new System.EventHandler(this.frmMain_Move);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.menubar.ResumeLayout(false);
			this.menubar.PerformLayout();
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menubar;
		private System.Windows.Forms.ToolStripMenuItem menuItem_About;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.StatusStrip statusbar;
		private System.Windows.Forms.ToolStripButton toolItem_quoteManager;
		private System.Windows.Forms.ToolStripButton toolItem_productManager;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanels;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolItem_quote;
		private System.Windows.Forms.ToolStripButton toolItem_script;
		private System.Windows.Forms.ToolStripButton toolItem_logger;
		private System.Windows.Forms.ToolStripButton toolItem_console;
		private System.Windows.Forms.ToolStripButton toolItem_cursor;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuItem_File;
		private System.Windows.Forms.ToolStripMenuItem menuItem_ImportProfile;
		private System.Windows.Forms.ToolStripMenuItem menuItem_ExportProfile;
		private System.Windows.Forms.OpenFileDialog openDialog;
		private System.Windows.Forms.SaveFileDialog saveDialog;
		private System.Windows.Forms.ToolStripSeparator menuItem_line01;
		private System.Windows.Forms.ToolStripMenuItem menuItem_ExportReport;
		private System.Windows.Forms.ToolStripMenuItem menuItem_CsvReport;
		private System.Windows.Forms.ToolStripMenuItem menuItem_JsonReport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton toolItem_params;
	}
}