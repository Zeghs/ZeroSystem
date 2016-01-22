namespace Zeghs.Forms {
	partial class frmScriptViewer {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("交易信號", 1, 1);
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("自訂腳本", 2, 2);
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("模組系統", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScriptViewer));
			this.treeView = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageList;
			this.treeView.ItemHeight = 20;
			this.treeView.Location = new System.Drawing.Point(0, 23);
			this.treeView.Name = "treeView";
			treeNode1.ImageIndex = 1;
			treeNode1.Name = "treeItem_Signal";
			treeNode1.SelectedImageIndex = 1;
			treeNode1.Text = "交易信號";
			treeNode2.ImageIndex = 2;
			treeNode2.Name = "treeItem_Script";
			treeNode2.SelectedImageIndex = 2;
			treeNode2.Text = "自訂腳本";
			treeNode3.Name = "treeItem_Module";
			treeNode3.SelectedImageIndex = 0;
			treeNode3.Text = "模組系統";
			this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
			this.treeView.SelectedImageIndex = 0;
			this.treeView.Size = new System.Drawing.Size(228, 152);
			this.treeView.TabIndex = 0;
			this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
			this.treeView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseMove);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "module.png");
			this.imageList.Images.SetKeyName(1, "signal.png");
			this.imageList.Images.SetKeyName(2, "script.png");
			// 
			// frmScriptViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(229, 176);
			this.ControlBox = false;
			this.Controls.Add(this.treeView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmScriptViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "交易模組";
			this.Load += new System.EventHandler(this.frmScriptViewer_Load);
			this.Resize += new System.EventHandler(this.frmScriptViewer_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ImageList imageList;
	}
}