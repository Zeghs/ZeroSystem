using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;
using Zeghs.Utils;
using Zeghs.Events;

namespace LiveUpdate {
	public partial class frmMain : Form {
		private bool __bCompleted = false;
		private string __sUpdateURL = null;
		private WebClient __cWebClient = null;
		private Compression __cCompression = null;

		internal bool IsCompleted {
			get {
				return __bCompleted;
			}
		}
 
		public frmMain() {
			InitializeComponent();
		}

		internal void SetUpdateUrl(string url) {
			__sUpdateURL = url;
		}

		private void AsyncCompression() {
			Task.Factory.StartNew(() => {
				string sExePath = Application.StartupPath;
				__cCompression.Extract("update.zip", null, sExePath);

				File.Delete("update.zip");  //刪除更新的zip檔

				__bCompleted = true;  //完成更新

				labText.Invoke((MethodInvoker) delegate {
					labText.Text = "Update completed";
				});

				if (!this.IsDisposed) {
					this.BeginInvoke((MethodInvoker) delegate {
						this.Close();
					});
				}
			});
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			if (__cWebClient != null) {
				__cWebClient.Dispose();
			}

			if (__cCompression != null) {
				__cCompression.Dispose();
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e) {
			btnUpdate.Enabled = false;

			__cCompression = new Compression();
			__cCompression.onExtractProgress += Compression_onExtractProgress;

			__cWebClient = new WebClient();
			__cWebClient.DownloadFileCompleted += WebClient_onDownloadFileCompleted;
			__cWebClient.DownloadProgressChanged += WebClient_onDownloadProgressChanged;

			__cWebClient.DownloadFileAsync(new Uri(__sUpdateURL), "update.zip");
		}

		private void btnQuit_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void WebClient_onDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
			Application.DoEvents();

			progressBar.Value = 0;
			labText.Text = string.Empty;

			if (File.Exists("update.zip")) {
				FileInfo cFileInfo = new FileInfo("update.zip");
				if (cFileInfo.Length == 0) {
					File.Delete("update.zip");  //刪除更新的zip檔
				} else {
					AsyncCompression();
					return;
				}
			}
			this.Close();
		}

		private void WebClient_onDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			labText.Text = string.Format("Download update file: {0}%", e.ProgressPercentage);
			progressBar.Value = e.ProgressPercentage;
			progressBar.Maximum = 100;
		}

		private void Compression_onExtractProgress(object sender, ProgressEvent e) {
			labText.Invoke((MethodInvoker) delegate {
				labText.Text = string.Format("Update files: {0} ({1} / {2})", e.FileName, e.Current.ToString("N"), e.Totals.ToString("N"));
			});

			progressBar.Invoke((MethodInvoker) delegate {
				progressBar.Value = e.Current;
				progressBar.Maximum = e.Totals;
			});
		}
	}
}