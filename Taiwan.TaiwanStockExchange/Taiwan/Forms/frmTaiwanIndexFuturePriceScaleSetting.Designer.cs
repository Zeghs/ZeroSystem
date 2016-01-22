namespace Taiwan.Forms {
	partial class frmTaiwanIndexFuturePriceScaleSetting {
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
			this.groupPriecScale = new System.Windows.Forms.GroupBox();
			this.txtMinJump = new System.Windows.Forms.TextBox();
			this.txtPriceScale = new System.Windows.Forms.TextBox();
			this.labMinJump = new System.Windows.Forms.Label();
			this.labPriceScale = new System.Windows.Forms.Label();
			this.labText = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupPriecScale.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupPriecScale
			// 
			this.groupPriecScale.Controls.Add(this.txtMinJump);
			this.groupPriecScale.Controls.Add(this.txtPriceScale);
			this.groupPriecScale.Controls.Add(this.labMinJump);
			this.groupPriecScale.Controls.Add(this.labPriceScale);
			this.groupPriecScale.Controls.Add(this.labText);
			this.groupPriecScale.Location = new System.Drawing.Point(10, 17);
			this.groupPriecScale.Name = "groupPriecScale";
			this.groupPriecScale.Size = new System.Drawing.Size(229, 126);
			this.groupPriecScale.TabIndex = 0;
			this.groupPriecScale.TabStop = false;
			this.groupPriecScale.Text = "價格座標設定";
			// 
			// txtMinJump
			// 
			this.txtMinJump.Location = new System.Drawing.Point(89, 85);
			this.txtMinJump.Name = "txtMinJump";
			this.txtMinJump.Size = new System.Drawing.Size(64, 22);
			this.txtMinJump.TabIndex = 4;
			this.txtMinJump.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtPriceScale
			// 
			this.txtPriceScale.Location = new System.Drawing.Point(89, 57);
			this.txtPriceScale.Name = "txtPriceScale";
			this.txtPriceScale.Size = new System.Drawing.Size(64, 22);
			this.txtPriceScale.TabIndex = 3;
			this.txtPriceScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labMinJump
			// 
			this.labMinJump.AutoSize = true;
			this.labMinJump.Location = new System.Drawing.Point(18, 90);
			this.labMinJump.Name = "labMinJump";
			this.labMinJump.Size = new System.Drawing.Size(65, 12);
			this.labMinJump.TabIndex = 2;
			this.labMinJump.Text = "最小跳動點";
			// 
			// labPriceScale
			// 
			this.labPriceScale.AutoSize = true;
			this.labPriceScale.Location = new System.Drawing.Point(18, 62);
			this.labPriceScale.Name = "labPriceScale";
			this.labPriceScale.Size = new System.Drawing.Size(65, 12);
			this.labPriceScale.TabIndex = 1;
			this.labPriceScale.Text = "價格座標值";
			// 
			// labText
			// 
			this.labText.AutoSize = true;
			this.labText.Location = new System.Drawing.Point(16, 29);
			this.labText.Name = "labText";
			this.labText.Size = new System.Drawing.Size(137, 12);
			this.labText.TabIndex = 0;
			this.labText.Text = "設定座標與最小跳動點數";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(127, 151);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(46, 151);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmTaiwanIndexFuturePriceScaleSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(249, 189);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupPriecScale);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTaiwanIndexFuturePriceScaleSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "台灣指數型期貨價格座標參數設定";
			this.Load += new System.EventHandler(this.frmTaiwanIndexFuturePriceScaleSetting_Load);
			this.groupPriecScale.ResumeLayout(false);
			this.groupPriecScale.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupPriecScale;
		private System.Windows.Forms.TextBox txtMinJump;
		private System.Windows.Forms.TextBox txtPriceScale;
		private System.Windows.Forms.Label labMinJump;
		private System.Windows.Forms.Label labPriceScale;
		private System.Windows.Forms.Label labText;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
	}
}