namespace Zeghs.Forms {
	partial class frmCreateScriptSetting {
		private static readonly string __sMessageHeader_001 = "資料錯誤";
		private static readonly string __sMessageContent_001 = "資料來源不能空白，請選擇一個報價資料來源。";
		private static readonly string __sMessageContent_002 = "商品代號不能空白，請從清單選擇一個商品。";
		private static readonly string __sMessageContent_003 = "請求資料個數不能空白，請重新輸入請求個數。";
		private static readonly string __sMessageContent_004 = "請求資料個數只能是數值，請重新輸入請求個數。";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TabPage __cCurrentPages;
		private System.Collections.Generic.List<Zeghs.Data.SimpleBoundList<Data._ProductInfo>> __cSources = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}

				__cAddSymbolIds.Clear();
	
				int iCount = __cSources.Count;
				for (int i = iCount - 1; i >= 0; i--) {
					Zeghs.Data.SimpleBoundList<Zeghs.Data._ProductInfo> cProducts = __cSources[i];
					cProducts.Clear();  //清除分類商品資訊
					__cSources.RemoveAt(i);  //移除分類商品
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeSourceGrid() {
			this.dataGrid.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.dataGrid.Columns.Add("ProductId", "商品", typeof(string));
			this.dataGrid.Columns.Add("Description", "說明", typeof(string));
			this.dataGrid.Columns.Add("ExchangeName", "交易所", typeof(string));
			this.dataGrid.Columns[0].Width = 100;
			this.dataGrid.Columns[1].Width = 155;
			this.dataGrid.Columns[2].Width = 80;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.dataGrid.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));

			//建立資料來源
			__cSources = new System.Collections.Generic.List<Data.SimpleBoundList<Data._ProductInfo>>(32);
			__cSources.Add(new Data.SimpleBoundList<Data._ProductInfo>(4096));
			this.dataGrid.DataSource = __cSources[0];

