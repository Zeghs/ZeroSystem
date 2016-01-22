using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using PowerLanguage;
using Zeghs.Forms;
using Zeghs.Managers;

namespace zeroSystem {
	static class Program {
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main() {
			/*
			byte[] aaa = new byte[8192];
			byte[] bbb = new byte[8192];

			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			for (int i = 0; i < 10000; i++) {
				Array.Copy(aaa,bbb, aaa.Length);
			}

			sw.Stop();
			System.Console.WriteLine(sw.ElapsedMilliseconds);
		
			//*/

			Assembly.LoadFrom("WES.Runtime.Indicator.dll");  //預先載入否則編譯會找不到(因為此 Dll 只有在信號內會使用到)

			XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net_config.xml"));

			ProductManager.Load("exchanges");
			GlobalSettings.Load("options.set");

			SeriesManager.LoadSettings();
			ScriptManager.LoadSettings();
			ScriptManager.Manager.LoadScripts();

			OrderManager.Manager.Refresh("plugins\\orders");
			QuoteManager.Manager.Refresh("plugins\\quotes");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);  //如果要做系統保護則不能開啟這個選項

			frmWelcome frmWelcome = new frmWelcome();
			frmWelcome.ShowDialog();
			frmWelcome.Dispose();

			Application.Run(new frmMain());
		}
	}
}