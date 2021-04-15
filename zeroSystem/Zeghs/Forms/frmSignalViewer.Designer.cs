namespace Zeghs.Forms {
	partial class frmSignalViewer {
		private static readonly string __sMessageHeader_001 = "是否移除";
		private static readonly string __sMessageContent_001 = "是否移除並終止此交易信號?";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Services.TradeService __cTradeService = null;
		private SourceGrid.Cells.Views.IView __cTextAlignView = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				__cChart.Dispose();
				__cSignalObject.Dispose();
				__cTradeService.Dispose();

				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeSourceGrid_Trust() {
			__cTextAlignView = new SourceGrid.Cells.Views.Cell();
			__cTextAlignView.TextAlignment = DevAge.Drawing.ContentAlignment.TopRight;
			
			this.dataGrid_Trust.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid_Trust.Columns.Add("Ticket", "序號", typeof(string));
			this.dataGrid_Trust.Columns.Add("Category", "委託別", typeof(string));
			this.dataGrid_Trust.Columns.Add("SymbolId", "商品代號", typeof(string));
			this.dataGrid_Trust.Columns.Add("Action", "買/賣", typeof(string));
			this.dataGrid_Trust.Columns.Add("Contracts", "數量", typeof(int));
			this.dataGrid_Trust.Columns.Add("Price", "價格", typeof(double));
			this.dataGrid_Trust.Columns.Add("Time", "時間", typeof(string));
			this.dataGrid_Trust.Columns.Add("Comment", "描述", typeof(string));
			this.dataGrid_Trust.Columns[0].Width = 60;
			this.dataGrid_Trust.Columns[1].Width = 60;
			this.dataGrid_Trust.Columns[2].Width = 80;
			this.dataGrid_Trust.Columns[3].Width = 70;
			this.dataGrid_Trust.Columns[4].Width = 60;
			this.dataGrid_Trust.Columns[4].DataCell.View = __cTextAlignView;
			this.dataGrid_Trust.Columns[5].Width = 60;
			this.dataGrid_Trust.Columns[5].DataCell.View = __cTextAlignView;
			this.dataGrid_Trust.Columns[6].Width = 110;
			this.dataGrid_Trust.Columns[7].Width = 200;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid_Trust.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			this.dataGrid_Trust.DataSource = __cTradeService.Trusts;
		}

		private void InitializeSourceGrid_Trade() {
			this.dataGrid_Trade.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid_Trade.Columns.Add("Ticket", "序號", typeof(string));
			this.dataGrid_Trade.Columns.Add("Category", "委託別", typeof(string));
			this.dataGrid_Trade.Columns.Add("SymbolId", "商品代號", typeof(string));
			this.dataGrid_Trade.Columns.Add("Action", "買/賣", typeof(string));
			this.dataGrid_Trade.Columns.Add("Contracts", "數量", typeof(int));
			this.dataGrid_Trade.Columns.Add("Price", "價格", typeof(double));
			this.dataGrid_Trade.Columns.Add("Profit", "損益", typeof(double));
			this.dataGrid_Trade.Columns.Add("Fee", "手續費", typeof(double));
			this.dataGrid_Trade.Columns.Add("Tax", "交易稅", typeof(double));
			this.dataGrid_Trade.Columns.Add("Time", "時間", typeof(string));
			this.dataGrid_Trade.Columns.Add("Comment", "描述", typeof(string));
			this.dataGrid_Trade.Columns[0].Width = 60;
			this.dataGrid_Trade.Columns[1].Width = 60;
			this.dataGrid_Trade.Columns[2].Width = 80;
			this.dataGrid_Trade.Columns[3].Width = 70;
			this.dataGrid_Trade.Columns[4].Width = 60;
			this.dataGrid_Trade.Columns[4].DataCell.View = __cTextAlignView;
			this.dataGrid_Trade.Columns[5].Width = 60;
			this.dataGrid_Trade.Columns[5].DataCell.View = __cTextAlignView;
			this.dataGrid_Trade.Columns[6].Width = 80;
			this.dataGrid_Trade.Columns[6].DataCell.View = __cTextAlignView;
			this.dataGrid_Trade.Columns[7].Width = 70;
			this.dataGrid_Trade.Columns[7].DataCell.View = __cTextAlignView;
			this.dataGrid_Trade.Columns[8].Width = 70;
			this.dataGrid_Trade.Columns[8].DataCell.View = __cTextAlignView;
			this.dataGrid_Trade.Columns[9].Width = 110;
			this.dataGrid_Trade.Columns[10].Width = 200;

			//設定總計欄位合併(沒有使用的欄位直接合併, 有使用的欄位不需合併)
			this.dataGrid_Trade.Summation(4, 1, 1, 1, 1, 1, 2);

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid_Trade.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			this.dataGrid_Trade.DataSource = __cTradeService.Opens;
		}

		private void InitializeSourceGrid_History() {
			this.dataGrid_History.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid_History.Columns.Add("Ticket", "序號", typeof(string));
			this.dataGrid_History.Columns.Add("Category", "委託別", typeof(string));
			this.dataGrid_History.Columns.Add("SymbolId", "商品代號", typeof(string));
			this.dataGrid_History.Columns.Add("Action", "買/賣", typeof(string));
			this.dataGrid_History.Columns.Add("Contracts", "數量", typeof(int));
			this.dataGrid_History.Columns.Add("Price", "價格", typeof(double));
			this.dataGrid_History.Columns.Add("Profit", "損益", typeof(double));
			this.dataGrid_History.Columns.Add("Fee", "手續費", typeof(double));
			this.dataGrid_History.Columns.Add("Tax", "交易稅", typeof(double));
			this.dataGrid_History.Columns.Add("Time", "時間", typeof(string));
			this.dataGrid_History.Columns.Add("Comment", "描述", typeof(string));
			this.dataGrid_History.Columns[0].Width = 60;
			this.dataGrid_History.Columns[1].Width = 60;
			this.dataGrid_History.Columns[2].Width = 80;
			this.dataGrid_History.Columns[3].Width = 70;
			this.dataGrid_History.Columns[4].Width = 60;
			this.dataGrid_History.Columns[4].DataCell.View = __cTextAlignView;
			this.dataGrid_History.Columns[5].Width = 60;
			this.dataGrid_History.Columns[5].DataCell.View = __cTextAlignView;
			this.dataGrid_History.Columns[6].Width = 80;
			this.dataGrid_History.Columns[6].DataCell.View = __cTextAlignView;
			this.dataGrid_History.Columns[7].Width = 70;
			this.dataGrid_History.Columns[7].DataCell.View = __cTextAlignView;
			this.dataGrid_History.Columns[8].Width = 70;
			this.dataGrid_History.Columns[8].DataCell.View = __cTextAlignView;
			this.dataGrid_History.Columns[9].Width = 110;
			this.dataGrid_History.Columns[10].Width = 200;

			//設定總計欄位合併(沒有使用的欄位直接合併, 有使用的欄位不需合併)
			this.dataGrid_History.Summation(4, 1, 1, 1, 1, 1, 2);

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid_History.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			this.dataGrid_History.DataSource = __cTradeService.Closes;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.dataGrid_Trust = new SourceGrid.DataGrid();
			this.dataGrid_Trade = new Zeghs.Controls.CustomGrid();
			this.dataGrid_History = new Zeghs.Controls.CustomGrid();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.pageItem_Trust = new System.Windows.Forms.TabPage();
			this.pageItem_Trade = new System.Windows.Forms.TabPage();
			this.pageItem_History = new System.Windows.Forms.TabPage();
			this.panelQuery = new System.Windows.Forms.Panel();
			this.labChar = new System.Windows.Forms.Label();
			this.datePickerStop = new System.Windows.Forms.DateTimePicker();
			this.btnQuery = new System.Windows.Forms.Button();
			this.datePickerStart = new System.Windows.Forms.DateTimePicker();
			this.comboMode = new System.Windows.Forms.ComboBox();
			this.labDate = new System.Windows.Forms.Label();
			this.txtSymbol = new System.Windows.Forms.TextBox();
			this.labSymbol = new System.Windows.Forms.Label();
			this.pageItem_Output = new System.Windows.Forms.TabPage();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemSigner = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemParameters = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDataSource = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemReLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTradeDetails = new System.Windows.Forms.ToolStripMenuItem();
			this.chart = new System.Windows.Forms.Control();
			this.tabControl.SuspendLayout();
			this.pageItem_Trust.SuspendLayout();
			this.pageItem_Trade.SuspendLayout();
			this.pageItem_History.SuspendLayout();
			this.panelQuery.SuspendLayout();
			this.pageItem_Output.SuspendLayout();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGrid_Trust
			// 
			this.dataGrid_Trust.BackColor = System.Drawing.SystemColors.Window;
			this.dataGrid_Trust.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid_Trust.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid_Trust.EnableSort = false;
			this.dataGrid_Trust.FixedRows = 1;
			this.dataGrid_Trust.Location = new System.Drawing.Point(0, 0);
			this.dataGrid_Trust.Name = "dataGrid_Trust";
			this.dataGrid_Trust.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid_Trust.Size = new System.Drawing.Size(386, 118);
			this.dataGrid_Trust.TabIndex = 1;
			this.dataGrid_Trust.TabStop = true;
			this.dataGrid_Trust.ToolTipText = "";
			this.dataGrid_Trust.VisibleChanged += new System.EventHandler(this.dataGrid_Trust_VisibleChanged);
			// 
			// dataGrid_Trade
			// 
			this.dataGrid_Trade.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid_Trade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid_Trade.EnableSort = false;
			this.dataGrid_Trade.FixedRows = 1;
			this.dataGrid_Trade.Location = new System.Drawing.Point(0, 0);
			this.dataGrid_Trade.Name = "dataGrid_Trade";
			this.dataGrid_Trade.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid_Trade.Size = new System.Drawing.Size(386, 118);
			this.dataGrid_Trade.TabIndex = 0;
			this.dataGrid_Trade.TabStop = true;
			this.dataGrid_Trade.ToolTipText = "";
			this.dataGrid_Trade.VisibleChanged += new System.EventHandler(this.dataGrid_Trade_VisibleChanged);
			// 
			// dataGrid_History
			// 
			this.dataGrid_History.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.dataGrid_History.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid_History.EnableSort = false;
			this.dataGrid_History.FixedRows = 1;
			this.dataGrid_History.Location = new System.Drawing.Point(0, 30);
			this.dataGrid_History.Name = "dataGrid_History";
			this.dataGrid_History.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.dataGrid_History.Size = new System.Drawing.Size(386, 88);
			this.dataGrid_History.TabIndex = 3;
			this.dataGrid_History.TabStop = true;
			this.dataGrid_History.ToolTipText = "";
			this.dataGrid_History.VisibleChanged += new System.EventHandler(this.dataGrid_History_VisibleChanged);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.pageItem_Trust);
			this.tabControl.Controls.Add(this.pageItem_Trade);
			this.tabControl.Controls.Add(this.pageItem_History);
			this.tabControl.Controls.Add(this.pageItem_Output);
			this.tabControl.Location = new System.Drawing.Point(0, 152);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(394, 143);
			this.tabControl.TabIndex = 0;
			this.tabControl.DoubleClick += new System.EventHandler(this.tabControl_DoubleClick);
			// 
			// pageItem_Trust
			// 
			this.pageItem_Trust.Controls.Add(this.dataGrid_Trust);
			this.pageItem_Trust.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Trust.Margin = new System.Windows.Forms.Padding(0);
			this.pageItem_Trust.Name = "pageItem_Trust";
			this.pageItem_Trust.Size = new System.Drawing.Size(386, 118);
			this.pageItem_Trust.TabIndex = 0;
			this.pageItem_Trust.Text = "委託";
			this.pageItem_Trust.UseVisualStyleBackColor = true;
			// 
			// pageItem_Trade
			// 
			this.pageItem_Trade.Controls.Add(this.dataGrid_Trade);
			this.pageItem_Trade.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Trade.Margin = new System.Windows.Forms.Padding(0);
			this.pageItem_Trade.Name = "pageItem_Trade";
			this.pageItem_Trade.Size = new System.Drawing.Size(386, 118);
			this.pageItem_Trade.TabIndex = 1;
			this.pageItem_Trade.Text = "交易";
			this.pageItem_Trade.UseVisualStyleBackColor = true;
			// 
			// pageItem_History
			// 
			this.pageItem_History.Controls.Add(this.dataGrid_History);
			this.pageItem_History.Controls.Add(this.panelQuery);
			this.pageItem_History.Location = new System.Drawing.Point(4, 21);
			this.pageItem_History.Margin = new System.Windows.Forms.Padding(0);
			this.pageItem_History.Name = "pageItem_History";
			this.pageItem_History.Size = new System.Drawing.Size(386, 118);
			this.pageItem_History.TabIndex = 2;
			this.pageItem_History.Text = "歷史";
			this.pageItem_History.UseVisualStyleBackColor = true;
			// 
			// panelQuery
			// 
			this.panelQuery.Controls.Add(this.labChar);
			this.panelQuery.Controls.Add(this.datePickerStop);
			this.panelQuery.Controls.Add(this.btnQuery);
			this.panelQuery.Controls.Add(this.datePickerStart);
			this.panelQuery.Controls.Add(this.comboMode);
			this.panelQuery.Controls.Add(this.labDate);
			this.panelQuery.Controls.Add(this.txtSymbol);
			this.panelQuery.Controls.Add(this.labSymbol);
			this.panelQuery.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelQuery.Location = new System.Drawing.Point(0, 0);
			this.panelQuery.Name = "panelQuery";
			this.panelQuery.Size = new System.Drawing.Size(386, 30);
			this.panelQuery.TabIndex = 2;
			// 
			// labChar
			// 
			this.labChar.AutoSize = true;
			this.labChar.Location = new System.Drawing.Point(279, 9);
			this.labChar.Name = "labChar";
			this.labChar.Size = new System.Drawing.Size(11, 12);
			this.labChar.TabIndex = 16;
			this.labChar.Text = "~";
			// 
			// datePickerStop
			// 
			this.datePickerStop.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.datePickerStop.Location = new System.Drawing.Point(294, 4);
			this.datePickerStop.Name = "datePickerStop";
			this.datePickerStop.Size = new System.Drawing.Size(82, 22);
			this.datePickerStop.TabIndex = 15;
			// 
			// btnQuery
			// 
			this.btnQuery.Location = new System.Drawing.Point(377, 3);
			this.btnQuery.Name = "btnQuery";
			this.btnQuery.Size = new System.Drawing.Size(41, 23);
			this.btnQuery.TabIndex = 12;
			this.btnQuery.Text = "查詢";
			this.btnQuery.UseVisualStyleBackColor = true;
			this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
			// 
			// datePickerStart
			// 
			this.datePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.datePickerStart.Location = new System.Drawing.Point(193, 4);
			this.datePickerStart.Name = "datePickerStart";
			this.datePickerStart.Size = new System.Drawing.Size(82, 22);
			this.datePickerStart.TabIndex = 14;
			// 
			// comboMode
			// 
			this.comboMode.FormattingEnabled = true;
			this.comboMode.Items.AddRange(new object[] {
            "全部",
            "今日",
            "區間"});
			this.comboMode.Location = new System.Drawing.Point(137, 4);
			this.comboMode.Name = "comboMode";
			this.comboMode.Size = new System.Drawing.Size(54, 20);
			this.comboMode.TabIndex = 13;
			this.comboMode.SelectedIndexChanged += new System.EventHandler(this.comboMode_SelectedIndexChanged);
			// 
			// labDate
			// 
			this.labDate.AutoSize = true;
			this.labDate.Location = new System.Drawing.Point(104, 6);
			this.labDate.Name = "labDate";
			this.labDate.Size = new System.Drawing.Size(29, 12);
			this.labDate.TabIndex = 11;
			this.labDate.Text = "日期";
			// 
			// txtSymbol
			// 
			this.txtSymbol.Location = new System.Drawing.Point(40, 3);
			this.txtSymbol.Name = "txtSymbol";
			this.txtSymbol.Size = new System.Drawing.Size(57, 22);
			this.txtSymbol.TabIndex = 10;
			// 
			// labSymbol
			// 
			this.labSymbol.AutoSize = true;
			this.labSymbol.Location = new System.Drawing.Point(6, 6);
			this.labSymbol.Name = "labSymbol";
			this.labSymbol.Size = new System.Drawing.Size(29, 12);
			this.labSymbol.TabIndex = 9;
			this.labSymbol.Text = "商品";
			// 
			// pageItem_Output
			// 
			this.pageItem_Output.Controls.Add(this.txtOutput);
			this.pageItem_Output.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Output.Name = "pageItem_Output";
			this.pageItem_Output.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Output.Size = new System.Drawing.Size(386, 118);
			this.pageItem_Output.TabIndex = 3;
			this.pageItem_Output.Text = "輸出";
			this.pageItem_Output.UseVisualStyleBackColor = true;
			// 
			// txtOutput
			// 
			this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutput.Location = new System.Drawing.Point(3, 3);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutput.Size = new System.Drawing.Size(380, 112);
			this.txtOutput.TabIndex = 0;
			this.txtOutput.VisibleChanged += new System.EventHandler(this.txtOutput_VisibleChanged);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSigner,
            this.menuItemTradeDetails});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(153, 70);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// menuItemSigner
			// 
			this.menuItemSigner.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemParameters,
            this.menuItemDataSource,
            this.menuItemRemove,
            this.toolStripSeparator1,
            this.menuItemReLoad});
			this.menuItemSigner.Name = "menuItemSigner";
			this.menuItemSigner.Size = new System.Drawing.Size(152, 22);
			this.menuItemSigner.Text = "信號交易";
			// 
			// menuItemParameters
			// 
			this.menuItemParameters.Name = "menuItemParameters";
			this.menuItemParameters.Size = new System.Drawing.Size(152, 22);
			this.menuItemParameters.Text = "參數設定";
			this.menuItemParameters.Click += new System.EventHandler(this.menuItemParameters_Click);
			// 
			// menuItemDataSource
			// 
			this.menuItemDataSource.Name = "menuItemDataSource";
			this.menuItemDataSource.Size = new System.Drawing.Size(152, 22);
			this.menuItemDataSource.Text = "資料來源";
			this.menuItemDataSource.Click += new System.EventHandler(this.menuItemDataSource_Click);
			// 
			// menuItemReLoad
			// 
			this.menuItemReLoad.Name = "menuItemReLoad";
			this.menuItemReLoad.Size = new System.Drawing.Size(152, 22);
			this.menuItemReLoad.Text = "重新載入";
			this.menuItemReLoad.Click += new System.EventHandler(this.menuItemReLoad_Click);
			// 
			// menuItemRemove
			// 
			this.menuItemRemove.Name = "menuItemRemove";
			this.menuItemRemove.Size = new System.Drawing.Size(152, 22);
			this.menuItemRemove.Text = "刪除信號";
			this.menuItemRemove.Click += new System.EventHandler(this.menuItemRemove_Click);
			// 
			// menuItemTradeDetails
			// 
			this.menuItemTradeDetails.Name = "menuItemTradeDetails";
			this.menuItemTradeDetails.Size = new System.Drawing.Size(152, 22);
			this.menuItemTradeDetails.Text = "交易明細";
			this.menuItemTradeDetails.Click += new System.EventHandler(this.menuItemTradeDetails_Click);
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(0, 0);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(394, 150);
			this.chart.TabIndex = 1;
			this.chart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chart_onMouseUp);
			// 
			// frmSignalViewer
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 294);
			this.ContextMenuStrip = this.contextMenu;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.chart);
			this.Name = "frmSignalViewer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSignalViewer_FormClosing);
			this.Load += new System.EventHandler(this.frmSignalViewer_Load);
			this.Shown += new System.EventHandler(this.frmSignalViewer_Shown);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmSignalViewer_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmSignalViewer_DragEnter);
			this.Resize += new System.EventHandler(this.frmSignalViewer_Resize);
			this.tabControl.ResumeLayout(false);
			this.pageItem_Trust.ResumeLayout(false);
			this.pageItem_Trade.ResumeLayout(false);
			this.pageItem_History.ResumeLayout(false);
			this.panelQuery.ResumeLayout(false);
			this.panelQuery.PerformLayout();
			this.pageItem_Output.ResumeLayout(false);
			this.pageItem_Output.PerformLayout();
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid dataGrid_Trust;
		private Zeghs.Controls.CustomGrid dataGrid_Trade;
		private Zeghs.Controls.CustomGrid dataGrid_History;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageItem_Trust;
		private System.Windows.Forms.TabPage pageItem_Trade;
		private System.Windows.Forms.TabPage pageItem_History;
		private System.Windows.Forms.TabPage pageItem_Output;
		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemSigner;
		private System.Windows.Forms.ToolStripMenuItem menuItemParameters;
		private System.Windows.Forms.ToolStripMenuItem menuItemRemove;
		private System.Windows.Forms.Control chart;
		private System.Windows.Forms.ToolStripMenuItem menuItemTradeDetails;
		private System.Windows.Forms.ToolStripMenuItem menuItemDataSource;
		private System.Windows.Forms.Panel panelQuery;
		private System.Windows.Forms.Label labChar;
		private System.Windows.Forms.DateTimePicker datePickerStop;
		private System.Windows.Forms.Button btnQuery;
		private System.Windows.Forms.DateTimePicker datePickerStart;
		private System.Windows.Forms.ComboBox comboMode;
		private System.Windows.Forms.Label labDate;
		private System.Windows.Forms.TextBox txtSymbol;
		private System.Windows.Forms.Label labSymbol;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuItemReLoad;
	}
}