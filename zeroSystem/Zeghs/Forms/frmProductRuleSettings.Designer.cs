namespace Zeghs.Forms {
	partial class frmProductRuleSettings {
		private static readonly string __sMessageHeader_001 = "規則修改";
		private static readonly string __sMessageContent_001 = "您是否要在原規則上套用修改後的設定?";

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
			this.labName = new System.Windows.Forms.Label();
			this.labMemo = new System.Windows.Forms.Label();
			this.txtMemo = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.comboRules = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// labName
			// 
			this.labName.AutoSize = true;
			this.labName.Location = new System.Drawing.Point(12, 16);
			this.labName.Name = "labName";
			this.labName.Size = new System.Drawing.Size(53, 12);
			this.labName.TabIndex = 0;
			this.labName.Text = "規則名稱";
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Location = new System.Drawing.Point(12, 46);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(53, 12);
			this.labMemo.TabIndex = 1;
			this.labMemo.Text = "規則描述";
			// 
			// txtMemo
			// 
			this.txtMemo.Location = new System.Drawing.Point(71, 42);
			this.txtMemo.Multiline = true;
			this.txtMemo.Name = "txtMemo";
			this.txtMemo.Size = new System.Drawing.Size(259, 83);
			this.txtMemo.TabIndex = 3;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(174, 136);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(255, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// comboRules
			// 
			this.comboRules.FormattingEnabled = true;
			this.comboRules.Location = new System.Drawing.Point(72, 13);
			this.comboRules.Name = "comboRules";
			this.comboRules.Size = new System.Drawing.Size(258, 20);
			this.comboRules.TabIndex = 1;
			this.comboRules.SelectedIndexChanged += new System.EventHandler(this.comboRules_SelectedIndexChanged);
			// 
			// frmProductRuleSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(345, 175);
			this.Controls.Add(this.comboRules);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtMemo);
			this.Controls.Add(this.labMemo);
			this.Controls.Add(this.labName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmProductRuleSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "商品規則設定";
			this.Load += new System.EventHandler(this.frmProductRuleSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labName;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.TextBox txtMemo;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox comboRules;
	}
}