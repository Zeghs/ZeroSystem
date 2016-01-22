namespace Taiwan.Forms {
	partial class frmTaiwanTaxSetting {
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
			this.groupTax = new System.Windows.Forms.GroupBox();
			this.labPercent = new System.Windows.Forms.Label();
			this.numericTax = new System.Windows.Forms.NumericUpDown();
			this.labText = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupTax.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericTax)).BeginInit();
			this.SuspendLayout();
			// 
			// groupTax
			// 
			this.groupTax.Controls.Add(this.labPercent);
			this.groupTax.Controls.Add(this.numericTax);
			this.groupTax.Controls.Add(this.labText);
			this.groupTax.Location = new System.Drawing.Point(11, 15);
			this.groupTax.Name = "groupTax";
			this.groupTax.Size = new System.Drawing.Size(211, 73);
			this.groupTax.TabIndex = 0;
			this.groupTax.TabStop = false;
			this.groupTax.Text = "交易稅率設定";
			// 
			// labPercent
			// 
			this.labPercent.AutoSize = true;
			this.labPercent.Location = new System.Drawing.Point(174, 33);
			this.labPercent.Name = "labPercent";
			this.labPercent.Size = new System.Drawing.Size(14, 12);
			this.labPercent.TabIndex = 2;
			this.labPercent.Text = "%";
			// 
			// numericTax
			// 
			this.numericTax.Location = new System.Drawing.Point(76, 28);
			this.numericTax.Name = "numericTax";
			this.numericTax.Size = new System.Drawing.Size(94, 22);
			this.numericTax.TabIndex = 1;
			this.numericTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labText
			// 
			this.labText.AutoSize = true;
			this.labText.Location = new System.Drawing.Point(17, 33);
			this.labText.Name = "labText";
			this.labText.Size = new System.Drawing.Size(53, 12);
			this.labText.TabIndex = 0;
			this.labText.Text = "交易稅率";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(120, 97);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(39, 97);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmTaiwanTaxSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(235, 139);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupTax);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTaiwanTaxSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "台灣交易稅規則參數設定";
			this.Load += new System.EventHandler(this.frmTaiwanTaxSetting_Load);
			this.groupTax.ResumeLayout(false);
			this.groupTax.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericTax)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupTax;
		private System.Windows.Forms.Label labPercent;
		private System.Windows.Forms.NumericUpDown numericTax;
		private System.Windows.Forms.Label labText;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
	}
}