			__cCurrentPages = this.pageItem_All;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dataGrid = new SourceGrid.DataGrid();
			this.tabControl_Main = new System.Windows.Forms.TabControl();
			this.pageItem_Product = new System.Windows.Forms.TabPage();
			this.comboProduct = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboDataSources = new System.Windows.Forms.ComboBox();
			this.labDataSource = new System.Windows.Forms.Label();
			this.tabControl_Products = new System.Windows.Forms.TabControl();
			this.pageItem_All = new System.Windows.Forms.TabPage();
			this.pageItem_Settings = new System.Windows.Forms.TabPage();
			this.listSubChart = new System.Windows.Forms.ListBox();
			this.labSubChart = new System.Windows.Forms.Label();
			this.comboDataStream = new System.Windows.Forms.ComboBox();
			this.labDataStream = new System.Windows.Forms.Label();
			this.comboTimeZone = new System.Windows.Forms.ComboBox();
			this.labＴimeZone = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.labShow = new System.Windows.Forms.Label();
			this.dtEndDate = new System.Windows.Forms.DateTimePicker();
			this.label6 = new System.Windows.Forms.Label();
			this.dtStartDate = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.dtTODate = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.comboRequestMode = new System.Windows.Forms.ComboBox();
			this.txtCount = new System.Windows.Forms.TextBox();
			this.radioRange2 = new System.Windows.Forms.RadioButton();
			this.radioRange1 = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.labDataRange = new System.Windows.Forms.Label();
			this.comboResolution = new System.Windows.Forms.ComboBox();
			this.txtPeriod = new System.Windows.Forms.TextBox();
			this.labPeriods = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labPeriodType = new System.Windows.Forms.Label();
			this.pageItem_Template = new System.Windows.Forms.TabPage();
			this.labLine = new System.Windows.Forms.Label();
			this.colorLine = new DevAge.Windows.Forms.ColorPicker();
			this.labDown = new System.Windows.Forms.Label();
			this.colorDown = new DevAge.Windows.Forms.ColorPicker();
			this.labUp = new System.Windows.Forms.Label();
			this.colorUp = new DevAge.Windows.Forms.ColorPicker();
			this.label9 = new System.Windows.Forms.Label();
			this.labTemplate = new System.Windows.Forms.Label();
			this.groupExtend = new System.Windows.Forms.GroupBox();
			this.checkShowNewPrice = new System.Windows.Forms.CheckBox();
			this.listChartType = new System.Windows.Forms.ListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.labChartType = new System.Windows.Forms.Label();
			this.pageItem_Axis = new System.Windows.Forms.TabPage();
			this.txtScaleCount = new System.Windows.Forms.TextBox();
			this.radioScaleCount = new System.Windows.Forms.RadioButton();
			this.txtScaleGap = new System.Windows.Forms.TextBox();
			this.radioScaleGap = new System.Windows.Forms.RadioButton();
			this.checkManualAxis = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.labBottomMarginP = new System.Windows.Forms.Label();
			this.txtBottomMargin = new System.Windows.Forms.TextBox();
			this.labBottomMargin = new System.Windows.Forms.Label();
			this.labTopMarginP = new System.Windows.Forms.Label();
			this.txtTopMargin = new System.Windows.Forms.TextBox();
			this.labTopMargin = new System.Windows.Forms.Label();
			this.checkMargin = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.listAxisRange = new System.Windows.Forms.ListBox();
			this.label10 = new System.Windows.Forms.Label();
			this.labAxisRange = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.comboUpWidth = new Zeghs.Controls.LineWidthComboBox();
			this.comboDownWidth = new Zeghs.Controls.LineWidthComboBox();
			this.comboLineWidth = new Zeghs.Controls.LineWidthComboBox();
			this.tabControl_Main.SuspendLayout();
			this.pageItem_Product.SuspendLayout();
			this.tabControl_Products.SuspendLayout();
			this.pageItem_All.SuspendLayout();
			this.pageItem_Settings.SuspendLayout();
			this.pageItem_Template.SuspendLayout();
			this.groupExtend.SuspendLayout();
			this.pageItem_Axis.SuspendLayout();
			this.SuspendLayout();
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
			this.dataGrid.Size = new System.Drawing.Size(351, 250);
			this.dataGrid.TabIndex = 3;
			this.dataGrid.TabStop = true;
			this.dataGrid.ToolTipText = "";
			this.dataGrid.DoubleClick += new System.EventHandler(this.dataGrid_DoubleClick);
			// 
			// tabControl_Main
			// 
			this.tabControl_Main.Controls.Add(this.pageItem_Product);
			this.tabControl_Main.Controls.Add(this.pageItem_Settings);
			this.tabControl_Main.Controls.Add(this.pageItem_Template);
			this.tabControl_Main.Controls.Add(this.pageItem_Axis);
			this.tabControl_Main.Location = new System.Drawing.Point(4, 7);
			this.tabControl_Main.Name = "tabControl_Main";
			this.tabControl_Main.SelectedIndex = 0;
			this.tabControl_Main.Size = new System.Drawing.Size(388, 396);
			this.tabControl_Main.TabIndex = 0;
			// 
			// pageItem_Product
			// 
			this.pageItem_Product.Controls.Add(this.comboProduct);
			this.pageItem_Product.Controls.Add(this.label1);
			this.pageItem_Product.Controls.Add(this.comboDataSources);
			this.pageItem_Product.Controls.Add(this.labDataSource);
			this.pageItem_Product.Controls.Add(this.tabControl_Products);
			this.pageItem_Product.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Product.Name = "pageItem_Product";
			this.pageItem_Product.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Product.Size = new System.Drawing.Size(380, 371);
			this.pageItem_Product.TabIndex = 0;
			this.pageItem_Product.Text = "商品";
			this.pageItem_Product.UseVisualStyleBackColor = true;
			// 
			// comboProduct
			// 
			this.comboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProduct.FormattingEnabled = true;
			this.comboProduct.Location = new System.Drawing.Point(106, 49);
			this.comboProduct.Name = "comboProduct";
			this.comboProduct.Size = new System.Drawing.Size(127, 20);
			this.comboProduct.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "▼商品";
			// 
			// comboDataSources
			// 
			this.comboDataSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDataSources.FormattingEnabled = true;
			this.comboDataSources.Location = new System.Drawing.Point(106, 19);
			this.comboDataSources.Name = "comboDataSources";
			this.comboDataSources.Size = new System.Drawing.Size(127, 20);
			this.comboDataSources.TabIndex = 2;
			this.comboDataSources.SelectedIndexChanged += new System.EventHandler(this.comboDataSources_SelectedIndexChanged);
			// 
			// labDataSource
			// 
			this.labDataSource.AutoSize = true;
			this.labDataSource.Location = new System.Drawing.Point(18, 23);
			this.labDataSource.Name = "labDataSource";
			this.labDataSource.Size = new System.Drawing.Size(41, 12);
			this.labDataSource.TabIndex = 1;
			this.labDataSource.Text = "數據源";
			// 
			// tabControl_Products
			// 
			this.tabControl_Products.Controls.Add(this.pageItem_All);
			this.tabControl_Products.Location = new System.Drawing.Point(8, 85);
			this.tabControl_Products.Name = "tabControl_Products";
			this.tabControl_Products.SelectedIndex = 0;
			this.tabControl_Products.Size = new System.Drawing.Size(365, 281);
			this.tabControl_Products.TabIndex = 0;
			this.tabControl_Products.SelectedIndexChanged += new System.EventHandler(this.tabControl_Products_SelectedIndexChanged);
			// 
			// pageItem_All
			// 
			this.pageItem_All.Controls.Add(this.dataGrid);
			this.pageItem_All.Location = new System.Drawing.Point(4, 21);
			this.pageItem_All.Name = "pageItem_All";
			this.pageItem_All.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_All.Size = new System.Drawing.Size(357, 256);
			this.pageItem_All.TabIndex = 0;
			this.pageItem_All.Text = "全部";
			this.pageItem_All.UseVisualStyleBackColor = true;
			// 
			// pageItem_Settings
			// 
			this.pageItem_Settings.Controls.Add(this.listSubChart);
			this.pageItem_Settings.Controls.Add(this.labSubChart);
			this.pageItem_Settings.Controls.Add(this.comboDataStream);
			this.pageItem_Settings.Controls.Add(this.labDataStream);
			this.pageItem_Settings.Controls.Add(this.comboTimeZone);
			this.pageItem_Settings.Controls.Add(this.labＴimeZone);
			this.pageItem_Settings.Controls.Add(this.label7);
			this.pageItem_Settings.Controls.Add(this.labShow);
			this.pageItem_Settings.Controls.Add(this.dtEndDate);
			this.pageItem_Settings.Controls.Add(this.label6);
			this.pageItem_Settings.Controls.Add(this.dtStartDate);
			this.pageItem_Settings.Controls.Add(this.label5);
			this.pageItem_Settings.Controls.Add(this.dtTODate);
			this.pageItem_Settings.Controls.Add(this.label4);
			this.pageItem_Settings.Controls.Add(this.comboRequestMode);
			this.pageItem_Settings.Controls.Add(this.txtCount);
			this.pageItem_Settings.Controls.Add(this.radioRange2);
			this.pageItem_Settings.Controls.Add(this.radioRange1);
			this.pageItem_Settings.Controls.Add(this.label3);
			this.pageItem_Settings.Controls.Add(this.labDataRange);
			this.pageItem_Settings.Controls.Add(this.comboResolution);
			this.pageItem_Settings.Controls.Add(this.txtPeriod);
			this.pageItem_Settings.Controls.Add(this.labPeriods);
			this.pageItem_Settings.Controls.Add(this.label2);
			this.pageItem_Settings.Controls.Add(this.labPeriodType);
			this.pageItem_Settings.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Settings.Name = "pageItem_Settings";
			this.pageItem_Settings.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Settings.Size = new System.Drawing.Size(380, 371);
			this.pageItem_Settings.TabIndex = 1;
			this.pageItem_Settings.Text = "設定";
			this.pageItem_Settings.UseVisualStyleBackColor = true;
			// 
			// listSubChart
			// 
			this.listSubChart.FormattingEnabled = true;
			this.listSubChart.ItemHeight = 12;
			this.listSubChart.Items.AddRange(new object[] {
            "副圖",
            "隱藏"});
			this.listSubChart.Location = new System.Drawing.Point(132, 287);
			this.listSubChart.Name = "listSubChart";
			this.listSubChart.Size = new System.Drawing.Size(121, 52);
			this.listSubChart.TabIndex = 24;
			// 
			// labSubChart
			// 
			this.labSubChart.AutoSize = true;
			this.labSubChart.Location = new System.Drawing.Point(35, 291);
			this.labSubChart.Name = "labSubChart";
			this.labSubChart.Size = new System.Drawing.Size(32, 12);
			this.labSubChart.TabIndex = 23;
			this.labSubChart.Text = "副圖:";
			// 
			// comboDataStream
			// 
			this.comboDataStream.FormattingEnabled = true;
			this.comboDataStream.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
			this.comboDataStream.Location = new System.Drawing.Point(132, 259);
			this.comboDataStream.Name = "comboDataStream";
			this.comboDataStream.Size = new System.Drawing.Size(121, 20);
			this.comboDataStream.TabIndex = 22;
			// 
			// labDataStream
			// 
			this.labDataStream.AutoSize = true;
			this.labDataStream.Location = new System.Drawing.Point(35, 265);
			this.labDataStream.Name = "labDataStream";
			this.labDataStream.Size = new System.Drawing.Size(56, 12);
			this.labDataStream.TabIndex = 21;
			this.labDataStream.Text = "數列編號:";
			// 
			// comboTimeZone
			// 
			this.comboTimeZone.FormattingEnabled = true;
			this.comboTimeZone.Items.AddRange(new object[] {
            "交易所"});
			this.comboTimeZone.Location = new System.Drawing.Point(132, 232);
			this.comboTimeZone.Name = "comboTimeZone";
			this.comboTimeZone.Size = new System.Drawing.Size(121, 20);
			this.comboTimeZone.TabIndex = 20;
			// 
			// labＴimeZone
			// 
			this.labＴimeZone.AutoSize = true;
			this.labＴimeZone.Location = new System.Drawing.Point(35, 238);
			this.labＴimeZone.Name = "labＴimeZone";
			this.labＴimeZone.Size = new System.Drawing.Size(32, 12);
			this.labＴimeZone.TabIndex = 19;
			this.labＴimeZone.Text = "時區:";
			// 
			// label7
			// 
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label7.Location = new System.Drawing.Point(52, 222);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(307, 2);
			this.label7.TabIndex = 18;
			// 
			// labShow
			// 
			this.labShow.AutoSize = true;
			this.labShow.Location = new System.Drawing.Point(14, 217);
			this.labShow.Name = "labShow";
			this.labShow.Size = new System.Drawing.Size(32, 12);
			this.labShow.TabIndex = 17;
			this.labShow.Text = "顯示:";
			// 
			// dtEndDate
			// 
			this.dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtEndDate.Location = new System.Drawing.Point(262, 184);
			this.dtEndDate.Name = "dtEndDate";
			this.dtEndDate.Size = new System.Drawing.Size(90, 22);
			this.dtEndDate.TabIndex = 16;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(228, 189);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 12);
			this.label6.TabIndex = 15;
			this.label6.Text = "到:";
			// 
			// dtStartDate
			// 
			this.dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtStartDate.Location = new System.Drawing.Point(132, 184);
			this.dtStartDate.Name = "dtStartDate";
			this.dtStartDate.Size = new System.Drawing.Size(90, 22);
			this.dtStartDate.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(66, 189);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(20, 12);
			this.label5.TabIndex = 13;
			this.label5.Text = "從:";
			// 
			// dtTODate
			// 
			this.dtTODate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtTODate.Location = new System.Drawing.Point(262, 156);
			this.dtTODate.Name = "dtTODate";
			this.dtTODate.Size = new System.Drawing.Size(90, 22);
			this.dtTODate.TabIndex = 12;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(228, 161);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(20, 12);
			this.label4.TabIndex = 11;
			this.label4.Text = "從:";
			// 
			// comboRequestMode
			// 
			this.comboRequestMode.FormattingEnabled = true;
			this.comboRequestMode.Items.AddRange(new object[] {
            "根K線使用",
            "日資料使用"});
			this.comboRequestMode.Location = new System.Drawing.Point(132, 157);
			this.comboRequestMode.Name = "comboRequestMode";
			this.comboRequestMode.Size = new System.Drawing.Size(90, 20);
			this.comboRequestMode.TabIndex = 10;
			// 
			// txtCount
			// 
			this.txtCount.Location = new System.Drawing.Point(63, 156);
			this.txtCount.Name = "txtCount";
			this.txtCount.Size = new System.Drawing.Size(59, 22);
			this.txtCount.TabIndex = 9;
			// 
			// radioRange2
			// 
			this.radioRange2.AutoSize = true;
			this.radioRange2.Location = new System.Drawing.Point(35, 188);
			this.radioRange2.Name = "radioRange2";
			this.radioRange2.Size = new System.Drawing.Size(14, 13);
			this.radioRange2.TabIndex = 8;
			this.radioRange2.UseVisualStyleBackColor = true;
			this.radioRange2.CheckedChanged += new System.EventHandler(this.radioRange2_CheckedChanged);
			// 
			// radioRange1
			// 
			this.radioRange1.AutoSize = true;
			this.radioRange1.Location = new System.Drawing.Point(35, 160);
			this.radioRange1.Name = "radioRange1";
			this.radioRange1.Size = new System.Drawing.Size(14, 13);
			this.radioRange1.TabIndex = 7;
			this.radioRange1.UseVisualStyleBackColor = true;
			this.radioRange1.CheckedChanged += new System.EventHandler(this.radioRange1_CheckedChanged);
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(75, 139);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(284, 2);
			this.label3.TabIndex = 6;
			// 
			// labDataRange
			// 
			this.labDataRange.AutoSize = true;
			this.labDataRange.Location = new System.Drawing.Point(14, 134);
			this.labDataRange.Name = "labDataRange";
			this.labDataRange.Size = new System.Drawing.Size(56, 12);
			this.labDataRange.TabIndex = 5;
			this.labDataRange.Text = "資料區間:";
			// 
			// comboResolution
			// 
			this.comboResolution.FormattingEnabled = true;
			this.comboResolution.Location = new System.Drawing.Point(190, 39);
			this.comboResolution.Name = "comboResolution";
			this.comboResolution.Size = new System.Drawing.Size(63, 20);
			this.comboResolution.TabIndex = 4;
			// 
			// txtPeriod
			// 
			this.txtPeriod.Location = new System.Drawing.Point(116, 38);
			this.txtPeriod.Name = "txtPeriod";
			this.txtPeriod.Size = new System.Drawing.Size(67, 22);
			this.txtPeriod.TabIndex = 3;
			this.txtPeriod.Text = "1";
			// 
			// labPeriods
			// 
			this.labPeriods.AutoSize = true;
			this.labPeriods.Location = new System.Drawing.Point(33, 44);
			this.labPeriods.Name = "labPeriods";
			this.labPeriods.Size = new System.Drawing.Size(32, 12);
			this.labPeriods.TabIndex = 2;
			this.labPeriods.Text = "週期:";
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(75, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(284, 2);
			this.label2.TabIndex = 1;
			// 
			// labPeriodType
			// 
			this.labPeriodType.AutoSize = true;
			this.labPeriodType.Location = new System.Drawing.Point(14, 17);
			this.labPeriodType.Name = "labPeriodType";
			this.labPeriodType.Size = new System.Drawing.Size(56, 12);
			this.labPeriodType.TabIndex = 0;
			this.labPeriodType.Text = "週期類型:";
			// 
			// pageItem_Template
			// 
			this.pageItem_Template.Controls.Add(this.comboLineWidth);
			this.pageItem_Template.Controls.Add(this.comboDownWidth);
			this.pageItem_Template.Controls.Add(this.comboUpWidth);
			this.pageItem_Template.Controls.Add(this.labLine);
			this.pageItem_Template.Controls.Add(this.colorLine);
			this.pageItem_Template.Controls.Add(this.labDown);
			this.pageItem_Template.Controls.Add(this.colorDown);
			this.pageItem_Template.Controls.Add(this.labUp);
			this.pageItem_Template.Controls.Add(this.colorUp);
			this.pageItem_Template.Controls.Add(this.label9);
			this.pageItem_Template.Controls.Add(this.labTemplate);
			this.pageItem_Template.Controls.Add(this.groupExtend);
			this.pageItem_Template.Controls.Add(this.listChartType);
			this.pageItem_Template.Controls.Add(this.label8);
			this.pageItem_Template.Controls.Add(this.labChartType);
			this.pageItem_Template.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Template.Name = "pageItem_Template";
			this.pageItem_Template.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Template.Size = new System.Drawing.Size(380, 371);
			this.pageItem_Template.TabIndex = 2;
			this.pageItem_Template.Text = "樣式";
			this.pageItem_Template.UseVisualStyleBackColor = true;
			// 
			// labLine
			// 
			this.labLine.AutoSize = true;
			this.labLine.Location = new System.Drawing.Point(32, 254);
			this.labLine.Name = "labLine";
			this.labLine.Size = new System.Drawing.Size(32, 12);
			this.labLine.TabIndex = 17;
			this.labLine.Text = "影線:";
			// 
			// colorLine
			// 
			this.colorLine.Location = new System.Drawing.Point(98, 250);
			this.colorLine.Name = "colorLine";
			this.colorLine.SelectedColor = System.Drawing.Color.Black;
			this.colorLine.Size = new System.Drawing.Size(132, 21);
			this.colorLine.TabIndex = 16;
			// 
			// labDown
			// 
			this.labDown.AutoSize = true;
			this.labDown.Location = new System.Drawing.Point(32, 225);
			this.labDown.Name = "labDown";
			this.labDown.Size = new System.Drawing.Size(32, 12);
			this.labDown.TabIndex = 13;
			this.labDown.Text = "下方:";
			// 
			// colorDown
			// 
			this.colorDown.Location = new System.Drawing.Point(98, 221);
			this.colorDown.Name = "colorDown";
			this.colorDown.SelectedColor = System.Drawing.Color.Black;
			this.colorDown.Size = new System.Drawing.Size(132, 21);
			this.colorDown.TabIndex = 12;
			// 
			// labUp
			// 
			this.labUp.AutoSize = true;
			this.labUp.Location = new System.Drawing.Point(32, 196);
			this.labUp.Name = "labUp";
			this.labUp.Size = new System.Drawing.Size(32, 12);
			this.labUp.TabIndex = 9;
			this.labUp.Text = "上方:";
			// 
			// colorUp
			// 
			this.colorUp.Location = new System.Drawing.Point(98, 192);
			this.colorUp.Name = "colorUp";
			this.colorUp.SelectedColor = System.Drawing.Color.Black;
			this.colorUp.Size = new System.Drawing.Size(132, 21);
			this.colorUp.TabIndex = 8;
			// 
			// label9
			// 
			this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label9.Location = new System.Drawing.Point(75, 176);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(284, 2);
			this.label9.TabIndex = 7;
			// 
			// labTemplate
			// 
			this.labTemplate.AutoSize = true;
			this.labTemplate.Location = new System.Drawing.Point(14, 171);
			this.labTemplate.Name = "labTemplate";
			this.labTemplate.Size = new System.Drawing.Size(56, 12);
			this.labTemplate.TabIndex = 6;
			this.labTemplate.Text = "圖表樣式:";
			// 
			// groupExtend
			// 
			this.groupExtend.Controls.Add(this.checkShowNewPrice);
			this.groupExtend.Location = new System.Drawing.Point(155, 53);
			this.groupExtend.Name = "groupExtend";
			this.groupExtend.Size = new System.Drawing.Size(171, 101);
			this.groupExtend.TabIndex = 5;
			this.groupExtend.TabStop = false;
			this.groupExtend.Text = "進階";
			// 
			// checkShowNewPrice
			// 
			this.checkShowNewPrice.AutoSize = true;
			this.checkShowNewPrice.Location = new System.Drawing.Point(17, 22);
			this.checkShowNewPrice.Name = "checkShowNewPrice";
			this.checkShowNewPrice.Size = new System.Drawing.Size(96, 16);
			this.checkShowNewPrice.TabIndex = 1;
			this.checkShowNewPrice.Text = "顯示最新價格";
			this.checkShowNewPrice.UseVisualStyleBackColor = true;
			// 
			// listChartType
			// 
			this.listChartType.FormattingEnabled = true;
			this.listChartType.ItemHeight = 12;
			this.listChartType.Items.AddRange(new object[] {
            "美國線",
            "HLC線",
            "蠟燭線",
            "收盤線"});
			this.listChartType.Location = new System.Drawing.Point(34, 42);
			this.listChartType.Name = "listChartType";
			this.listChartType.Size = new System.Drawing.Size(106, 112);
			this.listChartType.TabIndex = 4;
			this.listChartType.SelectedIndexChanged += new System.EventHandler(this.listChartType_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.Location = new System.Drawing.Point(75, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(284, 2);
			this.label8.TabIndex = 3;
			// 
			// labChartType
			// 
			this.labChartType.AutoSize = true;
			this.labChartType.Location = new System.Drawing.Point(14, 17);
			this.labChartType.Name = "labChartType";
			this.labChartType.Size = new System.Drawing.Size(56, 12);
			this.labChartType.TabIndex = 2;
			this.labChartType.Text = "圖表類型:";
			// 
			// pageItem_Axis
			// 
			this.pageItem_Axis.Controls.Add(this.txtScaleCount);
			this.pageItem_Axis.Controls.Add(this.radioScaleCount);
			this.pageItem_Axis.Controls.Add(this.txtScaleGap);
			this.pageItem_Axis.Controls.Add(this.radioScaleGap);
			this.pageItem_Axis.Controls.Add(this.checkManualAxis);
			this.pageItem_Axis.Controls.Add(this.label13);
			this.pageItem_Axis.Controls.Add(this.labBottomMarginP);
			this.pageItem_Axis.Controls.Add(this.txtBottomMargin);
			this.pageItem_Axis.Controls.Add(this.labBottomMargin);
			this.pageItem_Axis.Controls.Add(this.labTopMarginP);
			this.pageItem_Axis.Controls.Add(this.txtTopMargin);
			this.pageItem_Axis.Controls.Add(this.labTopMargin);
			this.pageItem_Axis.Controls.Add(this.checkMargin);
			this.pageItem_Axis.Controls.Add(this.label11);
			this.pageItem_Axis.Controls.Add(this.label12);
			this.pageItem_Axis.Controls.Add(this.listAxisRange);
			this.pageItem_Axis.Controls.Add(this.label10);
			this.pageItem_Axis.Controls.Add(this.labAxisRange);
			this.pageItem_Axis.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Axis.Name = "pageItem_Axis";
			this.pageItem_Axis.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Axis.Size = new System.Drawing.Size(380, 371);
			this.pageItem_Axis.TabIndex = 3;
			this.pageItem_Axis.Text = "座標";
			this.pageItem_Axis.UseVisualStyleBackColor = true;
			// 
			// txtScaleCount
			// 
			this.txtScaleCount.Enabled = false;
			this.txtScaleCount.Location = new System.Drawing.Point(166, 224);
			this.txtScaleCount.Name = "txtScaleCount";
			this.txtScaleCount.Size = new System.Drawing.Size(70, 22);
			this.txtScaleCount.TabIndex = 22;
			this.txtScaleCount.Text = "10";
			// 
			// radioScaleCount
			// 
			this.radioScaleCount.AutoSize = true;
			this.radioScaleCount.Enabled = false;
			this.radioScaleCount.Location = new System.Drawing.Point(31, 227);
			this.radioScaleCount.Name = "radioScaleCount";
			this.radioScaleCount.Size = new System.Drawing.Size(95, 16);
			this.radioScaleCount.TabIndex = 21;
			this.radioScaleCount.TabStop = true;
			this.radioScaleCount.Text = "設定刻度數量";
			this.radioScaleCount.UseVisualStyleBackColor = true;
			// 
			// txtScaleGap
			// 
			this.txtScaleGap.Enabled = false;
			this.txtScaleGap.Location = new System.Drawing.Point(166, 196);
			this.txtScaleGap.Name = "txtScaleGap";
			this.txtScaleGap.Size = new System.Drawing.Size(70, 22);
			this.txtScaleGap.TabIndex = 20;
			this.txtScaleGap.Text = "0.01";
			// 
			// radioScaleGap
			// 
			this.radioScaleGap.AutoSize = true;
			this.radioScaleGap.Enabled = false;
			this.radioScaleGap.Location = new System.Drawing.Point(31, 199);
			this.radioScaleGap.Name = "radioScaleGap";
			this.radioScaleGap.Size = new System.Drawing.Size(95, 16);
			this.radioScaleGap.TabIndex = 19;
			this.radioScaleGap.TabStop = true;
			this.radioScaleGap.Text = "設定刻度間隔";
			this.radioScaleGap.UseVisualStyleBackColor = true;
			// 
			// checkManualAxis
			// 
			this.checkManualAxis.AutoSize = true;
			this.checkManualAxis.Location = new System.Drawing.Point(16, 174);
			this.checkManualAxis.Name = "checkManualAxis";
			this.checkManualAxis.Size = new System.Drawing.Size(99, 16);
			this.checkManualAxis.TabIndex = 18;
			this.checkManualAxis.Text = "手動刻度設定:";
			this.checkManualAxis.UseVisualStyleBackColor = true;
			this.checkManualAxis.CheckedChanged += new System.EventHandler(this.checkManualAxis_CheckedChanged);
			// 
			// label13
			// 
			this.label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label13.Location = new System.Drawing.Point(75, 181);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(284, 2);
			this.label13.TabIndex = 17;
			// 
			// labBottomMarginP
			// 
			this.labBottomMarginP.AutoSize = true;
			this.labBottomMarginP.Enabled = false;
			this.labBottomMarginP.Location = new System.Drawing.Point(293, 96);
			this.labBottomMarginP.Name = "labBottomMarginP";
			this.labBottomMarginP.Size = new System.Drawing.Size(14, 12);
			this.labBottomMarginP.TabIndex = 15;
			this.labBottomMarginP.Text = "%";
			// 
			// txtBottomMargin
			// 
			this.txtBottomMargin.Enabled = false;
			this.txtBottomMargin.Location = new System.Drawing.Point(217, 91);
			this.txtBottomMargin.Name = "txtBottomMargin";
			this.txtBottomMargin.Size = new System.Drawing.Size(70, 22);
			this.txtBottomMargin.TabIndex = 14;
			// 
			// labBottomMargin
			// 
			this.labBottomMargin.AutoSize = true;
			this.labBottomMargin.Enabled = false;
			this.labBottomMargin.Location = new System.Drawing.Point(145, 96);
			this.labBottomMargin.Name = "labBottomMargin";
			this.labBottomMargin.Size = new System.Drawing.Size(44, 12);
			this.labBottomMargin.TabIndex = 13;
			this.labBottomMargin.Text = "下邊界:";
			// 
			// labTopMarginP
			// 
			this.labTopMarginP.AutoSize = true;
			this.labTopMarginP.Enabled = false;
			this.labTopMarginP.Location = new System.Drawing.Point(293, 66);
			this.labTopMarginP.Name = "labTopMarginP";
			this.labTopMarginP.Size = new System.Drawing.Size(14, 12);
			this.labTopMarginP.TabIndex = 12;
			this.labTopMarginP.Text = "%";
			// 
			// txtTopMargin
			// 
			this.txtTopMargin.Enabled = false;
			this.txtTopMargin.Location = new System.Drawing.Point(217, 61);
			this.txtTopMargin.Name = "txtTopMargin";
			this.txtTopMargin.Size = new System.Drawing.Size(70, 22);
			this.txtTopMargin.TabIndex = 11;
			// 
			// labTopMargin
			// 
			this.labTopMargin.AutoSize = true;
			this.labTopMargin.Enabled = false;
			this.labTopMargin.Location = new System.Drawing.Point(145, 66);
			this.labTopMargin.Name = "labTopMargin";
			this.labTopMargin.Size = new System.Drawing.Size(44, 12);
			this.labTopMargin.TabIndex = 10;
			this.labTopMargin.Text = "上邊界:";
			// 
			// checkMargin
			// 
			this.checkMargin.AutoSize = true;
			this.checkMargin.Location = new System.Drawing.Point(137, 42);
			this.checkMargin.Name = "checkMargin";
			this.checkMargin.Size = new System.Drawing.Size(75, 16);
			this.checkMargin.TabIndex = 9;
			this.checkMargin.Text = "設定邊界:";
			this.checkMargin.UseVisualStyleBackColor = true;
			this.checkMargin.CheckedChanged += new System.EventHandler(this.checkMargin_CheckedChanged);
			// 
			// label11
			// 
			this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label11.Location = new System.Drawing.Point(217, 49);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(120, 2);
			this.label11.TabIndex = 8;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(164, 42);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(0, 12);
			this.label12.TabIndex = 7;
			// 
			// listAxisRange
			// 
			this.listAxisRange.FormattingEnabled = true;
			this.listAxisRange.ItemHeight = 12;
			this.listAxisRange.Items.AddRange(new object[] {
            "目前畫面",
            "全部數列",
            "變動大小",
            "價格區間"});
			this.listAxisRange.Location = new System.Drawing.Point(16, 42);
			this.listAxisRange.Name = "listAxisRange";
			this.listAxisRange.Size = new System.Drawing.Size(107, 112);
			this.listAxisRange.TabIndex = 6;
			// 
			// label10
			// 
			this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label10.Location = new System.Drawing.Point(75, 22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(284, 2);
			this.label10.TabIndex = 5;
			// 
			// labAxisRange
			// 
			this.labAxisRange.AutoSize = true;
			this.labAxisRange.Location = new System.Drawing.Point(14, 17);
			this.labAxisRange.Name = "labAxisRange";
			this.labAxisRange.Size = new System.Drawing.Size(56, 12);
			this.labAxisRange.TabIndex = 4;
			this.labAxisRange.Text = "座標範圍:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(230, 412);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(313, 412);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// comboUpWidth
			// 
			this.comboUpWidth.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboUpWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUpWidth.FormattingEnabled = true;
			this.comboUpWidth.Location = new System.Drawing.Point(237, 191);
			this.comboUpWidth.Name = "comboUpWidth";
			this.comboUpWidth.Size = new System.Drawing.Size(89, 23);
			this.comboUpWidth.TabIndex = 18;
			// 
			// comboDownWidth
			// 
			this.comboDownWidth.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboDownWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDownWidth.FormattingEnabled = true;
			this.comboDownWidth.Location = new System.Drawing.Point(237, 220);
			this.comboDownWidth.Name = "comboDownWidth";
			this.comboDownWidth.Size = new System.Drawing.Size(89, 23);
			this.comboDownWidth.TabIndex = 19;
			// 
			// comboLineWidth
			// 
			this.comboLineWidth.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboLineWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLineWidth.FormattingEnabled = true;
			this.comboLineWidth.Location = new System.Drawing.Point(237, 249);
			this.comboLineWidth.Name = "comboLineWidth";
			this.comboLineWidth.Size = new System.Drawing.Size(89, 23);
			this.comboLineWidth.TabIndex = 20;
			// 
			// frmCreateScriptSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 448);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabControl_Main);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmCreateScriptSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "商品設置";
			this.Load += new System.EventHandler(this.frmCreateScriptSetting_Load);
			this.tabControl_Main.ResumeLayout(false);
			this.pageItem_Product.ResumeLayout(false);
			this.pageItem_Product.PerformLayout();
			this.tabControl_Products.ResumeLayout(false);
			this.pageItem_All.ResumeLayout(false);
			this.pageItem_Settings.ResumeLayout(false);
			this.pageItem_Settings.PerformLayout();
			this.pageItem_Template.ResumeLayout(false);
			this.pageItem_Template.PerformLayout();
			this.groupExtend.ResumeLayout(false);
			this.groupExtend.PerformLayout();
			this.pageItem_Axis.ResumeLayout(false);
			this.pageItem_Axis.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid dataGrid;
		private System.Windows.Forms.TabControl tabControl_Main;
		private System.Windows.Forms.TabPage pageItem_Product;
		private System.Windows.Forms.TabPage pageItem_Settings;
		private System.Windows.Forms.TabControl tabControl_Products;
		private System.Windows.Forms.TabPage pageItem_All;
		private System.Windows.Forms.ComboBox comboDataSources;
		private System.Windows.Forms.Label labDataSource;
		private System.Windows.Forms.ComboBox comboProduct;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox comboResolution;
		private System.Windows.Forms.TextBox txtPeriod;
		private System.Windows.Forms.Label labPeriods;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labPeriodType;
		private System.Windows.Forms.ComboBox comboRequestMode;
		private System.Windows.Forms.TextBox txtCount;
		private System.Windows.Forms.RadioButton radioRange2;
		private System.Windows.Forms.RadioButton radioRange1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labDataRange;
		private System.Windows.Forms.DateTimePicker dtEndDate;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.DateTimePicker dtStartDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dtTODate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labShow;
		private System.Windows.Forms.ComboBox comboDataStream;
		private System.Windows.Forms.Label labDataStream;
		private System.Windows.Forms.ComboBox comboTimeZone;
		private System.Windows.Forms.Label labＴimeZone;
		private System.Windows.Forms.ListBox listSubChart;
		private System.Windows.Forms.Label labSubChart;
		private System.Windows.Forms.TabPage pageItem_Template;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labChartType;
		private System.Windows.Forms.GroupBox groupExtend;
		private System.Windows.Forms.CheckBox checkShowNewPrice;
		private System.Windows.Forms.ListBox listChartType;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label labTemplate;
		private System.Windows.Forms.Label labDown;
		private DevAge.Windows.Forms.ColorPicker colorDown;
		private System.Windows.Forms.Label labUp;
		private DevAge.Windows.Forms.ColorPicker colorUp;
		private System.Windows.Forms.Label labLine;
		private DevAge.Windows.Forms.ColorPicker colorLine;
		private System.Windows.Forms.TabPage pageItem_Axis;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labAxisRange;
		private System.Windows.Forms.ListBox listAxisRange;
		private System.Windows.Forms.Label labBottomMarginP;
		private System.Windows.Forms.TextBox txtBottomMargin;
		private System.Windows.Forms.Label labBottomMargin;
		private System.Windows.Forms.Label labTopMarginP;
		private System.Windows.Forms.TextBox txtTopMargin;
		private System.Windows.Forms.Label labTopMargin;
		private System.Windows.Forms.CheckBox checkMargin;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtScaleCount;
		private System.Windows.Forms.RadioButton radioScaleCount;
		private System.Windows.Forms.TextBox txtScaleGap;
		private System.Windows.Forms.RadioButton radioScaleGap;
		private System.Windows.Forms.CheckBox checkManualAxis;
		private System.Windows.Forms.Label label13;
		private Controls.LineWidthComboBox comboLineWidth;
		private Controls.LineWidthComboBox comboDownWidth;
		private Controls.LineWidthComboBox comboUpWidth;
	}
}