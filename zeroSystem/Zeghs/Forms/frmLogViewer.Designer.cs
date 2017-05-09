namespace Zeghs.Forms {
	partial class frmLogViewer {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Services.LogService __cLogService = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				__cLogService.Dispose();

				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeSourceGrid_Log() {
			this.dataGrid_Log.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid_Log.Columns.Add("LogTime", "時間", typeof(string));
			this.dataGrid_Log.Columns.Add("Level", "層級", typeof(string));
			this.dataGrid_Log.Columns.Add("Message", "訊息", typeof(string));
			this.dataGrid_Log.Columns[0].Width = 120;
			this.dataGrid_Log.Columns[1].Width = 50;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid_Log.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			__cLogService = new Services.LogService();
			this.dataGrid_Log.DataSource = __cLogService.Logs;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dataGrid_Log = new SourceGrid.DataGrid();
			this.SuspendLayout();
			// 
			// dataGrid_Log
			// 
			this.dataGrid_Log.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid_Log.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid_Log.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid_Log.EnableSort = false;
			this.dataGrid_Log.FixedRows = 1;
			this.dataGrid_Log.Location = new System.Drawing.Point(0, 0);
			this.dataGrid_Log.Name = "dataGrid_Log";
			this.dataGrid_Log.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid_Log.Size = new System.Drawing.Size(292, 148);
			this.dataGrid_Log.TabIndex = 2;
			this.dataGrid_Log.TabStop = true;
			this.dataGrid_Log.ToolTipText = "";
			// 
			// frmLogViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 148);
			this.Controls.Add(this.dataGrid_Log);
			this.Name = "frmLogViewer";
			this.Text = "日誌";
			this.Load += new System.EventHandler(this.frmLogViewer_Load);
			this.Resize += new System.EventHandler(this.frmLogViewer_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid dataGrid_Log;

	}
}