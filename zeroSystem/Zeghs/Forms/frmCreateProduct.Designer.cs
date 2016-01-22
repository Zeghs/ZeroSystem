namespace Zeghs.Forms {
	partial class frmCreateProduct {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.txtCommodity = new System.Windows.Forms.TextBox();
			this.txtProductId = new System.Windows.Forms.TextBox();
			this.comboDataSource = new System.Windows.Forms.ComboBox();
			this.labCommodity = new System.Windows.Forms.Label();
			this.labProductId = new System.Windows.Forms.Label();
			this.labDataSource = new System.Windows.Forms.Label();
			this.comboExchange = new System.Windows.Forms.ComboBox();
			this.labExchange = new System.Windows.Forms.Label();
			this.labProductName = new System.Windows.Forms.Label();
			this.txtProductName = new System.Windows.Forms.TextBox();
			this.comboCategory = new System.Windows.Forms.ComboBox();
			this.labCategory = new System.Windows.Forms.Label();
			this.labLine = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtCommodity
			// 
			this.txtCommodity.Location = new System.Drawing.Point(73, 156);
			this.txtCommodity.Name = "txtCommodity";
			this.txtCommodity.Size = new System.Drawing.Size(290, 22);
			this.txtCommodity.TabIndex = 6;
			// 
			// txtProductId
			// 
			this.txtProductId.Location = new System.Drawing.Point(73, 95);
			this.txtProductId.Name = "txtProductId";
			this.txtProductId.Size = new System.Drawing.Size(290, 22);
			this.txtProductId.TabIndex = 4;
			// 
			// comboDataSource
			// 
			this.comboDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDataSource.FormattingEnabled = true;
			this.comboDataSource.Items.AddRange(new object[] {
            ""});
			this.comboDataSource.Location = new System.Drawing.Point(73, 41);
			this.comboDataSource.Name = "comboDataSource";
			this.comboDataSource.Size = new System.Drawing.Size(289, 20);
			this.comboDataSource.TabIndex = 2;
			// 
			// labCommodity
			// 
			this.labCommodity.AutoSize = true;
			this.labCommodity.Location = new System.Drawing.Point(13, 161);
			this.labCommodity.Name = "labCommodity";
			this.labCommodity.Size = new System.Drawing.Size(53, 12);
			this.labCommodity.TabIndex = 10;
			this.labCommodity.Text = "商品來源";
			// 
			// labProductId
			// 
			this.labProductId.AutoSize = true;
			this.labProductId.Location = new System.Drawing.Point(13, 99);
			this.labProductId.Name = "labProductId";
			this.labProductId.Size = new System.Drawing.Size(53, 12);
			this.labProductId.TabIndex = 9;
			this.labProductId.Text = "商品代號";
			// 
			// labDataSource
			// 
			this.labDataSource.AutoSize = true;
			this.labDataSource.Location = new System.Drawing.Point(13, 45);
			this.labDataSource.Name = "labDataSource";
			this.labDataSource.Size = new System.Drawing.Size(53, 12);
			this.labDataSource.TabIndex = 8;
			this.labDataSource.Text = "資料來源";
			// 
			// comboExchange
			// 
			this.comboExchange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboExchange.Location = new System.Drawing.Point(73, 14);
			this.comboExchange.Name = "comboExchange";
			this.comboExchange.Size = new System.Drawing.Size(289, 20);
			this.comboExchange.TabIndex = 1;
			// 
			// labExchange
			// 
			this.labExchange.AutoSize = true;
			this.labExchange.Location = new System.Drawing.Point(25, 18);
			this.labExchange.Name = "labExchange";
			this.labExchange.Size = new System.Drawing.Size(41, 12);
			this.labExchange.TabIndex = 16;
			this.labExchange.Text = "交易所";
			// 
			// labProductName
			// 
			this.labProductName.AutoSize = true;
			this.labProductName.Location = new System.Drawing.Point(13, 130);
			this.labProductName.Name = "labProductName";
			this.labProductName.Size = new System.Drawing.Size(53, 12);
			this.labProductName.TabIndex = 18;
			this.labProductName.Text = "商品名稱";
			// 
			// txtProductName
			// 
			this.txtProductName.Location = new System.Drawing.Point(72, 125);
			this.txtProductName.Name = "txtProductName";
			this.txtProductName.Size = new System.Drawing.Size(290, 22);
			this.txtProductName.TabIndex = 5;
			// 
			// comboCategory
			// 
			this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCategory.Location = new System.Drawing.Point(73, 68);
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(289, 20);
			this.comboCategory.TabIndex = 3;
			// 
			// labCategory
			// 
			this.labCategory.AutoSize = true;
			this.labCategory.Location = new System.Drawing.Point(13, 72);
			this.labCategory.Name = "labCategory";
			this.labCategory.Size = new System.Drawing.Size(53, 12);
			this.labCategory.TabIndex = 20;
			this.labCategory.Text = "商品類型";
			// 
			// labLine
			// 
			this.labLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labLine.Location = new System.Drawing.Point(11, 186);
			this.labLine.Name = "labLine";
			this.labLine.Size = new System.Drawing.Size(355, 2);
			this.labLine.TabIndex = 22;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(206, 196);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 7;
			this.btnOk.Text = "確定";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(287, 196);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmCreateProduct
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(376, 229);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.labLine);
			this.Controls.Add(this.comboCategory);
			this.Controls.Add(this.labCategory);
			this.Controls.Add(this.txtProductName);
			this.Controls.Add(this.labProductName);
			this.Controls.Add(this.comboExchange);
			this.Controls.Add(this.labExchange);
			this.Controls.Add(this.txtCommodity);
			this.Controls.Add(this.txtProductId);
			this.Controls.Add(this.comboDataSource);
			this.Controls.Add(this.labCommodity);
			this.Controls.Add(this.labProductId);
			this.Controls.Add(this.labDataSource);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmCreateProduct";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "建立商品資訊";
			this.Load += new System.EventHandler(this.frmCreateProduct_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtCommodity;
		private System.Windows.Forms.TextBox txtProductId;
		private System.Windows.Forms.ComboBox comboDataSource;
		private System.Windows.Forms.Label labCommodity;
		private System.Windows.Forms.Label labProductId;
		private System.Windows.Forms.Label labDataSource;
		private System.Windows.Forms.ComboBox comboExchange;
		private System.Windows.Forms.Label labExchange;
		private System.Windows.Forms.Label labProductName;
		private System.Windows.Forms.TextBox txtProductName;
		private System.Windows.Forms.ComboBox comboCategory;
		private System.Windows.Forms.Label labCategory;
		private System.Windows.Forms.Label labLine;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}