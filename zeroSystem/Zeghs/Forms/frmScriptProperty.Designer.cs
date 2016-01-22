namespace Zeghs.Forms {
	partial class frmScriptProperty {
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
			this.label1 = new System.Windows.Forms.Label();
			this.labVersion = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labComment = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labCopyright = new System.Windows.Forms.Label();
			this.groupInformation = new System.Windows.Forms.GroupBox();
			this.labScriptType = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labFullName = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupInformation.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "模組版本:";
			// 
			// labVersion
			// 
			this.labVersion.Location = new System.Drawing.Point(94, 9);
			this.labVersion.Name = "labVersion";
			this.labVersion.Size = new System.Drawing.Size(105, 12);
			this.labVersion.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(35, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "模組描述:";
			// 
			// labComment
			// 
			this.labComment.Location = new System.Drawing.Point(94, 31);
			this.labComment.Name = "labComment";
			this.labComment.Size = new System.Drawing.Size(330, 12);
			this.labComment.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(35, 54);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "著作商標:";
			// 
			// labCopyright
			// 
			this.labCopyright.Location = new System.Drawing.Point(94, 54);
			this.labCopyright.Name = "labCopyright";
			this.labCopyright.Size = new System.Drawing.Size(330, 12);
			this.labCopyright.TabIndex = 5;
			// 
			// groupInformation
			// 
			this.groupInformation.Controls.Add(this.labScriptType);
			this.groupInformation.Controls.Add(this.label6);
			this.groupInformation.Controls.Add(this.labFullName);
			this.groupInformation.Controls.Add(this.label5);
			this.groupInformation.Controls.Add(this.labName);
			this.groupInformation.Controls.Add(this.label4);
			this.groupInformation.Location = new System.Drawing.Point(24, 83);
			this.groupInformation.Name = "groupInformation";
			this.groupInformation.Size = new System.Drawing.Size(425, 99);
			this.groupInformation.TabIndex = 6;
			this.groupInformation.TabStop = false;
			this.groupInformation.Text = "其他資訊";
			// 
			// labScriptType
			// 
			this.labScriptType.Location = new System.Drawing.Point(70, 71);
			this.labScriptType.Name = "labScriptType";
			this.labScriptType.Size = new System.Drawing.Size(330, 12);
			this.labScriptType.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(11, 71);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 12);
			this.label6.TabIndex = 9;
			this.label6.Text = "腳本類型:";
			// 
			// labFullName
			// 
			this.labFullName.Location = new System.Drawing.Point(70, 46);
			this.labFullName.Name = "labFullName";
			this.labFullName.Size = new System.Drawing.Size(330, 12);
			this.labFullName.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(11, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 12);
			this.label5.TabIndex = 7;
			this.label5.Text = "完整名稱:";
			// 
			// labName
			// 
			this.labName.Location = new System.Drawing.Point(70, 23);
			this.labName.Name = "labName";
			this.labName.Size = new System.Drawing.Size(330, 12);
			this.labName.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 12);
			this.label4.TabIndex = 5;
			this.label4.Text = "腳本名稱:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(369, 188);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 23);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "確定";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmScriptProperty
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(474, 220);
			this.ControlBox = false;
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupInformation);
			this.Controls.Add(this.labCopyright);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labComment);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labVersion);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmScriptProperty";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "腳本屬性";
			this.groupInformation.ResumeLayout(false);
			this.groupInformation.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labVersion;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labComment;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labCopyright;
		private System.Windows.Forms.GroupBox groupInformation;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labScriptType;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labFullName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labName;
		private System.Windows.Forms.Button btnOK;
	}
}