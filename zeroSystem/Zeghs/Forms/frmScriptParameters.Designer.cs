namespace Zeghs.Forms {
	internal class BoundImage : SourceGrid.Cells.Models.IImage {
		internal BoundImage() {
		}

		public System.Drawing.Image GetImage(SourceGrid.CellContext cellContext) {
			return ZeroSystem.Properties.Resources.frmScriptParameters_dataGrid_cell_keyboard;
		}
	}

	partial class frmScriptParameters {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private SourceGrid.Cells.Editors.ComboBox __cComboBox = null;
		private SourceGrid.Cells.Editors.EditorBase __cTextBox = null;
		private Zeghs.Data.SimpleBoundList<Zeghs.Data._ParameterInfo> source = null;

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

		private void InitializeSourceGrid() {
			this.dataGrid.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid.Columns.Add("Comment", "參數", typeof(string));
			this.dataGrid.Columns.Add("Value", "數值", typeof(string));
			this.dataGrid.Columns[0].Width = 180;
			this.dataGrid.Columns[0].DataCell.Model.AddModel(new BoundImage());
			this.dataGrid.Columns[0].DataCell.Editor.EnableEdit = false;
			this.dataGrid.Columns[1].Width = 175;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			//建立必要的 Editor 控制項
			__cTextBox = this.dataGrid.Columns[1].DataCell.Editor;  //保存原始 Editor 控制像
			__cTextBox.AllowNull = false;
			__cComboBox = new SourceGrid.Cells.Editors.ComboBox(typeof(string), new string[] { "True", "False" }, true);
			__cComboBox.AllowNull = false;

			//建立資料來源
			source = new Data.SimpleBoundList<Data._ParameterInfo>(64);
			source.AllowEdit = true;
			this.dataGrid.DataSource = source;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.pageItem_Parameters = new System.Windows.Forms.TabPage();
			this.dataGrid = new SourceGrid.DataGrid();
			this.pageItem_Commissions = new System.Windows.Forms.TabPage();
			this.ruleGrid = new SourceGrid.DataGrid();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolItem_Create = new System.Windows.Forms.ToolStripButton();
			this.toolItem_Remove = new System.Windows.Forms.ToolStripButton();
			this.tabControl.SuspendLayout();
			this.pageItem_Parameters.SuspendLayout();
			this.pageItem_Commissions.SuspendLayout();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(118, 238);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(199, 238);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.pageItem_Parameters);
			this.tabControl.Controls.Add(this.pageItem_Commissions);
			this.tabControl.Location = new System.Drawing.Point(3, 4);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(390, 230);
			this.tabControl.TabIndex = 7;
			// 
			// pageItem_Parameters
			// 
			this.pageItem_Parameters.Controls.Add(this.dataGrid);
			this.pageItem_Parameters.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Parameters.Name = "pageItem_Parameters";
			this.pageItem_Parameters.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Parameters.Size = new System.Drawing.Size(382, 205);
			this.pageItem_Parameters.TabIndex = 0;
			this.pageItem_Parameters.Text = "輸入參數";
			this.pageItem_Parameters.UseVisualStyleBackColor = true;
			// 
			// dataGrid
			// 
			this.dataGrid.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(3, 3);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(376, 199);
			this.dataGrid.TabIndex = 5;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			this.dataGrid.DoubleClick += new System.EventHandler(this.dataGrid_DoubleClick);
			// 
			// pageItem_Commissions
			// 
			this.pageItem_Commissions.Controls.Add(this.ruleGrid);
			this.pageItem_Commissions.Controls.Add(this.toolbar);
			this.pageItem_Commissions.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Commissions.Name = "pageItem_Commissions";
			this.pageItem_Commissions.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Commissions.Size = new System.Drawing.Size(382, 205);
			this.pageItem_Commissions.TabIndex = 1;
			this.pageItem_Commissions.Text = "交易成本";
			this.pageItem_Commissions.UseVisualStyleBackColor = true;
			// 
			// ruleGrid
			// 
			this.ruleGrid.BackColor = System.Drawing.SystemColors.Window;
			this.ruleGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ruleGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.ruleGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ruleGrid.EnableSort = false;
			this.ruleGrid.FixedRows = 1;
			this.ruleGrid.Location = new System.Drawing.Point(3, 28);
			this.ruleGrid.Name = "ruleGrid";
			this.ruleGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.ruleGrid.Size = new System.Drawing.Size(376, 174);
			this.ruleGrid.TabIndex = 6;
			this.ruleGrid.TabStop = true;
			this.ruleGrid.ToolTipText = "";
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolItem_Create,
            this.toolItem_Remove});
			this.toolbar.Location = new System.Drawing.Point(3, 3);
			this.toolbar.Name = "toolbar";
			this.toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolbar.Size = new System.Drawing.Size(376, 25);
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
			// 
			// toolItem_Create
			// 
			this.toolItem_Create.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Create.Image = global::ZeroSystem.Properties.Resources.frmScriptParameters_toolbar_toolItem_Create;
			this.toolItem_Create.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Create.Name = "toolItem_Create";
			this.toolItem_Create.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Create.ToolTipText = "新增規則";
			this.toolItem_Create.Click += new System.EventHandler(this.toolItem_Create_Click);
			// 
			// toolItem_Remove
			// 
			this.toolItem_Remove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolItem_Remove.Image = global::ZeroSystem.Properties.Resources.frmScriptParameters_toolbar_toolItem_Delete;
			this.toolItem_Remove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolItem_Remove.Name = "toolItem_Remove";
			this.toolItem_Remove.Size = new System.Drawing.Size(23, 22);
			this.toolItem_Remove.ToolTipText = "刪除規則";
			// 
			// frmScriptParameters
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 268);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmScriptParameters";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "參數輸入";
			this.TopMost = true;
			this.tabControl.ResumeLayout(false);
			this.pageItem_Parameters.ResumeLayout(false);
			this.pageItem_Commissions.ResumeLayout(false);
			this.pageItem_Commissions.PerformLayout();
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageItem_Parameters;
		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.TabPage pageItem_Commissions;
		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.ToolStripButton toolItem_Create;
		private System.Windows.Forms.ToolStripButton toolItem_Remove;
		private SourceGrid.DataGrid ruleGrid;
	}
}