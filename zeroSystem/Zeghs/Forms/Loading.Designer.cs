namespace Zeghs.Forms {
	partial class Loading {
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
			this.imageLoading = new System.Windows.Forms.PictureBox();
			this.labLoading = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.imageLoading)).BeginInit();
			this.SuspendLayout();
			// 
			// imageLoading
			// 
			this.imageLoading.Image = global::ZeroSystem.Properties.Resources.loading;
			this.imageLoading.Location = new System.Drawing.Point(10, 5);
			this.imageLoading.Name = "imageLoading";
			this.imageLoading.Size = new System.Drawing.Size(40, 40);
			this.imageLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.imageLoading.TabIndex = 0;
			this.imageLoading.TabStop = false;
			// 
			// labLoading
			// 
			this.labLoading.AutoSize = true;
			this.labLoading.Location = new System.Drawing.Point(66, 18);
			this.labLoading.Name = "labLoading";
			this.labLoading.Size = new System.Drawing.Size(53, 12);
			this.labLoading.TabIndex = 1;
			this.labLoading.Text = "Loading...";
			// 
			// Loading
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(150, 50);
			this.Controls.Add(this.labLoading);
			this.Controls.Add(this.imageLoading);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "Loading";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.Loading_Shown);
			((System.ComponentModel.ISupportInitialize)(this.imageLoading)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox imageLoading;
		private System.Windows.Forms.Label labLoading;
	}
}