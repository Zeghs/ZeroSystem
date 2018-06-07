namespace Zeghs.Forms {
	partial class frmCommissionRuleSettings {
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
			this.comboRules = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtMemo = new System.Windows.Forms.TextBox();
			this.labMemo = new System.Windows.Forms.Label();
			this.labName = new System.Windows.Forms.Label();
			this.labType = new System.Windows.Forms.Label();
			this.comboType = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// comboRules
			// 
			this.comboRules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRules.FormattingEnabled = true;
			this.comboRules.Location = new System.Drawing.Point(73, 43);
			this.comboRules.Name = "comboRules";
			this.comboRules.Size = new System.Drawing.Size(258, 20);
			this.comboRules.TabIndex = 8;
			this.comboRules.SelectedIndexChanged += new System.EventHandler(this.comboRules_SelectedIndexChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(256, 166);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(175, 166);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtMemo
			// 
			this.txtMemo.Location = new System.Drawing.Point(72, 72);
			this.txtMemo.Multiline = true;
			this.txtMemo.Name = "txtMemo";
			this.txtMemo.Size = new System.Drawing.Size(259, 83);
			this.txtMemo.TabIndex = 9;
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Location = new System.Drawing.Point(13, 76);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(53, 12);
			this.labMemo.TabIndex = 7;
			this.labMemo.Text = "規則描述";
			// 
			// labName
			// 
			this.labName.AutoSize = true;
			this.labName.Location = new System.Drawing.Point(13, 46);
			this.labName.Name = "labName";
			this.labName.Size = new System.Drawing.Size(53, 12);
			this.labName.TabIndex = 6;
			this.labName.Text = "規則名稱";
			// 
			// labType
			// 
			this.labType.AutoSize = true;
			this.labType.Location = new System.Drawing.Point(13, 17);
			this.labType.Name = "labType";
			this.labType.Size = new System.Drawing.Size(53, 12);
			this.labType.TabIndex = 12;
			this.labType.Text = "規則類型";
			// 
			// comboType
			// 
			this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboType.FormattingEnabled = true;
			this.comboType.Items.AddRange(new object[] {
            "傭金(Commission)",
            "手續費(Fee)"});
			this.comboType.Location = new System.Drawing.Point(73, 14);
			this.comboType.Name = "comboType";
			this.comboType.Size = new System.Drawing.Size(114, 20);
			this.comboType.TabIndex = 7;
			this.comboType.SelectedIndexChanged += new System.EventHandler(this.comboType_SelectedIndexChanged);
			// 
			// frmCommissionRuleSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(345, 201);
			this.Controls.Add(this.comboType);
			this.Controls.Add(this.labType);
			this.Controls.Add(this.comboRules);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtMemo);
			this.Controls.Add(this.labMemo);
			this.Controls.Add(this.labName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmCommissionRuleSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "傭金/手續費 規則設定";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.frmCommissionRuleSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboRules;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox txtMemo;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.Label labName;
		private System.Windows.Forms.Label labType;
		private System.Windows.Forms.ComboBox comboType;
	}
}