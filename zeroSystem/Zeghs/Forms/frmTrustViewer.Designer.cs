namespace Zeghs.Forms {
	partial class frmTrustViewer {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.SimpleBoundList<Zeghs.Data._DOMInfo> source = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Zeghs.Services.AbstractQuoteService cService = Zeghs.Managers.QuoteManager.Manager.GetQuoteService(__sDataSource);
				if (cService != null) {
					cService.onQuote -= QuoteService_onQuote;
				}

				if (__cTimer != null) {
					__cTimer.Dispose();  //釋放Timer
				}

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
			this.dataGrid.Columns.Add("BidSize", "委買量", typeof(double));
			this.dataGrid.Columns.Add("BidPrice", "委買價", typeof(double));
			this.dataGrid.Columns.Add("AskPrice", "委賣價", typeof(double));
			this.dataGrid.Columns.Add("AskSize", "委賣量", typeof(double));
			this.dataGrid.Columns[0].Width = 60;
			this.dataGrid.Columns[0].DataCell.View = cTextAlignView;
			this.dataGrid.Columns[1].Width = 60;
			this.dataGrid.Columns[1].DataCell.View = cTextAlignView;
			this.dataGrid.Columns[2].Width = 60;
			this.dataGrid.Columns[2].DataCell.View = cTextAlignView;
			this.dataGrid.Columns[3].Width = 60;
			this.dataGrid.Columns[3].DataCell.View = cTextAlignView;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			source = new Data.SimpleBoundList<Data._DOMInfo>(16);
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
			this.dataGrid.Size = new System.Drawing.Size(259, 126);
			this.dataGrid.TabIndex = 2;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			// 
			// frmTrustViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(259, 126);
			this.Controls.Add(this.dataGrid);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTrustViewer";
			this.Text = "委託價量表";
			this.Load += new System.EventHandler(this.frmTrustViewer_Load);
			this.Shown += new System.EventHandler(this.frmTrustViewer_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid dataGrid;
	}
}