namespace Zeghs.Forms {
	partial class frmSignalProperty {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Data.SimpleBoundList<Informations.OrderServiceInformation> source = null;

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
			this.dataGrid.Columns.Add("ModuleName", "報價元件", typeof(string));
			this.dataGrid.Columns.Add("FileVersion", "檔案版本", typeof(string));
			this.dataGrid.Columns.Add("ProductVersion", "元件版本", typeof(string));
			this.dataGrid.Columns.Add("Description", "說明", typeof(string));
			this.dataGrid.Columns.Add("Company", "開發廠商", typeof(string));
			this.dataGrid.Columns[0].Width = 150;
			this.dataGrid.Columns[1].Width = 70;
			this.dataGrid.Columns[2].Width = 70;
			this.dataGrid.Columns[3].Width = 200;
			this.dataGrid.Columns[4].Width = 200;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));
			this.dataGrid.Selection.SelectionChanged += dataGrid_onSelectionChanged;

			source = new Data.SimpleBoundList<Informations.OrderServiceInformation>(64);
			this.dataGrid.DataSource = source;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tabControl = new System.Windows.Forms.TabControl();
			this.pageItem_Properties = new System.Windows.Forms.TabPage();
			this.comboOrderService = new System.Windows.Forms.ComboBox();
			this.labOrderService = new System.Windows.Forms.Label();
			this.dataGrid = new SourceGrid.DataGrid();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtDefaultContracts = new System.Windows.Forms.TextBox();
			this.labDefaultContracts = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labTradeSize = new System.Windows.Forms.Label();
			this.txtMaxBarsReference = new System.Windows.Forms.TextBox();
			this.labMaxBarsReference = new System.Windows.Forms.Label();
			this.txtInitCapital = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labInitCapital = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labCapitalization = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.pageItem_Properties.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.pageItem_Properties);
			this.tabControl.Location = new System.Drawing.Point(7, 6);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(381, 419);
			this.tabControl.TabIndex = 0;
			// 
			// pageItem_Properties
			// 
			this.pageItem_Properties.Controls.Add(this.comboOrderService);
			this.pageItem_Properties.Controls.Add(this.labOrderService);
			this.pageItem_Properties.Controls.Add(this.dataGrid);
			this.pageItem_Properties.Controls.Add(this.label4);
			this.pageItem_Properties.Controls.Add(this.label5);
			this.pageItem_Properties.Controls.Add(this.txtDefaultContracts);
			this.pageItem_Properties.Controls.Add(this.labDefaultContracts);
			this.pageItem_Properties.Controls.Add(this.label3);
			this.pageItem_Properties.Controls.Add(this.labTradeSize);
			this.pageItem_Properties.Controls.Add(this.txtMaxBarsReference);
			this.pageItem_Properties.Controls.Add(this.labMaxBarsReference);
			this.pageItem_Properties.Controls.Add(this.txtInitCapital);
			this.pageItem_Properties.Controls.Add(this.label1);
			this.pageItem_Properties.Controls.Add(this.labInitCapital);
			this.pageItem_Properties.Controls.Add(this.label2);
			this.pageItem_Properties.Controls.Add(this.labCapitalization);
			this.pageItem_Properties.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Properties.Name = "pageItem_Properties";
			this.pageItem_Properties.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Properties.Size = new System.Drawing.Size(373, 394);
			this.pageItem_Properties.TabIndex = 0;
			this.pageItem_Properties.Text = "屬性";
			this.pageItem_Properties.UseVisualStyleBackColor = true;
			// 
			// comboOrderService
			// 
			this.comboOrderService.FormattingEnabled = true;
			this.comboOrderService.Location = new System.Drawing.Point(78, 359);
			this.comboOrderService.Name = "comboOrderService";
			this.comboOrderService.Size = new System.Drawing.Size(280, 20);
			this.comboOrderService.TabIndex = 17;
			// 
			// labOrderService
			// 
			this.labOrderService.AutoSize = true;
			this.labOrderService.Location = new System.Drawing.Point(15, 363);
			this.labOrderService.Name = "labOrderService";
			this.labOrderService.Size = new System.Drawing.Size(56, 12);
			this.labOrderService.TabIndex = 16;
			this.labOrderService.Text = "交易服務:";
			// 
			// dataGrid
			// 
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(15, 178);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(343, 167);
			this.dataGrid.TabIndex = 15;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label4.Location = new System.Drawing.Point(74, 161);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(284, 2);
			this.label4.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 156);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 12);
			this.label5.TabIndex = 13;
			this.label5.Text = "交易API:";
			// 
			// txtDefaultContracts
			// 
			this.txtDefaultContracts.Location = new System.Drawing.Point(137, 120);
			this.txtDefaultContracts.Name = "txtDefaultContracts";
			this.txtDefaultContracts.Size = new System.Drawing.Size(58, 22);
			this.txtDefaultContracts.TabIndex = 12;
			this.txtDefaultContracts.Text = "0";
			// 
			// labDefaultContracts
			// 
			this.labDefaultContracts.AutoSize = true;
			this.labDefaultContracts.Location = new System.Drawing.Point(23, 125);
			this.labDefaultContracts.Name = "labDefaultContracts";
			this.labDefaultContracts.Size = new System.Drawing.Size(104, 12);
			this.labDefaultContracts.TabIndex = 11;
			this.labDefaultContracts.Text = "預設交易合約數量:";
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(74, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(284, 2);
			this.label3.TabIndex = 10;
			// 
			// labTradeSize
			// 
			this.labTradeSize.AutoSize = true;
			this.labTradeSize.Location = new System.Drawing.Point(13, 98);
			this.labTradeSize.Name = "labTradeSize";
			this.labTradeSize.Size = new System.Drawing.Size(56, 12);
			this.labTradeSize.TabIndex = 9;
			this.labTradeSize.Text = "交易數量:";
			// 
			// txtMaxBarsReference
			// 
			this.txtMaxBarsReference.Location = new System.Drawing.Point(137, 66);
			this.txtMaxBarsReference.Name = "txtMaxBarsReference";
			this.txtMaxBarsReference.Size = new System.Drawing.Size(58, 22);
			this.txtMaxBarsReference.TabIndex = 8;
			this.txtMaxBarsReference.Text = "0";
			// 
			// labMaxBarsReference
			// 
			this.labMaxBarsReference.AutoSize = true;
			this.labMaxBarsReference.Location = new System.Drawing.Point(23, 71);
			this.labMaxBarsReference.Name = "labMaxBarsReference";
			this.labMaxBarsReference.Size = new System.Drawing.Size(107, 12);
			this.labMaxBarsReference.TabIndex = 7;
			this.labMaxBarsReference.Text = "最大 Bars 參考數量:";
			// 
			// txtInitCapital
			// 
			this.txtInitCapital.Location = new System.Drawing.Point(108, 38);
			this.txtInitCapital.Name = "txtInitCapital";
			this.txtInitCapital.Size = new System.Drawing.Size(87, 22);
			this.txtInitCapital.TabIndex = 6;
			this.txtInitCapital.Text = "0";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(91, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(11, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "$";
			// 
			// labInitCapital
			// 
			this.labInitCapital.AutoSize = true;
			this.labInitCapital.Location = new System.Drawing.Point(23, 43);
			this.labInitCapital.Name = "labInitCapital";
			this.labInitCapital.Size = new System.Drawing.Size(56, 12);
			this.labInitCapital.TabIndex = 4;
			this.labInitCapital.Text = "初始資本:";
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(74, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(284, 2);
			this.label2.TabIndex = 3;
			// 
			// labCapitalization
			// 
			this.labCapitalization.AutoSize = true;
			this.labCapitalization.Location = new System.Drawing.Point(13, 15);
			this.labCapitalization.Name = "labCapitalization";
			this.labCapitalization.Size = new System.Drawing.Size(59, 12);
			this.labCapitalization.TabIndex = 2;
			this.labCapitalization.Text = "成本/資本:";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(309, 431);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(228, 431);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmSignalProperty
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 468);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSignalProperty";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Signal Properties";
			this.Load += new System.EventHandler(this.frmSignalProperty_Load);
			this.tabControl.ResumeLayout(false);
			this.pageItem_Properties.ResumeLayout(false);
			this.pageItem_Properties.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageItem_Properties;
		private System.Windows.Forms.TextBox txtInitCapital;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labInitCapital;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labCapitalization;
		private System.Windows.Forms.TextBox txtMaxBarsReference;
		private System.Windows.Forms.Label labMaxBarsReference;
		private System.Windows.Forms.TextBox txtDefaultContracts;
		private System.Windows.Forms.Label labDefaultContracts;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labTradeSize;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.ComboBox comboOrderService;
		private System.Windows.Forms.Label labOrderService;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
	}
}