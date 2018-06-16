namespace Zeghs.Forms {
	partial class frmWelcome {
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
			this.labTitle = new System.Windows.Forms.Label();
			this.labMemo = new System.Windows.Forms.Label();
			this.labCopyright = new System.Windows.Forms.Label();
			this.labVersion = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labTitle
			// 
			this.labTitle.AutoSize = true;
			this.labTitle.Font = new System.Drawing.Font("Impact", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labTitle.ForeColor = System.Drawing.Color.White;
			this.labTitle.Location = new System.Drawing.Point(0, 0);
			this.labTitle.Name = "labTitle";
			this.labTitle.Size = new System.Drawing.Size(209, 43);
			this.labTitle.TabIndex = 0;
			this.labTitle.Text = "ZERO System";
			// 
			// labMemo
			// 
			this.labMemo.AutoSize = true;
			this.labMemo.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labMemo.ForeColor = System.Drawing.Color.White;
			this.labMemo.Location = new System.Drawing.Point(5, 43);
			this.labMemo.Name = "labMemo";
			this.labMemo.Size = new System.Drawing.Size(267, 16);
			this.labMemo.TabIndex = 1;
			this.labMemo.Text = "Zoning and Emotional Range Omitted System";
			// 
			// labCopyright
			// 
			this.labCopyright.AutoSize = true;
			this.labCopyright.BackColor = System.Drawing.Color.Black;
			this.labCopyright.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labCopyright.ForeColor = System.Drawing.Color.White;
			this.labCopyright.Location = new System.Drawing.Point(64, 279);
			this.labCopyright.Name = "labCopyright";
			this.labCopyright.Size = new System.Drawing.Size(270, 16);
			this.labCopyright.TabIndex = 2;
			this.labCopyright.Text = "Copyright © Web Electric Services 2004 - 2018";
			// 
			// labVersion
			// 
			this.labVersion.AutoSize = true;
			this.labVersion.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labVersion.ForeColor = System.Drawing.Color.White;
			this.labVersion.Location = new System.Drawing.Point(273, 43);
			this.labVersion.Name = "labVersion";
			this.labVersion.Size = new System.Drawing.Size(0, 16);
			this.labVersion.TabIndex = 3;
			// 
			// frmWelcome
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = global::ZeroSystem.Properties.Resources.frmWelcome_background;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(400, 300);
			this.ControlBox = false;
			this.Controls.Add(this.labVersion);
			this.Controls.Add(this.labCopyright);
			this.Controls.Add(this.labMemo);
			this.Controls.Add(this.labTitle);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmWelcome";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.frmWelcome_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labTitle;
		private System.Windows.Forms.Label labMemo;
		private System.Windows.Forms.Label labCopyright;
		private System.Windows.Forms.Label labVersion;
	}
}