using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Zeghs.Utils;

namespace LiveUpdate {
	static class Program {
		private static readonly string __sMainFile = "zeroSystem.exe";
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main() {
			string sURL = null;
			string sProductPath = string.Format("{0}\\{1}", Path.GetDirectoryName(Application.ExecutablePath), __sMainFile);
			if (File.Exists(sProductPath)) {
				sURL = LiveUpdateUtil.GetUpdateUrl(FileVersionInfo.GetVersionInfo(sProductPath));
			} else {
				return;
			}

			bool bCompleted = false;
			if (sURL != null) {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				frmMain frmMain = new frmMain();
				frmMain.SetUpdateUrl(sURL);

				Application.Run(frmMain);

				bCompleted = frmMain.IsCompleted;
			}

			if (bCompleted) {
				if (File.Exists(__sMainFile)) {
					Process.Start(__sMainFile);
				}
			}
		}
	}
}
