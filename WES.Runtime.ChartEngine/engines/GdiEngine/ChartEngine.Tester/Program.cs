using System;
using System.Windows.Forms;
using Zeghs.Managers;

namespace ChartEngine.Tester {
	static class Program {
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main() {
			ProductManager.Load("exchanges");
			SeriesManager.LoadSettings();
			OrderManager.Manager.Refresh("plugins\\orders");
			QuoteManager.Manager.Refresh("plugins\\quotes");
			PaintManager.Manager.Refresh("plugins\\charts");
			QuoteManager.Manager.StartQuoteService(QuoteManager.Manager.GetQuoteServiceInformations()[0]);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmChart());
		}
	}
}
