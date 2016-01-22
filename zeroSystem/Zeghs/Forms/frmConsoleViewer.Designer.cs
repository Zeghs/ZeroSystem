namespace Zeghs.Forms {
	partial class frmConsoleViewer {
		private static int __iFixGridBottomCount = 0;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Services.LogService __cLogService = null;
		private Zeghs.Utils.OutputWriter __cOutputWriter = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				__cLogService.Dispose();
				__cOutputWriter.Dispose();

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

		private void InitializeConsoleOutput() {
			__cOutputWriter = new Utils.OutputWriter();
			__cOutputWriter.onOutputData += OutputWriter_onOutputData;

			System.Console.SetOut(__cOutputWriter);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.consoleTabControl = new Zeghs.Controls.CustomTabControl();
			this.pageItem_LOG = new System.Windows.Forms.TabPage();
			this.dataGrid_Log = new SourceGrid.DataGrid();
			this.pageItem_Output = new System.Windows.Forms.TabPage();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.consoleTabControl.SuspendLayout();
			this.pageItem_LOG.SuspendLayout();
			this.pageItem_Output.SuspendLayout();
			this.SuspendLayout();
			// 
			// consoleTabControl
			// 
			this.consoleTabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.consoleTabControl.Controls.Add(this.pageItem_LOG);
			this.consoleTabControl.Controls.Add(this.pageItem_Output);
			this.consoleTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.consoleTabControl.Location = new System.Drawing.Point(0, 0);
			this.consoleTabControl.Name = "consoleTabControl";
			this.consoleTabControl.SelectedIndex = 0;
			this.consoleTabControl.Size = new System.Drawing.Size(292, 144);
			this.consoleTabControl.TabIndex = 0;
			// 
			// pageItem_LOG
			// 
			this.pageItem_LOG.Controls.Add(this.dataGrid_Log);
			this.pageItem_LOG.Location = new System.Drawing.Point(1, 1);
			this.pageItem_LOG.Margin = new System.Windows.Forms.Padding(0);
			this.pageItem_LOG.Name = "pageItem_LOG";
			this.pageItem_LOG.Size = new System.Drawing.Size(290, 119);
			this.pageItem_LOG.TabIndex = 2;
			this.pageItem_LOG.Text = "日誌";
			this.pageItem_LOG.UseVisualStyleBackColor = true;
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
			this.dataGrid_Log.Size = new System.Drawing.Size(290, 119);
			this.dataGrid_Log.TabIndex = 1;
			this.dataGrid_Log.TabStop = true;
			this.dataGrid_Log.ToolTipText = "";
			this.dataGrid_Log.VisibleChanged += new System.EventHandler(this.dataGrid_Log_VisibleChanged);
			// 
			// pageItem_Output
			// 
			this.pageItem_Output.Controls.Add(this.txtOutput);
			this.pageItem_Output.Location = new System.Drawing.Point(1, 1);
			this.pageItem_Output.Name = "pageItem_Output";
			this.pageItem_Output.Size = new System.Drawing.Size(290, 119);
			this.pageItem_Output.TabIndex = 3;
			this.pageItem_Output.Text = "輸出";
			this.pageItem_Output.UseVisualStyleBackColor = true;
			// 
			// txtOutput
			// 
			this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutput.Location = new System.Drawing.Point(0, 0);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutput.Size = new System.Drawing.Size(290, 119);
			this.txtOutput.TabIndex = 0;
			this.txtOutput.VisibleChanged += new System.EventHandler(this.txtOutput_VisibleChanged);
			// 
			// frmConsoleViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 144);
			this.ControlBox = false;
			this.Controls.Add(this.consoleTabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frmConsoleViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Load += new System.EventHandler(this.frmConsoleViewer_Load);
			this.Resize += new System.EventHandler(this.frmConsoleViewer_Resize);
			this.consoleTabControl.ResumeLayout(false);
			this.pageItem_LOG.ResumeLayout(false);
			this.pageItem_Output.ResumeLayout(false);
			this.pageItem_Output.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.CustomTabControl consoleTabControl;
		private System.Windows.Forms.TabPage pageItem_LOG;
		private System.Windows.Forms.TabPage pageItem_Output;
		private SourceGrid.DataGrid dataGrid_Log;
		private System.Windows.Forms.TextBox txtOutput;
	}
}