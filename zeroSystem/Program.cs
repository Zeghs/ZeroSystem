using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using PowerLanguage;
using Zeghs.Forms;
using Zeghs.Managers;
using Zeghs.Settings;

namespace zeroSystem {
	static class Program {
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main() {
			//檢查是否有即時更新程式的換版檔案
			if (File.Exists("LiveUpdate.ex_")) {
				File.Delete("LiveUpdate.exe");  //先刪除舊版本
				File.Move("LiveUpdate.ex_", "LiveUpdate.exe");  //再將新的程式更名
			}

			Assembly.LoadFrom("WES.Runtime.Indicator.dll");  //預先載入否則編譯會找不到(因為此 Dll 只有在信號內會使用到)

			XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net_config.xml"));

			ProductManager.Load("exchanges");
			GlobalSettings.Load("options.set");

			SeriesManager.LoadSettings();
			ScriptManager.LoadSettings();

			OrderManager.Manager.Refresh("plugins\\orders");
			QuoteManager.Manager.Refresh("plugins\\quotes");
			PaintManager.Manager.Refresh("plugins\\charts");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			frmWelcome frmWelcome = new frmWelcome();
			frmWelcome.ShowDialog();
			frmWelcome.Dispose();

			Application.Run(new frmMain());
		}
	}
}