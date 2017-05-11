namespace ChartEngine.Tester {
	partial class frmChart {
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
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.itemPointer = new System.Windows.Forms.ToolStripButton();
			this.itemCross = new System.Windows.Forms.ToolStripButton();
			this.itemPaint = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.chart = new System.Windows.Forms.Control();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemPointer,
            this.itemCross,
            this.itemPaint,
            this.toolStripSeparator1});
			this.toolbar.Location = new System.Drawing.Point(0, 0);
			this.toolbar.Name = "toolbar";
			this.toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolbar.Size = new System.Drawing.Size(792, 25);
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
			// 
			// itemPointer
			// 
			this.itemPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.itemPointer.Image = global::Zeghs.Properties.Resources.toolbar_pointer;
			this.itemPointer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.itemPointer.Name = "itemPointer";
			this.itemPointer.Size = new System.Drawing.Size(23, 22);
			this.itemPointer.Click += new System.EventHandler(this.itemPointer_Click);
			// 
			// itemCross
			// 
			this.itemCross.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.itemCross.Image = global::Zeghs.Properties.Resources.toolbar_cross;
			this.itemCross.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.itemCross.Name = "itemCross";
			this.itemCross.Size = new System.Drawing.Size(23, 22);
			this.itemCross.Click += new System.EventHandler(this.itemCross_Click);
			// 
			// itemPaint
			// 
			this.itemPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.itemPaint.Image = global::Zeghs.Properties.Resources.toolbar_paint;
			this.itemPaint.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.itemPaint.Name = "itemPaint";
			this.itemPaint.Size = new System.Drawing.Size(23, 22);
			this.itemPaint.Click += new System.EventHandler(this.itemPaint_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// chart
			// 
			this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chart.Location = new System.Drawing.Point(0, 25);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(792, 541);
			this.chart.TabIndex = 1;
			// 
			// frmChart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.chart);
			this.Controls.Add(this.toolbar);
			this.Name = "frmChart";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChartEngine Tester";
			this.Load += new System.EventHandler(this.frmChart_Load);
			this.toolbar.ResumeLayout(false);
			this.toolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private System.Windows.Forms.Control chart;
		private System.Windows.Forms.ToolStripButton itemPointer;
		private System.Windows.Forms.ToolStripButton itemCross;
		private System.Windows.Forms.ToolStripButton itemPaint;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

	}
}

