namespace Zeghs.Forms {
	partial class frmProductPropertySettings {
		private static readonly string __sMessageHeader_001 = "變更基礎設定";
		private static readonly string __sMessageContent_001 = "您是否要變更交易所商品的基礎設定值?";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Zeghs.Data.SimpleBoundList<Zeghs.Products.CautionMoney> sourceCautions = null;
		private Zeghs.Data.SimpleBoundList< PowerLanguage.SessionObject> sourceSessions = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeGridCaution() {
			SourceGrid.Cells.Views.IView cTextAlignView = new SourceGrid.Cells.Views.Cell();
			cTextAlignView.TextAlignment = DevAge.Drawing.ContentAlignment.TopRight;

			this.gridCaution.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.gridCaution.Columns.Add("Description", "描述", typeof(string));
			this.gridCaution.Columns.Add("InitialMoney", "初始保證金", typeof(double));
			this.gridCaution.Columns.Add("KeepMoney", "維持保證金", typeof(double));
			this.gridCaution.Columns.Add("CloseMoney", "結算保證金", typeof(double));
			this.gridCaution.Columns[0].Width = 70;
			this.gridCaution.Columns[1].Width = 75;
			this.gridCaution.Columns[1].DataCell.View = cTextAlignView;
			this.gridCaution.Columns[2].Width = 75;
			this.gridCaution.Columns[2].DataCell.View = cTextAlignView;
			this.gridCaution.Columns[3].Width = 75;
			this.gridCaution.Columns[3].DataCell.View = cTextAlignView;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.gridCaution.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));
		}

		private void InitializeGridSession() {
			this.gridSession.Rows.RowHeight = 21;  //處理第一列中文字體會被遮住的問題
			this.gridSession.Columns.Add("StartDay", "開盤日", typeof(System.DayOfWeek));
			this.gridSession.Columns.Add("StartTime", "開盤時間", typeof(System.TimeSpan));
			this.gridSession.Columns.Add("EndDay", "收盤日", typeof(System.DayOfWeek));
			this.gridSession.Columns.Add("EndTime", "收盤時間", typeof(System.TimeSpan));
			this.gridSession.Columns[0].Width = 75;
			this.gridSession.Columns[1].Width = 71;
			this.gridSession.Columns[2].Width = 75;
			this.gridSession.Columns[3].Width = 71;

			//修改選擇條的框線寬度與顏色
			SourceGrid.Selection.SelectionBase cSelectionBase = this.gridSession.Selection as SourceGrid.Selection.SelectionBase;
			cSelectionBase.Border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(cSelectionBase.BackColor, 1));
			this.gridSession.Selection.SelectionChanged += gridSession_onSelectionChanged;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.gridCaution = new SourceGrid.DataGrid();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.pageItem_Normal = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnCautionDelete = new System.Windows.Forms.Button();
			this.btnCautionCreate = new System.Windows.Forms.Button();
			this.txtDailyLimit = new System.Windows.Forms.TextBox();
			this.labDayLimit = new System.Windows.Forms.Label();
			this.txtBigPointValue = new System.Windows.Forms.TextBox();
			this.labBigPointValue = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.txtCommodity = new System.Windows.Forms.TextBox();
			this.txtProduct = new System.Windows.Forms.TextBox();
			this.comboDataSource = new System.Windows.Forms.ComboBox();
			this.labMemo = new System.Windows.Forms.Label();
			this.labCommodity = new System.Windows.Forms.Label();
			this.labProduct = new System.Windows.Forms.Label();
			this.labDataSource = new System.Windows.Forms.Label();
			this.pageItem_Rule = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.btnTaxRule = new System.Windows.Forms.Button();
			this.txtTaxRule = new System.Windows.Forms.TextBox();
			this.labTaxRule = new System.Windows.Forms.Label();
			this.btnPriceScaleRule = new System.Windows.Forms.Button();
			this.txtPriceScaleRule = new System.Windows.Forms.TextBox();
			this.labPriceScaleRule = new System.Windows.Forms.Label();
			this.btnContractTimeRule = new System.Windows.Forms.Button();
			this.txtContractTimeRule = new System.Windows.Forms.TextBox();
			this.labContractTimeRule = new System.Windows.Forms.Label();
			this.pageItem_Session = new System.Windows.Forms.TabPage();
			this.groupDaylight = new System.Windows.Forms.GroupBox();
			this.checkDaylight = new System.Windows.Forms.CheckBox();
			this.pickerDaylightEnd = new System.Windows.Forms.DateTimePicker();
			this.labDaylightEnd = new System.Windows.Forms.Label();
			this.pickerDaylightStart = new System.Windows.Forms.DateTimePicker();
			this.labDaylightStart = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnSessionDelete = new System.Windows.Forms.Button();
			this.btnSessionCreate = new System.Windows.Forms.Button();
			this.gridSession = new SourceGrid.DataGrid();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.pageItem_Normal.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.pageItem_Rule.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.pageItem_Session.SuspendLayout();
			this.groupDaylight.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridCaution
			// 
			this.gridCaution.BackColor = System.Drawing.SystemColors.Window;
			this.gridCaution.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridCaution.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.gridCaution.EnableSort = false;
			this.gridCaution.FixedRows = 1;
			this.gridCaution.Location = new System.Drawing.Point(9, 83);
			this.gridCaution.Name = "gridCaution";
			this.gridCaution.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.gridCaution.Size = new System.Drawing.Size(317, 88);
			this.gridCaution.TabIndex = 3;
			this.gridCaution.TabStop = true;
			this.gridCaution.ToolTipText = "";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.pageItem_Normal);
			this.tabControl.Controls.Add(this.pageItem_Rule);
			this.tabControl.Controls.Add(this.pageItem_Session);
			this.tabControl.Location = new System.Drawing.Point(6, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(361, 415);
			this.tabControl.TabIndex = 0;
			// 
			// pageItem_Normal
			// 
			this.pageItem_Normal.Controls.Add(this.groupBox2);
			this.pageItem_Normal.Controls.Add(this.groupBox1);
			this.pageItem_Normal.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Normal.Name = "pageItem_Normal";
			this.pageItem_Normal.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Normal.Size = new System.Drawing.Size(353, 390);
			this.pageItem_Normal.TabIndex = 0;
			this.pageItem_Normal.Text = "一般";
			this.pageItem_Normal.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnCautionDelete);
			this.groupBox2.Controls.Add(this.btnCautionCreate);
			this.groupBox2.Controls.Add(this.gridCaution);
			this.groupBox2.Controls.Add(this.txtDailyLimit);
			this.groupBox2.Controls.Add(this.labDayLimit);
			this.groupBox2.Controls.Add(this.txtBigPointValue);
			this.groupBox2.Controls.Add(this.labBigPointValue);
			this.groupBox2.Location = new System.Drawing.Point(9, 176);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(334, 205);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "其他設定";
			// 
			// btnCautionDelete
			// 
			this.btnCautionDelete.Location = new System.Drawing.Point(72, 173);
			this.btnCautionDelete.Name = "btnCautionDelete";
			this.btnCautionDelete.Size = new System.Drawing.Size(61, 23);
			this.btnCautionDelete.TabIndex = 5;
			this.btnCautionDelete.Text = "刪除";
			this.btnCautionDelete.UseVisualStyleBackColor = true;
			this.btnCautionDelete.Click += new System.EventHandler(this.btnCautionDelete_Click);
			// 
			// btnCautionCreate
			// 
			this.btnCautionCreate.Location = new System.Drawing.Point(9, 173);
			this.btnCautionCreate.Name = "btnCautionCreate";
			this.btnCautionCreate.Size = new System.Drawing.Size(61, 23);
			this.btnCautionCreate.TabIndex = 4;
			this.btnCautionCreate.Text = " 新增";
			this.btnCautionCreate.UseVisualStyleBackColor = true;
			this.btnCautionCreate.Click += new System.EventHandler(this.btnCautionCreate_Click);
			// 
			// txtDailyLimit
			// 
			this.txtDailyLimit.Location = new System.Drawing.Point(111, 51);
			this.txtDailyLimit.Name = "txtDailyLimit";
			this.txtDailyLimit.Size = new System.Drawing.Size(92, 22);
			this.txtDailyLimit.TabIndex = 3;
			this.txtDailyLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labDayLimit
			// 
			this.labDayLimit.AutoSize = true;
			this.labDayLimit.Location = new System.Drawing.Point(16, 56);
			this.labDayLimit.Name = "labDayLimit";
			this.labDayLimit.Size = new System.Drawing.Size(89, 12);
			this.labDayLimit.TabIndex = 2;
			this.labDayLimit.Text = "當日漲跌幅限制";
			// 
			// txtBigPointValue
			// 
			this.txtBigPointValue.Location = new System.Drawing.Point(111, 21);
			this.txtBigPointValue.Name = "txtBigPointValue";
			this.txtBigPointValue.Size = new System.Drawing.Size(92, 22);
			this.txtBigPointValue.TabIndex = 1;
			this.txtBigPointValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labBigPointValue
			// 
			this.labBigPointValue.AutoSize = true;
			this.labBigPointValue.Location = new System.Drawing.Point(16, 26);
			this.labBigPointValue.Name = "labBigPointValue";
			this.labBigPointValue.Size = new System.Drawing.Size(89, 12);
			this.labBigPointValue.TabIndex = 0;
			this.labBigPointValue.Text = "每跳動一點金額";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtDescription);
			this.groupBox1.Controls.Add(this.txtCommodity);
			this.groupBox1.Controls.Add(this.txtProduct);
			this.groupBox1.Controls.Add(this.comboDataSource);
			this.groupBox1.Controls.Add(this.labMemo);
			this.groupBox1.Controls.Add(this.labCommodity);
			this.groupBox1.Controls.Add(this.labProduct);
			this.groupBox1.Controls.Add(this.labDataSource);
			this.groupBox1.Location = new System.Drawing.Point(9, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(334, 144);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "商品設定";
			// 
			// txtDescription
			// 
			this.txtDescription.Location = new System.Drawing.Point(76, 108);
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size(250, 22);
			this.txtDescription.TabIndex = 7;
			// 
			// txtCommodity
			// 
			this.txtCommodity.Location = new System.Drawing.Point(76, 78);
			this.txtCommodity.Name = "txtCommodity";
			this.txtCommodity.ReadOnly = true;
			this.txtCommodity.Size = new System.Drawing.Size(154, 22);
			this.txtCommodity.TabIndex = 6;
			// 
			// txtProduct
			// 
			this.txtProduct.Location = new System.Drawing.Point(76, 49);
			this.txtProduct.Name = "txtProduct";
			this.txtProduct.ReadOnly = true;
			this.txtProduct.Size = new System.Drawing.Size(154, 22);
			this.txtProduct.TabIndex = 5;
			// 
			// comboDataSource
			// 
			this.comboDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDataSource.FormattingEnabled = true;
			this.comboDataSource.Items.AddRange(new object[] {
            ""});
			this.comboDataSource.Location = new System.Drawing.Point(76, 22);
			this.comboDataSource.Name = "comboDataSource";
			this.comboDataSource.Size = new System.Drawing.Size(154, 20);
			this.comboDataSource.TabIndex = 4;
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Location = new System.Drawing.Point(16, 113);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(53, 12);
			this.labMemo.TabIndex = 3;
			this.labMemo.Text = "商品描述";
			// 
			// labCommodity
			// 
			this.labCommodity.AutoSize = true;
			this.labCommodity.Location = new System.Drawing.Point(16, 83);
			this.labCommodity.Name = "labCommodity";
			this.labCommodity.Size = new System.Drawing.Size(53, 12);
			this.labCommodity.TabIndex = 2;
			this.labCommodity.Text = "商品來源";
			// 
			// labProduct
			// 
			this.labProduct.AutoSize = true;
			this.labProduct.Location = new System.Drawing.Point(16, 53);
			this.labProduct.Name = "labProduct";
			this.labProduct.Size = new System.Drawing.Size(53, 12);
			this.labProduct.TabIndex = 1;
			this.labProduct.Text = "商品名稱";
			// 
			// labDataSource
			// 
			this.labDataSource.AutoSize = true;
			this.labDataSource.Location = new System.Drawing.Point(16, 26);
			this.labDataSource.Name = "labDataSource";
			this.labDataSource.Size = new System.Drawing.Size(53, 12);
			this.labDataSource.TabIndex = 0;
			this.labDataSource.Text = "資料來源";
			// 
			// pageItem_Rule
			// 
			this.pageItem_Rule.Controls.Add(this.groupBox4);
			this.pageItem_Rule.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Rule.Name = "pageItem_Rule";
			this.pageItem_Rule.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Rule.Size = new System.Drawing.Size(353, 390);
			this.pageItem_Rule.TabIndex = 2;
			this.pageItem_Rule.Text = "規則";
			this.pageItem_Rule.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.btnTaxRule);
			this.groupBox4.Controls.Add(this.txtTaxRule);
			this.groupBox4.Controls.Add(this.labTaxRule);
			this.groupBox4.Controls.Add(this.btnPriceScaleRule);
			this.groupBox4.Controls.Add(this.txtPriceScaleRule);
			this.groupBox4.Controls.Add(this.labPriceScaleRule);
			this.groupBox4.Controls.Add(this.btnContractTimeRule);
			this.groupBox4.Controls.Add(this.txtContractTimeRule);
			this.groupBox4.Controls.Add(this.labContractTimeRule);
			this.groupBox4.Location = new System.Drawing.Point(9, 15);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(335, 247);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "規則設定";
			// 
			// btnTaxRule
			// 
			this.btnTaxRule.Location = new System.Drawing.Point(295, 185);
			this.btnTaxRule.Name = "btnTaxRule";
			this.btnTaxRule.Size = new System.Drawing.Size(24, 24);
			this.btnTaxRule.TabIndex = 8;
			this.btnTaxRule.Text = "...";
			this.btnTaxRule.UseVisualStyleBackColor = true;
			this.btnTaxRule.Click += new System.EventHandler(this.RuleButton_onClick);
			// 
			// txtTaxRule
			// 
			this.txtTaxRule.Location = new System.Drawing.Point(16, 186);
			this.txtTaxRule.Name = "txtTaxRule";
			this.txtTaxRule.ReadOnly = true;
			this.txtTaxRule.Size = new System.Drawing.Size(278, 22);
			this.txtTaxRule.TabIndex = 7;
			// 
			// labTaxRule
			// 
			this.labTaxRule.AutoSize = true;
			this.labTaxRule.Location = new System.Drawing.Point(14, 167);
			this.labTaxRule.Name = "labTaxRule";
			this.labTaxRule.Size = new System.Drawing.Size(101, 12);
			this.labTaxRule.TabIndex = 6;
			this.labTaxRule.Text = "商品交易稅率規則";
			// 
			// btnPriceScaleRule
			// 
			this.btnPriceScaleRule.Location = new System.Drawing.Point(295, 119);
			this.btnPriceScaleRule.Name = "btnPriceScaleRule";
			this.btnPriceScaleRule.Size = new System.Drawing.Size(24, 24);
			this.btnPriceScaleRule.TabIndex = 5;
			this.btnPriceScaleRule.Text = "...";
			this.btnPriceScaleRule.UseVisualStyleBackColor = true;
			this.btnPriceScaleRule.Click += new System.EventHandler(this.RuleButton_onClick);
			// 
			// txtPriceScaleRule
			// 
			this.txtPriceScaleRule.Location = new System.Drawing.Point(16, 120);
			this.txtPriceScaleRule.Name = "txtPriceScaleRule";
			this.txtPriceScaleRule.ReadOnly = true;
			this.txtPriceScaleRule.Size = new System.Drawing.Size(278, 22);
			this.txtPriceScaleRule.TabIndex = 4;
			// 
			// labPriceScaleRule
			// 
			this.labPriceScaleRule.AutoSize = true;
			this.labPriceScaleRule.Location = new System.Drawing.Point(14, 101);
			this.labPriceScaleRule.Name = "labPriceScaleRule";
			this.labPriceScaleRule.Size = new System.Drawing.Size(101, 12);
			this.labPriceScaleRule.TabIndex = 3;
			this.labPriceScaleRule.Text = "商品價格座標規則";
			// 
			// btnContractTimeRule
			// 
			this.btnContractTimeRule.Location = new System.Drawing.Point(295, 53);
			this.btnContractTimeRule.Name = "btnContractTimeRule";
			this.btnContractTimeRule.Size = new System.Drawing.Size(24, 24);
			this.btnContractTimeRule.TabIndex = 2;
			this.btnContractTimeRule.Text = "...";
			this.btnContractTimeRule.UseVisualStyleBackColor = true;
			this.btnContractTimeRule.Click += new System.EventHandler(this.RuleButton_onClick);
			// 
			// txtContractTimeRule
			// 
			this.txtContractTimeRule.Location = new System.Drawing.Point(16, 54);
			this.txtContractTimeRule.Name = "txtContractTimeRule";
			this.txtContractTimeRule.ReadOnly = true;
			this.txtContractTimeRule.Size = new System.Drawing.Size(278, 22);
			this.txtContractTimeRule.TabIndex = 1;
			// 
			// labContractTimeRule
			// 
			this.labContractTimeRule.AutoSize = true;
			this.labContractTimeRule.Location = new System.Drawing.Point(14, 35);
			this.labContractTimeRule.Name = "labContractTimeRule";
			this.labContractTimeRule.Size = new System.Drawing.Size(101, 12);
			this.labContractTimeRule.TabIndex = 0;
			this.labContractTimeRule.Text = "商品合約到期規則";
			// 
			// pageItem_Session
			// 
			this.pageItem_Session.Controls.Add(this.groupDaylight);
			this.pageItem_Session.Controls.Add(this.groupBox3);
			this.pageItem_Session.Location = new System.Drawing.Point(4, 21);
			this.pageItem_Session.Name = "pageItem_Session";
			this.pageItem_Session.Padding = new System.Windows.Forms.Padding(3);
			this.pageItem_Session.Size = new System.Drawing.Size(353, 390);
			this.pageItem_Session.TabIndex = 1;
			this.pageItem_Session.Text = "交易時段";
			this.pageItem_Session.UseVisualStyleBackColor = true;
			// 
			// groupDaylight
			// 
			this.groupDaylight.Controls.Add(this.checkDaylight);
			this.groupDaylight.Controls.Add(this.pickerDaylightEnd);
			this.groupDaylight.Controls.Add(this.labDaylightEnd);
			this.groupDaylight.Controls.Add(this.pickerDaylightStart);
			this.groupDaylight.Controls.Add(this.labDaylightStart);
			this.groupDaylight.Location = new System.Drawing.Point(10, 261);
			this.groupDaylight.Name = "groupDaylight";
			this.groupDaylight.Size = new System.Drawing.Size(332, 117);
			this.groupDaylight.TabIndex = 1;
			this.groupDaylight.TabStop = false;
			this.groupDaylight.Text = "日光節約( 請使用標準全球時區 UTC 格式 )";
			// 
			// checkDaylight
			// 
			this.checkDaylight.AutoSize = true;
			this.checkDaylight.Location = new System.Drawing.Point(117, 89);
			this.checkDaylight.Name = "checkDaylight";
			this.checkDaylight.Size = new System.Drawing.Size(144, 16);
			this.checkDaylight.TabIndex = 4;
			this.checkDaylight.Text = "啟用日光節約設定設定";
			this.checkDaylight.UseVisualStyleBackColor = true;
			this.checkDaylight.CheckedChanged += new System.EventHandler(this.checkDaylight_CheckedChanged);
			// 
			// pickerDaylightEnd
			// 
			this.pickerDaylightEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
			this.pickerDaylightEnd.Enabled = false;
			this.pickerDaylightEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.pickerDaylightEnd.Location = new System.Drawing.Point(117, 56);
			this.pickerDaylightEnd.Name = "pickerDaylightEnd";
			this.pickerDaylightEnd.Size = new System.Drawing.Size(138, 22);
			this.pickerDaylightEnd.TabIndex = 3;
			// 
			// labDaylightEnd
			// 
			this.labDaylightEnd.AutoSize = true;
			this.labDaylightEnd.Location = new System.Drawing.Point(22, 61);
			this.labDaylightEnd.Name = "labDaylightEnd";
			this.labDaylightEnd.Size = new System.Drawing.Size(89, 12);
			this.labDaylightEnd.TabIndex = 2;
			this.labDaylightEnd.Text = "夏令時間結束日";
			// 
			// pickerDaylightStart
			// 
			this.pickerDaylightStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
			this.pickerDaylightStart.Enabled = false;
			this.pickerDaylightStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.pickerDaylightStart.Location = new System.Drawing.Point(117, 25);
			this.pickerDaylightStart.Name = "pickerDaylightStart";
			this.pickerDaylightStart.Size = new System.Drawing.Size(138, 22);
			this.pickerDaylightStart.TabIndex = 1;
			// 
			// labDaylightStart
			// 
			this.labDaylightStart.AutoSize = true;
			this.labDaylightStart.Location = new System.Drawing.Point(22, 30);
			this.labDaylightStart.Name = "labDaylightStart";
			this.labDaylightStart.Size = new System.Drawing.Size(89, 12);
			this.labDaylightStart.TabIndex = 0;
			this.labDaylightStart.Text = "夏令時間開始日";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.btnSessionDelete);
			this.groupBox3.Controls.Add(this.btnSessionCreate);
			this.groupBox3.Controls.Add(this.gridSession);
			this.groupBox3.Location = new System.Drawing.Point(10, 14);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(332, 227);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "時段設定";
			// 
			// btnSessionDelete
			// 
			this.btnSessionDelete.Location = new System.Drawing.Point(71, 198);
			this.btnSessionDelete.Name = "btnSessionDelete";
			this.btnSessionDelete.Size = new System.Drawing.Size(61, 23);
			this.btnSessionDelete.TabIndex = 7;
			this.btnSessionDelete.Text = "刪除";
			this.btnSessionDelete.UseVisualStyleBackColor = true;
			this.btnSessionDelete.Click += new System.EventHandler(this.btnSessionDelete_Click);
			// 
			// btnSessionCreate
			// 
			this.btnSessionCreate.Location = new System.Drawing.Point(8, 198);
			this.btnSessionCreate.Name = "btnSessionCreate";
			this.btnSessionCreate.Size = new System.Drawing.Size(61, 23);
			this.btnSessionCreate.TabIndex = 6;
			this.btnSessionCreate.Text = " 新增";
			this.btnSessionCreate.UseVisualStyleBackColor = true;
			this.btnSessionCreate.Click += new System.EventHandler(this.btnSessionCreate_Click);
			// 
			// gridSession
			// 
			this.gridSession.BackColor = System.Drawing.SystemColors.Window;
			this.gridSession.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridSession.DeleteQuestionMessage = "您確定要刪除此筆資訊?";
			this.gridSession.EnableSort = false;
			this.gridSession.FixedRows = 1;
			this.gridSession.Location = new System.Drawing.Point(8, 23);
			this.gridSession.Name = "gridSession";
			this.gridSession.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this.gridSession.Size = new System.Drawing.Size(317, 171);
			this.gridSession.TabIndex = 4;
			this.gridSession.TabStop = true;
			this.gridSession.ToolTipText = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(208, 427);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(289, 427);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmProductPropertySettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(373, 460);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmProductPropertySettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "商品屬性設定";
			this.Load += new System.EventHandler(this.frmProductPropertySettings_Load);
			this.tabControl.ResumeLayout(false);
			this.pageItem_Normal.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.pageItem_Rule.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.pageItem_Session.ResumeLayout(false);
			this.groupDaylight.ResumeLayout(false);
			this.groupDaylight.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.DataGrid gridCaution;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage pageItem_Normal;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labCommodity;
		private System.Windows.Forms.Label labProduct;
		private System.Windows.Forms.Label labDataSource;
		private System.Windows.Forms.TabPage pageItem_Session;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.TextBox txtCommodity;
		private System.Windows.Forms.TextBox txtProduct;
		private System.Windows.Forms.ComboBox comboDataSource;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labBigPointValue;
		private System.Windows.Forms.TextBox txtBigPointValue;
		private System.Windows.Forms.TextBox txtDailyLimit;
		private System.Windows.Forms.Label labDayLimit;
		private System.Windows.Forms.Button btnCautionDelete;
		private System.Windows.Forms.Button btnCautionCreate;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox3;
		private SourceGrid.DataGrid gridSession;
		private System.Windows.Forms.Button btnSessionDelete;
		private System.Windows.Forms.Button btnSessionCreate;
		private System.Windows.Forms.GroupBox groupDaylight;
		private System.Windows.Forms.Label labDaylightStart;
		private System.Windows.Forms.DateTimePicker pickerDaylightStart;
		private System.Windows.Forms.DateTimePicker pickerDaylightEnd;
		private System.Windows.Forms.Label labDaylightEnd;
		private System.Windows.Forms.CheckBox checkDaylight;
		private System.Windows.Forms.TabPage pageItem_Rule;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label labContractTimeRule;
		private System.Windows.Forms.Button btnContractTimeRule;
		private System.Windows.Forms.TextBox txtContractTimeRule;
		private System.Windows.Forms.Button btnPriceScaleRule;
		private System.Windows.Forms.TextBox txtPriceScaleRule;
		private System.Windows.Forms.Label labPriceScaleRule;
		private System.Windows.Forms.Button btnTaxRule;
		private System.Windows.Forms.TextBox txtTaxRule;
		private System.Windows.Forms.Label labTaxRule;
	}
}