namespace Zeghs.Forms {
	partial class frmQuoteServiceSettings {
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
			this.groupNetwork = new System.Windows.Forms.GroupBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.txtAccount = new System.Windows.Forms.TextBox();
			this.txtRemotePort = new System.Windows.Forms.TextBox();
			this.txtRemoteIP = new System.Windows.Forms.TextBox();
			this.labPassword = new System.Windows.Forms.Label();
			this.labAccount = new System.Windows.Forms.Label();
			this.labAddress = new System.Windows.Forms.Label();
			this.groupInfo = new System.Windows.Forms.GroupBox();
			this.txtDataSource = new System.Windows.Forms.TextBox();
			this.txtExchange = new System.Windows.Forms.TextBox();
			this.labExchange = new System.Windows.Forms.Label();
			this.labDataSource = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupNetwork.SuspendLayout();
			this.groupInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupNetwork
			// 
			this.groupNetwork.Controls.Add(this.txtPassword);
			this.groupNetwork.Controls.Add(this.txtAccount);
			this.groupNetwork.Controls.Add(this.txtRemotePort);
			this.groupNetwork.Controls.Add(this.txtRemoteIP);
			this.groupNetwork.Controls.Add(this.labPassword);
			this.groupNetwork.Controls.Add(this.labAccount);
			this.groupNetwork.Controls.Add(this.labAddress);
			this.groupNetwork.Location = new System.Drawing.Point(26, 123);
			this.groupNetwork.Name = "groupNetwork";
			this.groupNetwork.Size = new System.Drawing.Size(238, 117);
			this.groupNetwork.TabIndex = 11;
			this.groupNetwork.TabStop = false;
			this.groupNetwork.Text = "網路設定";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(71, 80);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(158, 22);
			this.txtPassword.TabIndex = 17;
			// 
			// txtAccount
			// 
			this.txtAccount.Location = new System.Drawing.Point(71, 50);
			this.txtAccount.Name = "txtAccount";
			this.txtAccount.Size = new System.Drawing.Size(158, 22);
			this.txtAccount.TabIndex = 16;
			// 
			// txtRemotePort
			// 
			this.txtRemotePort.Location = new System.Drawing.Point(188, 21);
			this.txtRemotePort.MaxLength = 5;
			this.txtRemotePort.Name = "txtRemotePort";
			this.txtRemotePort.Size = new System.Drawing.Size(41, 22);
			this.txtRemotePort.TabIndex = 15;
			// 
			// txtRemoteIP
			// 
			this.txtRemoteIP.Location = new System.Drawing.Point(71, 21);
			this.txtRemoteIP.Name = "txtRemoteIP";
			this.txtRemoteIP.Size = new System.Drawing.Size(111, 22);
			this.txtRemoteIP.TabIndex = 14;
			// 
			// labPassword
			// 
			this.labPassword.AutoSize = true;
			this.labPassword.Location = new System.Drawing.Point(12, 85);
			this.labPassword.Name = "labPassword";
			this.labPassword.Size = new System.Drawing.Size(53, 12);
			this.labPassword.TabIndex = 13;
			this.labPassword.Text = "登入密碼";
			// 
			// labAccount
			// 
			this.labAccount.AutoSize = true;
			this.labAccount.Location = new System.Drawing.Point(12, 55);
			this.labAccount.Name = "labAccount";
			this.labAccount.Size = new System.Drawing.Size(53, 12);
			this.labAccount.TabIndex = 12;
			this.labAccount.Text = "登入帳號";
			// 
			// labAddress
			// 
			this.labAddress.AutoSize = true;
			this.labAddress.Location = new System.Drawing.Point(12, 26);
			this.labAddress.Name = "labAddress";
			this.labAddress.Size = new System.Drawing.Size(53, 12);
			this.labAddress.TabIndex = 11;
			this.labAddress.Text = "遠端服務";
			// 
			// groupInfo
			// 
			this.groupInfo.Controls.Add(this.txtDataSource);
			this.groupInfo.Controls.Add(this.txtExchange);
			this.groupInfo.Controls.Add(this.labExchange);
			this.groupInfo.Controls.Add(this.labDataSource);
			this.groupInfo.Location = new System.Drawing.Point(26, 19);
			this.groupInfo.Name = "groupInfo";
			this.groupInfo.Size = new System.Drawing.Size(238, 85);
			this.groupInfo.TabIndex = 12;
			this.groupInfo.TabStop = false;
			this.groupInfo.Text = "來源資訊";
			// 
			// txtDataSource
			// 
			this.txtDataSource.Location = new System.Drawing.Point(71, 50);
			this.txtDataSource.Name = "txtDataSource";
			this.txtDataSource.Size = new System.Drawing.Size(158, 22);
			this.txtDataSource.TabIndex = 10;
			// 
			// txtExchange
			// 
			this.txtExchange.Location = new System.Drawing.Point(71, 21);
			this.txtExchange.Name = "txtExchange";
			this.txtExchange.ReadOnly = true;
			this.txtExchange.Size = new System.Drawing.Size(158, 22);
			this.txtExchange.TabIndex = 9;
			// 
			// labExchange
			// 
			this.labExchange.AutoSize = true;
			this.labExchange.Location = new System.Drawing.Point(24, 26);
			this.labExchange.Name = "labExchange";
			this.labExchange.Size = new System.Drawing.Size(41, 12);
			this.labExchange.TabIndex = 8;
			this.labExchange.Text = "交易所";
			// 
			// labDataSource
			// 
			this.labDataSource.AutoSize = true;
			this.labDataSource.Location = new System.Drawing.Point(12, 55);
			this.labDataSource.Name = "labDataSource";
			this.labDataSource.Size = new System.Drawing.Size(53, 12);
			this.labDataSource.TabIndex = 7;
			this.labDataSource.Text = "資料來源";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(69, 257);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 13;
			this.btnSave.Text = "儲存";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(150, 257);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 14;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmQuoteServiceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 298);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.groupInfo);
			this.Controls.Add(this.groupNetwork);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmQuoteServiceSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "即時報價服務設定";
			this.Load += new System.EventHandler(this.frmQuoteServiceSettings_Load);
			this.groupNetwork.ResumeLayout(false);
			this.groupNetwork.PerformLayout();
			this.groupInfo.ResumeLayout(false);
			this.groupInfo.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupNetwork;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtAccount;
		private System.Windows.Forms.TextBox txtRemotePort;
		private System.Windows.Forms.TextBox txtRemoteIP;
		private System.Windows.Forms.Label labPassword;
		private System.Windows.Forms.Label labAccount;
		private System.Windows.Forms.Label labAddress;
		private System.Windows.Forms.GroupBox groupInfo;
		private System.Windows.Forms.TextBox txtDataSource;
		private System.Windows.Forms.TextBox txtExchange;
		private System.Windows.Forms.Label labExchange;
		private System.Windows.Forms.Label labDataSource;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
	}
}