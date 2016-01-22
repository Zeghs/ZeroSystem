namespace Zeghs.Forms {
	partial class frmMain {
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
			this.menuItem_About = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolItem_quoteManager = new System.Windows.Forms.ToolStripButton();
			this.toolItem_productManager = new System.Windows.Forms.ToolStripButton();
			this.statusbar = new System.Windows.Forms.StatusStrip();
			this.panelForms = new System.Windows.Forms.Panel();
			this.menubar.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// menubar
			// 
			this.menubar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_About});
			this.menubar.Location = new System.Drawing.Point(0, 0);
			this.menubar.Name = "menubar";
			this.menubar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menubar.Size = new System.Drawing.Size(792, 24);
			this.menubar.TabIndex = 1;
			this.menubar.Text = "menuStrip1";
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
            this.toolItem_productManager});
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
			// statusbar
			// 
			this.statusbar.Location = new System.Drawing.Point(0, 544);
			this.statusbar.Name = "statusbar";
			this.statusbar.Size = new System.Drawing.Size(792, 22);
			this.statusbar.TabIndex = 3;
			this.statusbar.Text = "statusStrip1";
			// 
			// panelForms
			// 
			this.panelForms.AllowDrop = true;
			this.panelForms.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.panelForms.Location = new System.Drawing.Point(0, 52);
			this.panelForms.Name = "panelForms";
			this.panelForms.Size = new System.Drawing.Size(100, 100);
			this.panelForms.TabIndex = 5;
			this.panelForms.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelForms_DragDrop);
			this.panelForms.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelForms_DragEnter);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.panelForms);
			this.Controls.Add(this.statusbar);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.menubar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menubar;
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "zeroSystem - Zoning and Emotional Range Omitted System";
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
		private System.Windows.Forms.Panel panelForms;
	}
}