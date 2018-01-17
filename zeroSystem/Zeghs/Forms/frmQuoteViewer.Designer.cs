namespace Zeghs.Forms {
	partial class frmQuoteViewer {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.PrimaryBoundList<string, Zeghs.Data._QuoteInfo> source = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Zeghs.Managers.QuoteManager.Manager.onQuoteServiceSwitchChanged -= QuoteManager_onQuoteServiceSwitchChanged;
				RemoveQuoteServices();  //移除所有報價服務

				__cTimer.Dispose();  //釋放Timer
				__cQueue.Clear();

				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeSourceGrid() {
			SourceGrid.Cells.Views.IView cTextAlignView = new SourceGrid.Cells.Views.Cell();
			cTextAlignView.TextAlignment = DevAge.Drawing.ContentAlignment.TopRight;

			this.dataGrid.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid.Columns.Add("SymbolName", "名稱", typeof(string));
			this.dataGrid.Columns.Add("Time", "時間", typeof(System.TimeSpan));
			this.dataGrid.Columns.Add("Price", "成交", typeof(double));
			this.dataGrid.Columns.Add("Volume", "總量", typeof(double));
			this.dataGrid.Columns[0].Width = 56;
			this.dataGrid.Columns[1].Width = 55;
			this.dataGrid.Columns[2].DataCell.View = cTextAlignView;
			this.dataGrid.Columns[3].DataCell.View = cTextAlignView;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			source = new Data.PrimaryBoundList<string, Data._QuoteInfo>(256);
			source.SetFunctionForGetPrimary((quote) => {
				return quote.SymbolName;
			});
			this.dataGrid.DataSource = source;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dataGrid = new SourceGrid.DataGrid();
			this.SuspendLayout();
			// 
			// dataGrid
			// 
			this.dataGrid.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.EnableSort = false;
			this.dataGrid.FixedRows = 1;
			this.dataGrid.Location = new System.Drawing.Point(0, 0);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid.Size = new System.Drawing.Size(229, 176);
			this.dataGrid.TabIndex = 1;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			// 
			// frmQuoteViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(229, 176);
			this.Controls.Add(this.dataGrid);
			this.Name = "frmQuoteViewer";
			this.Text = "報價資訊";
			this.Load += new System.EventHandler(this.frmQuoteViewer_Load);
			this.Resize += new System.EventHandler(this.frmQuoteViewer_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid dataGrid;
	}
}