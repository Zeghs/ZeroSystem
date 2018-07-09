namespace Zeghs.Forms {
	partial class frmProductManager {
		private static readonly string __sMessageHeader_001 = "刪除商品";
		private static readonly string __sMessageHeader_002 = "刪除商品設定";
		private static readonly string __sMessageHeader_003 = "是否儲存";
		private static readonly string __sMessageHeader_004 = "儲存完畢";
		private static readonly string __sMessageContent_001 = "您是否要移除此金融商品資訊?";
		private static readonly string __sMessageContent_002 = "您是否要移除此商品的所有設定值?";
		private static readonly string __sMessageContent_003 = "隨意修改參數可能會造成策略或系統不穩定。\r\n確定要儲存商品設定值?";
		private static readonly string __sMessageContent_004 = "商品資訊已經儲存完畢，請重新啟動系統!";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.SimpleBoundList<Data._ProductInfo> source = null;
		private System.Collections.Generic.Dictionary<string, System.Comparison<Data._ProductInfo>> __cComparison = new System.Collections.Generic.Dictionary<string, System.Comparison<Data._ProductInfo>>(8) {
			{"ProductId_A", (a, b) => { return a.ProductId.CompareTo(b.ProductId); }},	
			{"ProductId_D", (a, b) => { return b.ProductId.CompareTo(a.ProductId); }},	
			{"CommodityId_A", (a, b) => { return a.CommodityId.CompareTo(b.CommodityId); }},	
			{"CommodityId_D", (a, b) => { return b.CommodityId.CompareTo(a.CommodityId); }},	
			{"ProductName_A", (a, b) => { return a.ProductName.CompareTo(b.ProductName); }},	
			{"ProductName_D", (a, b) => { return b.ProductName.CompareTo(a.ProductName); }}	
		};

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				__cAllData.Clear();
				__cBasicData.Clear();
				__cCustomData.Clear();
				__cComparison.Clear();
				__cDataSources.Clear();

				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeSourceGrid() {
			this.dataGrid.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid.Columns.Add("ProductId", "商品代號", typeof(string));
			this.dataGrid.Columns.Add("CommodityId", "商品來源", typeof(string));
			this.dataGrid.Columns.Add("ProductName", "商品名稱", typeof(string));
			this.dataGrid.Columns.Add("DataSource", "資料來源", typeof(string));
			this.dataGrid.Columns.Add("Description", "商品描述", typeof(string));
			this.dataGrid.Columns[0].Width = 100;
			this.dataGrid.Columns[1].Width = 100;
			this.dataGrid.Columns[2].Width = 70;
			this.dataGrid.Columns[3].Width = 90;
			this.dataGrid.Columns[4].Width = 190;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			source = new Data.SimpleBoundList<Data._ProductInfo>(__cAllData);
			source.AllowSort = true;
			source.SetComparers(__cComparison);

			this.dataGrid.DataSource = source;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("所有交易所");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProductManager));
			this.treeExchanges = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.dataGrid = new SourceGrid.DataGrid();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolItem_Create = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Modify = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Delete = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolItem_All = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Basic = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Custom = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolItem_Save = new System.Windows.Forms.ToolStripButton();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeExchanges
			// 
			this.treeExchanges.ImageIndex = 0;
			this.treeExchanges.ImageList = this.imageList;
			this.treeExchanges.Location = new System.Drawing.Point(2, 28);
			this.treeExchanges.Name = "treeExchanges";
			treeNode1.Name = "treeItem_Exchanges";
			treeNode1.Text = "所有交易所";
			this.treeExchanges.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.treeExchanges.SelectedImageIndex = 0;
			this.treeExchanges.Size = new System.Drawing.Size(183, 293);
			this.treeExchanges.TabIndex = 0;
			this.treeExchanges.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeExchanges_NodeMouseClick);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "finance.png");
			this.imageList.Images.SetKeyName(1, "exchange.png");
			this.imageList.Images.SetKeyName(2, "stock.png");
			// 
			// dataGrid
			// 
			this.dataGrid.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(188, 28);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(571, 293);
			this.dataGrid.TabIndex = 3;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			this.dataGrid.SortedRangeRows += new SourceGrid.SortRangeRowsEventHandler(this.dataGrid_SortedRangeRows);
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolItem_Create,
            this.toolItem_Modify,
            this.toolItem_Delete,
            this.toolItem_Separator1,
            this.toolItem_All,
            this.toolItem_Basic,
            this.toolItem_Custom,
            this.toolStripSeparator1,
            this.toolItem_Save});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolbar.Size = new System.Drawing.Size(764, 25);
			this.toolbar.TabIndex = 4;
			this.toolbar.Text = "toolStrip1";
			// 
			// toolItem_Create
			// 
			this.toolItem_Create.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Create.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Create;
			this.toolItem_Create.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Create.Name = "toolItem_Create";
			this.toolItem_Create.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Create.Tag = "1";
			this.toolItem_Create.ToolTipText = "新增金融商品";
			this.toolItem_Create.Click += new System.EventHandler(this.toolItem_Modify_Click);
			// 
			// toolItem_Modify
			// 
			this.toolItem_Modify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Modify.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Modify;
			this.toolItem_Modify.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Modify.Name = "toolItem_Modify";
			this.toolItem_Modify.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Modify.Tag = "2";
			this.toolItem_Modify.ToolTipText = "修改金融商品設定";
			this.toolItem_Modify.Click += new System.EventHandler(this.toolItem_Modify_Click);
			// 
			// toolItem_Delete
			// 
			this.toolItem_Delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Delete.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Delete;
			this.toolItem_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Delete.Name = "toolItem_Delete";
			this.toolItem_Delete.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Delete.Tag = "3";
			this.toolItem_Delete.ToolTipText = "刪除金融商品";
			this.toolItem_Delete.Click += new System.EventHandler(this.toolItem_Modify_Click);
			// 
			// toolItem_Separator1
			// 
			this.toolItem_Separator1.Name = "toolItem_Separator1";
			this.toolItem_Separator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolItem_All
			// 
			this.toolItem_All.Checked = true;
			this.toolItem_All.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolItem_All.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_All.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_All;
			this.toolItem_All.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_All.Name = "toolItem_All";
			this.toolItem_All.Size = new System.Drawing.Size(23, 22);
			this.toolItem_All.Tag = "0";
			this.toolItem_All.ToolTipText = "顯示所有商品資訊";
			this.toolItem_All.Click += new System.EventHandler(this.toolItem_Filter_Click);
			// 
			// toolItem_Basic
			// 
			this.toolItem_Basic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Basic.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Basic;
			this.toolItem_Basic.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Basic.Name = "toolItem_Basic";
			this.toolItem_Basic.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Basic.Tag = "1";
			this.toolItem_Basic.ToolTipText = "僅顯示交易所定義的商品資訊";
			this.toolItem_Basic.Click += new System.EventHandler(this.toolItem_Filter_Click);
			// 
			// toolItem_Custom
			// 
			this.toolItem_Custom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Custom.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Custom;
			this.toolItem_Custom.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Custom.Name = "toolItem_Custom";
			this.toolItem_Custom.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Custom.Tag = "2";
			this.toolItem_Custom.ToolTipText = "僅顯示使用者定義的商品資訊";
			this.toolItem_Custom.Click += new System.EventHandler(this.toolItem_Filter_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolItem_Save
			// 
			this.toolItem_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Save.Image = global::ZeroSystem.Properties.Resources.frmProductManager_toolbar_toolItem_Save;
			this.toolItem_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Save.Name = "toolItem_Save";
			this.toolItem_Save.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Save.ToolTipText = "儲存修改設定值";
			this.toolItem_Save.Click += new System.EventHandler(this.toolItem_Save_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(677, 331);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(595, 331);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "確定";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// frmProductManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(764, 365);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.toolbar);
			this.Controls.Add(this.dataGrid);
			this.Controls.Add(this.treeExchanges);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmProductManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Tag = "0";
			this.Text = "商品資訊管理員";
			this.Load += new System.EventHandler(this.frmProductManager_Load);
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeExchanges;
		private System.Windows.Forms.ImageList imageList;
		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.ToolStripSeparator toolItem_Separator1;
		private System.Windows.Forms.ToolStripButton toolItem_All;
		private System.Windows.Forms.ToolStripButton toolItem_Basic;
		private System.Windows.Forms.ToolStripButton toolItem_Custom;
		private System.Windows.Forms.ToolStripButton toolItem_Create;
		private System.Windows.Forms.ToolStripButton toolItem_Modify;
		private System.Windows.Forms.ToolStripButton toolItem_Delete;
		private System.Windows.Forms.ToolStripButton toolItem_Save;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
	}
}