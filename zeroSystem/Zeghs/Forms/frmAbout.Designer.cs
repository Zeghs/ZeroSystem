namespace Zeghs.Forms {
	partial class frmAbout {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labProduct = new System.Windows.Forms.Label();
			this.labCopyright = new System.Windows.Forms.Label();
			this.labMemo = new System.Windows.Forms.Label();
			this.labCompany = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(20, 108);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(313, 381);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// labProduct
			// 
			this.labProduct.AutoSize = true;
			this.labProduct.Font = new System.Drawing.Font("Impact", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labProduct.ForeColor = System.Drawing.Color.RoyalBlue;
			this.labProduct.Location = new System.Drawing.Point(36, 21);
			this.labProduct.Name = "labProduct";
			this.labProduct.Size = new System.Drawing.Size(282, 60);
			this.labProduct.TabIndex = 1;
			this.labProduct.Text = "ZERO System";
			// 
			// labCopyright
			// 
			this.labCopyright.AutoSize = true;
			this.labCopyright.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.labCopyright.Location = new System.Drawing.Point(42, 493);
			this.labCopyright.Name = "labCopyright";
			this.labCopyright.Size = new System.Drawing.Size(270, 16);
			this.labCopyright.TabIndex = 2;
			this.labCopyright.Text = "Copyright © Web Electric Services 2004 - 2019";
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.labMemo.Location = new System.Drawing.Point(32, 85);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(289, 17);
			this.labMemo.TabIndex = 3;
			this.labMemo.Text = "Zoning and Emotional Range Omitted System";
			// 
			// labCompany
			// 
			this.labCompany.AutoSize = true;
			this.labCompany.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.labCompany.Location = new System.Drawing.Point(66, 4);
			this.labCompany.Name = "labCompany";
			this.labCompany.Size = new System.Drawing.Size(222, 17);
			this.labCompany.TabIndex = 4;
			this.labCompany.Text = "Web Electric Services, by ZEGHS Lin.";
			// 
			// frmAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(354, 518);
			this.Controls.Add(this.labCompany);
			this.Controls.Add(this.labMemo);
			this.Controls.Add(this.labCopyright);
			this.Controls.Add(this.labProduct);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "關於 - ZERO System";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labProduct;
		private System.Windows.Forms.Label labCopyright;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.Label labCompany;

	}
}