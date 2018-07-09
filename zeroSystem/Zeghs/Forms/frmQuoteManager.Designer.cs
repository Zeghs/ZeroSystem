namespace Zeghs.Forms {
	partial class frmQuoteManager {
		private static readonly string __sMessageHeader_001 = "停用詢問";
		private static readonly string __sMessageContent_001 = "您是否確定要停用並中斷此資料來源？\n請注意：停用並中斷資料來源會導致策略模型無法正常運作！";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.SimpleBoundList<Zeghs.Data._QuoteServiceInfo> source = null;

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
			SourceGrid.Cells.Views.IView cTextAlignView = new SourceGrid.Cells.Views.Cell();
			cTextAlignView.TextAlignment = DevAge.Drawing.ContentAlignment.TopRight;

			this.dataGrid.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid.Columns.Add("Enabled", "啟用", typeof(bool));
			this.dataGrid.Columns.Add("Name", "報價元件", typeof(string));
			this.dataGrid.Columns.Add("FileVersion", "檔案版本", typeof(string));
			this.dataGrid.Columns.Add("ProductVersion", "元件版本", typeof(string));
			this.dataGrid.Columns.Add("Company", "開發廠商", typeof(string));
			this.dataGrid.Columns.Add("DataSource", "資料來源名稱", typeof(string));
			this.dataGrid.Columns.Add("PacketCountPerSeconds", "傳輸速率", typeof(string));
			this.dataGrid.Columns.Add("PacketCount", "封包個數", typeof(string));
			this.dataGrid.Columns[0].Width = 40;
			this.dataGrid.Columns[0].DataCell.Controller.RemoveController(SourceGrid.Cells.Controllers.CheckBox.Default);  //因為沒有編輯功能(取消對 Checkbox 的 Click 功能)
			this.dataGrid.Columns[1].Width = 150;
			this.dataGrid.Columns[2].Width = 70;
			this.dataGrid.Columns[3].Width = 70;
			this.dataGrid.Columns[4].Width = 200;
			this.dataGrid.Columns[5].Width = 150;
			this.dataGrid.Columns[6].Width = 90;
			this.dataGrid.Columns[6].DataCell.View = cTextAlignView;
			this.dataGrid.Columns[7].Width = 90;
			this.dataGrid.Columns[7].DataCell.View = cTextAlignView;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));
			this.dataGrid.Selection.SelectionChanged += dataGrid_onSelectionChanged;

			source = new Data.SimpleBoundList<Zeghs.Data._QuoteServiceInfo>(64);
			this.dataGrid.DataSource = source;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.btnEnabled = new System.Windows.Forms.Button();
			this.btnDisabled = new System.Windows.Forms.Button();
			this.btnSetting = new System.Windows.Forms.Button();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.groupGrid = new System.Windows.Forms.GroupBox();
			this.dataGrid = new SourceGrid.DataGrid();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabMemo = new System.Windows.Forms.TabPage();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnRefreshSymbol = new System.Windows.Forms.Button();
			this.btnReLogin = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.groupGrid.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabMemo.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnEnabled
			// 
			this.btnEnabled.Location = new System.Drawing.Point(544, 29);
			this.btnEnabled.Name = "btnEnabled";
			this.btnEnabled.Size = new System.Drawing.Size(94, 23);
			this.btnEnabled.TabIndex = 1;
			this.btnEnabled.Text = "啟用元件";
			this.btnEnabled.UseVisualStyleBackColor = true;
			this.btnEnabled.Click += new System.EventHandler(this.btnEnabled_Click);
			// 
			// btnDisabled
			// 
			this.btnDisabled.Location = new System.Drawing.Point(544, 29);
			this.btnDisabled.Name = "btnDisabled";
			this.btnDisabled.Size = new System.Drawing.Size(94, 23);
			this.btnDisabled.TabIndex = 2;
			this.btnDisabled.Text = "停用元件";
			this.btnDisabled.UseVisualStyleBackColor = true;
			this.btnDisabled.Visible = false;
			this.btnDisabled.Click += new System.EventHandler(this.btnDisabled_Click);
			// 
			// btnSetting
			// 
			this.btnSetting.Location = new System.Drawing.Point(544, 116);
			this.btnSetting.Name = "btnSetting";
			this.btnSetting.Size = new System.Drawing.Size(94, 23);
			this.btnSetting.TabIndex = 3;
			this.btnSetting.Text = "設定元件";
			this.btnSetting.UseVisualStyleBackColor = true;
			this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
			// 
			// btnRefresh
			// 
			this.btnRefresh.Location = new System.Drawing.Point(544, 145);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(94, 23);
			this.btnRefresh.TabIndex = 4;
			this.btnRefresh.Text = "重新整理";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// groupGrid
			// 
			this.groupGrid.Controls.Add(this.dataGrid);
			this.groupGrid.Location = new System.Drawing.Point(8, 11);
			this.groupGrid.Name = "groupGrid";
			this.groupGrid.Size = new System.Drawing.Size(530, 179);
			this.groupGrid.TabIndex = 5;
			this.groupGrid.TabStop = false;
			this.groupGrid.Text = "報價服務";
			// 
			// dataGrid
			// 
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(7, 18);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(516, 155);
			this.dataGrid.TabIndex = 1;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabMemo);
			this.tabControl.Location = new System.Drawing.Point(11, 195);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(524, 122);
			this.tabControl.TabIndex = 6;
			// 
			// tabMemo
			// 
			this.tabMemo.Controls.Add(this.txtDescription);
			this.tabMemo.Location = new System.Drawing.Point(4, 21);
			this.tabMemo.Name = "tabMemo";
			this.tabMemo.Padding = new System.Windows.Forms.Padding(3);
			this.tabMemo.Size = new System.Drawing.Size(516, 97);
			this.tabMemo.TabIndex = 0;
			this.tabMemo.Text = "描述";
			this.tabMemo.UseVisualStyleBackColor = true;
			// 
			// txtDescription
			// 
			this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDescription.Location = new System.Drawing.Point(3, 3);
			this.txtDescription.Multiline = true;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size(510, 91);
			this.txtDescription.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(544, 287);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(94, 23);
			this.btnClose.TabIndex = 7;
			this.btnClose.Text = "關閉";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnRefreshSymbol
			// 
			this.btnRefreshSymbol.Enabled = false;
			this.btnRefreshSymbol.Location = new System.Drawing.Point(544, 87);
			this.btnRefreshSymbol.Name = "btnRefreshSymbol";
			this.btnRefreshSymbol.Size = new System.Drawing.Size(94, 23);
			this.btnRefreshSymbol.TabIndex = 8;
			this.btnRefreshSymbol.Text = "更新代號表";
			this.btnRefreshSymbol.UseVisualStyleBackColor = true;
			this.btnRefreshSymbol.Click += new System.EventHandler(this.btnRefreshSymbol_Click);
			// 
			// btnReLogin
			// 
			this.btnReLogin.Location = new System.Drawing.Point(544, 58);
			this.btnReLogin.Name = "btnReLogin";
			this.btnReLogin.Size = new System.Drawing.Size(94, 23);
			this.btnReLogin.TabIndex = 9;
			this.btnReLogin.Text = "重新登入";
			this.btnReLogin.UseVisualStyleBackColor = true;
			this.btnReLogin.Click += new System.EventHandler(this.btnReLogin_Click);
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// frmQuoteManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(644, 329);
			this.Controls.Add(this.btnReLogin);
			this.Controls.Add(this.btnRefreshSymbol);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.groupGrid);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.btnSetting);
			this.Controls.Add(this.btnDisabled);
			this.Controls.Add(this.btnEnabled);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmQuoteManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "報價服務管理員";
			this.Load += new System.EventHandler(this.frmQuoteManager_Load);
			this.groupGrid.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabMemo.ResumeLayout(false);
			this.tabMemo.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnEnabled;
		private System.Windows.Forms.Button btnDisabled;
		private System.Windows.Forms.Button btnSetting;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.GroupBox groupGrid;
		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabMemo;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnRefreshSymbol;
		private System.Windows.Forms.Button btnReLogin;
		private System.Windows.Forms.Timer timer;
	}
}