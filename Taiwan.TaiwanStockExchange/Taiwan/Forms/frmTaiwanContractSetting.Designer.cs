namespace Taiwan.Forms {
	partial class frmTaiwanContractSetting {
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
			this.groupTime = new System.Windows.Forms.GroupBox();
			this.numericHour = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.numericMinute = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.numericSecond = new System.Windows.Forms.NumericUpDown();
			this.labText = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupTime.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMinute)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSecond)).BeginInit();
			this.SuspendLayout();
			// 
			// groupTime
			// 
			this.groupTime.Controls.Add(this.labText);
			this.groupTime.Controls.Add(this.numericSecond);
			this.groupTime.Controls.Add(this.label2);
			this.groupTime.Controls.Add(this.numericMinute);
			this.groupTime.Controls.Add(this.label1);
			this.groupTime.Controls.Add(this.numericHour);
			this.groupTime.Location = new System.Drawing.Point(10, 13);
			this.groupTime.Name = "groupTime";
			this.groupTime.Size = new System.Drawing.Size(208, 100);
			this.groupTime.TabIndex = 0;
			this.groupTime.TabStop = false;
			this.groupTime.Text = "到期日收盤時間";
			// 
			// numericHour
			// 
			this.numericHour.Location = new System.Drawing.Point(13, 56);
			this.numericHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.numericHour.Name = "numericHour";
			this.numericHour.Size = new System.Drawing.Size(36, 22);
			this.numericHour.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(53, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(8, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = ":";
			// 
			// numericMinute
			// 
			this.numericMinute.Location = new System.Drawing.Point(65, 57);
			this.numericMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.numericMinute.Name = "numericMinute";
			this.numericMinute.Size = new System.Drawing.Size(36, 22);
			this.numericMinute.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(105, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = ":";
			// 
			// numericSecond
			// 
			this.numericSecond.Location = new System.Drawing.Point(117, 57);
			this.numericSecond.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.numericSecond.Name = "numericSecond";
			this.numericSecond.Size = new System.Drawing.Size(36, 22);
			this.numericSecond.TabIndex = 4;
			// 
			// labText
			// 
			this.labText.AutoSize = true;
			this.labText.Location = new System.Drawing.Point(11, 25);
			this.labText.Name = "labText";
			this.labText.Size = new System.Drawing.Size(97, 12);
			this.labText.TabIndex = 5;
			this.labText.Text = "收盤時間(24時制)";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(38, 121);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(119, 121);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmTaiwanContractSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(229, 159);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupTime);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTaiwanContractSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "台灣合約到期日規則參數設定";
			this.Load += new System.EventHandler(this.frmTaiwanContractSetting_Load);
			this.groupTime.ResumeLayout(false);
			this.groupTime.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMinute)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSecond)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupTime;
		private System.Windows.Forms.Label labText;
		private System.Windows.Forms.NumericUpDown numericSecond;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericMinute;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericHour;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}