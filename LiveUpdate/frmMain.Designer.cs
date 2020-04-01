namespace LiveUpdate {
	partial class frmMain {
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		/// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 設計工具產生的程式碼

		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
		/// 修改這個方法的內容。
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.labText = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.imgBanner = new System.Windows.Forms.PictureBox();
			this.btnQuit = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labLiveUpdate = new System.Windows.Forms.Label();
			this.labCopyright = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.imgBanner)).BeginInit();
			this.SuspendLayout();
			// 
			// labText
			// 
			this.labText.Location = new System.Drawing.Point(9, 121);
			this.labText.Name = "labText";
			this.labText.Size = new System.Drawing.Size(383, 13);
			this.labText.TabIndex = 6;
			this.labText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(6, 137);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(386, 16);
			this.progressBar.TabIndex = 5;
			// 
			// imgBanner
			// 
			this.imgBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imgBanner.Image = global::Zeghs.Properties.Resources.frmMain_banner;
			this.imgBanner.Location = new System.Drawing.Point(0, 0);
			this.imgBanner.Name = "imgBanner";
			this.imgBanner.Size = new System.Drawing.Size(397, 115);
			this.imgBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.imgBanner.TabIndex = 9;
			this.imgBanner.TabStop = false;
			// 
			// btnQuit
			// 
			this.btnQuit.Image = global::Zeghs.Properties.Resources.frmMain_btnQuit_quit;
			this.btnQuit.Location = new System.Drawing.Point(307, 164);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(85, 30);
			this.btnQuit.TabIndex = 8;
			this.btnQuit.Text = "離開";
			this.btnQuit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnQuit.UseVisualStyleBackColor = true;
			this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.Image = global::Zeghs.Properties.Resources.frmMain_btnUpdate_update;
			this.btnUpdate.Location = new System.Drawing.Point(216, 164);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(85, 30);
			this.btnUpdate.TabIndex = 7;
			this.btnUpdate.Text = "更新";
			this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// labLiveUpdate
			// 
			this.labLiveUpdate.AutoSize = true;
			this.labLiveUpdate.BackColor = System.Drawing.SystemColors.Control;
			this.labLiveUpdate.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labLiveUpdate.ForeColor = System.Drawing.Color.White;
			this.labLiveUpdate.Location = new System.Drawing.Point(121, 35);
			this.labLiveUpdate.Name = "labLiveUpdate";
			this.labLiveUpdate.Size = new System.Drawing.Size(148, 34);
			this.labLiveUpdate.TabIndex = 10;
			this.labLiveUpdate.Text = "LiveUpdate";
			// 
			// labCopyright
			// 
			this.labCopyright.AutoSize = true;
			this.labCopyright.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labCopyright.ForeColor = System.Drawing.Color.White;
			this.labCopyright.Location = new System.Drawing.Point(205, 96);
			this.labCopyright.Name = "labCopyright";
			this.labCopyright.Size = new System.Drawing.Size(190, 14);
			this.labCopyright.TabIndex = 11;
			this.labCopyright.Text = "Copyright © Web Electric Services.";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(397, 204);
			this.Controls.Add(this.labCopyright);
			this.Controls.Add(this.labLiveUpdate);
			this.Controls.Add(this.imgBanner);
			this.Controls.Add(this.btnQuit);
			this.Controls.Add(this.btnUpdate);
			this.Controls.Add(this.labText);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "版本更新";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.imgBanner)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnQuit;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label labText;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.PictureBox imgBanner;
		private System.Windows.Forms.Label labLiveUpdate;
		private System.Windows.Forms.Label labCopyright;
	}
}

