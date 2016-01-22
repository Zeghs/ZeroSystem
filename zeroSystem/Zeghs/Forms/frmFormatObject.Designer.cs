namespace Zeghs.Forms {
	partial class frmFormatObject {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.SimpleBoundList<Zeghs.Data._DataStreamInfo> source = null;

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
			this.dataGrid.Columns.Add("DataStream", "Data #", typeof(string));
			this.dataGrid.Columns.Add("SymbolId", "商品代號", typeof(string));
			this.dataGrid.Columns.Add("Range", "區間", typeof(string));
			this.dataGrid.Columns.Add("LastDate", "終止日", typeof(string));
			this.dataGrid.Columns.Add("Period", "週期", typeof(string));
			this.dataGrid.Columns.Add("SubChart", "副圖", typeof(string));
			this.dataGrid.Columns[0].Width = 60;
			this.dataGrid.Columns[1].Width = 100;
			this.dataGrid.Columns[2].Width = 90;
			this.dataGrid.Columns[3].Width = 90;
			this.dataGrid.Columns[4].Width = 90;
			this.dataGrid.Columns[5].Width = 80;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			//建立資料來源
			source = new Data.SimpleBoundList<Data._DataStreamInfo>(32);
			this.dataGrid.DataSource = source;
		}
		
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tabControl = new System.Windows.Forms.TabControl();
			this.pageItem_Instrument = new System.Windows.Forms.TabPage();
			this.btnProperty = new System.Windows.Forms.Button();
			this.btnDataStreamRemove = new System.Windows.Forms.Button();
			this.btnDataStreamAdd = new System.Windows.Forms.Button();
			this.dataGrid = new SourceGrid.DataGrid();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.pageItem_Instrument.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.pageItem_Instrument);
			this.tabControl.Location = new System.Drawing.Point(7, 11);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(657, 246);
			this.tabControl.TabIndex = 0;
			// 
			// pageItem_Instrument
			// 
			this.pageItem_Instrument.Controls.Add(this.btnProperty);
			this.pageItem_Instrument.Controls.Add(this.btnDataStreamRemove);
			this.pageItem_Instrument.Controls.Add(this.btnDataStreamAdd);
			this.pageItem_Instrument.Controls.Add(this.dataGrid);
			this.pageItem_Instrument.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Instrument.Name = "pageItem_Instrument";
			this.pageItem_Instrument.Size = new System.Drawing.Size(649, 221);
			this.pageItem_Instrument.TabIndex = 0;
			this.pageItem_Instrument.Text = "Instrument";
			this.pageItem_Instrument.UseVisualStyleBackColor = true;
			// 
			// btnProperty
			// 
			this.btnProperty.Location = new System.Drawing.Point(550, 69);
			this.btnProperty.Name = "btnProperty";
			this.btnProperty.Size = new System.Drawing.Size(95, 23);
			this.btnProperty.TabIndex = 7;
			this.btnProperty.Text = "屬性...";
			this.btnProperty.UseVisualStyleBackColor = true;
			this.btnProperty.Click += new System.EventHandler(this.btnProperty_Click);
			// 
			// btnDataStreamRemove
			// 
			this.btnDataStreamRemove.Enabled = false;
			this.btnDataStreamRemove.Location = new System.Drawing.Point(550, 40);
			this.btnDataStreamRemove.Name = "btnDataStreamRemove";
			this.btnDataStreamRemove.Size = new System.Drawing.Size(95, 23);
			this.btnDataStreamRemove.TabIndex = 6;
			this.btnDataStreamRemove.Text = "移除";
			this.btnDataStreamRemove.UseVisualStyleBackColor = true;
			this.btnDataStreamRemove.Click += new System.EventHandler(this.btnDataStreamRemove_Click);
			// 
			// btnDataStreamAdd
			// 
			this.btnDataStreamAdd.Location = new System.Drawing.Point(550, 11);
			this.btnDataStreamAdd.Name = "btnDataStreamAdd";
			this.btnDataStreamAdd.Size = new System.Drawing.Size(95, 23);
			this.btnDataStreamAdd.TabIndex = 5;
			this.btnDataStreamAdd.Text = "新增";
			this.btnDataStreamAdd.UseVisualStyleBackColor = true;
			this.btnDataStreamAdd.Click += new System.EventHandler(this.btnDataStreamAdd_Click);
			// 
			// dataGrid
			// 
			this.dataGrid.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(10, 11);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(534, 203);
			this.dataGrid.TabIndex = 4;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(589, 260);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "關閉";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(508, 260);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmFormatObject
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(672, 293);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmFormatObject";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Format Object";
			this.tabControl.ResumeLayout(false);
			this.pageItem_Instrument.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageItem_Instrument;
		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.Button btnDataStreamRemove;
		private System.Windows.Forms.Button btnDataStreamAdd;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnProperty;
	}
}