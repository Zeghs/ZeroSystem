namespace Netwings.Forms {
	partial class frmCustomBrokerFeeSetting {
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
			this.groupSetting = new System.Windows.Forms.GroupBox();
			this.labMemo = new System.Windows.Forms.Label();
			this.txtFee = new System.Windows.Forms.TextBox();
			this.labFee = new System.Windows.Forms.Label();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.groupSetting.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupSetting
			// 
			this.groupSetting.Controls.Add(this.labMemo);
			this.groupSetting.Controls.Add(this.txtFee);
			this.groupSetting.Controls.Add(this.labFee);
			this.groupSetting.Location = new System.Drawing.Point(12, 12);
			this.groupSetting.Name = "groupSetting";
			this.groupSetting.Size = new System.Drawing.Size(197, 93);
			this.groupSetting.TabIndex = 0;
			this.groupSetting.TabStop = false;
			this.groupSetting.Text = "手續費用";
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Location = new System.Drawing.Point(14, 25);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(137, 12);
			this.labMemo.TabIndex = 2;
			this.labMemo.Text = "經紀商自訂手續費用設定";
			// 
			// txtFee
			// 
			this.txtFee.Location = new System.Drawing.Point(77, 52);
			this.txtFee.Name = "txtFee";
			this.txtFee.Size = new System.Drawing.Size(76, 22);
			this.txtFee.TabIndex = 1;
			this.txtFee.Text = "0";
			this.txtFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labFee
			// 
			this.labFee.AutoSize = true;
			this.labFee.Location = new System.Drawing.Point(30, 56);
			this.labFee.Name = "labFee";
			this.labFee.Size = new System.Drawing.Size(41, 12);
			this.labFee.TabIndex = 0;
			this.labFee.Text = "手續費";
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(36, 116);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 1;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.Location = new System.Drawing.Point(117, 116);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 23);
			this.bCancel.TabIndex = 2;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// frmCustomBrokerFeeSetting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(223, 151);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.groupSetting);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmCustomBrokerFeeSetting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "經紀商手續費用";
			this.groupSetting.ResumeLayout(false);
			this.groupSetting.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupSetting;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.TextBox txtFee;
		private System.Windows.Forms.Label labFee;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
	}
}