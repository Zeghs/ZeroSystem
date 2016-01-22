using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using log4net.Config;
using PowerLanguage;
using PowerLanguage.Strategy;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Scripts;
using Zeghs.Products;
using Zeghs.Settings;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Test {
	class Program {
		private static int __iPipedNumber = 1;
		private static ManualResetEvent __cManualReset = new ManualResetEvent(false);

		private static SignalObject CreateSignalObject() {
			SignalObject cSignalObject = new __BIAS_Signal(new object());
			//SignalObject cSignalObject = new __TEST_Signal(new object());
			//SignalObject cSignalObject = new __Liberty_Signal(new object());
			//SignalObject cSignalObject = new __Martingale_Signal(new object());
			//SignalObject cSignalObject = new __WeekOption_Signal(new object());
			//SignalObject cSignalObject = new __WEEK_OPTIONS_Signal(new object());
			//SignalObject cSignalObject = new __OPTIONS_Signal(new object());
			//SignalObject cSignalObject = new __Trust_Signal(new object());
			cSignalObject.onScriptParameters += Script_onScriptParameters;

			SignalSetting cSetting = SignalSetting.LoadSetting(cSignalObject);
			__iPipedNumber = cSetting.PipedNumber;
			
			cSignalObject.ApplyProperty(cSetting.Property);
			cSignalObject.AddDataStreams(RequestSetting.Convert(cSetting.DataRequests));
			return cSignalObject;
		}

		private static void Script_onScriptParameters(object sender, ScriptParametersEvent e) {
			List<InputAttribute> cParameters = e.ScriptParameters;
			foreach (InputAttribute cParameter in cParameters) {
				if (cParameter.Name.Equals("PipeNumber")) {
					cParameter.SetValue(__iPipedNumber);
				}
			}

			/* 設定手續費
			RulePropertyAttribute cRuleProperty = e.OrderService.GetRuleItems(ERuleType.Fee)[0];
			RuleBase cRule = new RuleBase();
			cRule.ClassName = cRuleProperty.ClassName;

			List<Zeghs.Orders.ICommission> cCommissions = new List<Zeghs.Orders.ICommission>();
			cCommissions.Add(e.OrderService.CreateCommission(cRule));
			e.OrderService.SetCommissions(cCommissions);
			//*/ 

			__cManualReset.Set();
		}

		private static void Main(string[] args) {
			Assembly.LoadFrom("WES.Runtime.Indicator.dll");  //預先載入否則編譯會找不到(因為此 Dll 只有在信號內會使用到)

			CheckDirectory();  //檢查資料夾是否存在

			XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net_config.xml"));

			ProductManager.Load("..\\..\\..\\..\\..\\..\\exchanges");
			GlobalSettings.Load("..\\..\\..\\..\\..\\..\\options.set");

			SeriesManager.LoadSettings();
			ScriptManager.LoadSettings();

			OrderManager.Manager.Refresh("..\\..\\..\\..\\..\\..\\plugins\\orders");
			QuoteManager.Manager.Refresh("..\\..\\..\\..\\..\\..\\plugins\\quotes");

			//------------------------------------------------------------------------------

			int iQuoteIndex = 0;
			QuoteServiceInformation[] cQuoteServiceInfos = QuoteManager.Manager.GetQuoteServiceInformations();
			int iQuoteCount = cQuoteServiceInfos.Length;

			foreach (QuoteServiceInformation cQuoteServiceInfo in cQuoteServiceInfos) {
				Console.WriteLine(string.Format("{0}. {1} [{2}]\n    {3}({4})", (++iQuoteIndex).ToString("0#"), cQuoteServiceInfo.Name, cQuoteServiceInfo.FileVersion, cQuoteServiceInfo.Description, cQuoteServiceInfo.Company));
			}

			Console.WriteLine("------------------");
			Console.Write("請選擇報價資訊來源(按'Enter'則不使用資訊來源):");

			iQuoteIndex = 0;
			string sQuoteIndex = Console.ReadLine();
			if (sQuoteIndex.Length > 0) {
				if (!int.TryParse(sQuoteIndex, out iQuoteIndex)) {
					iQuoteIndex = 1;
				}
				--iQuoteIndex;

				if (iQuoteIndex < 0 || iQuoteIndex > iQuoteCount) {
					Console.WriteLine("ERROR:無此報價資訊來源");
					goto exit;
				}

				QuoteServiceInformation cQuoteInformation = cQuoteServiceInfos[iQuoteIndex];
				QuoteManager.Manager.StartQuoteService(cQuoteInformation);
			}
			Console.WriteLine();

			Console.WriteLine("讀取腳本信號設定檔並啟動腳本...");

			SignalObject cSignalObject = CreateSignalObject();

			Console.WriteLine();

			__cManualReset.WaitOne();

			Console.WriteLine("請按任意鍵結束腳本測試...");
			Console.ReadLine();

			//釋放腳本資源
			cSignalObject.Dispose();

			//關閉所有報價資訊服務
			QuoteManager.Manager.CloseAll();

			Console.WriteLine("編譯所有腳本中...");

			//讀取所有腳本並重新編譯新的動態連結檔
			ScriptManager.Manager.LoadScripts();

			Console.WriteLine("編譯腳本完成...");
			
		exit:			
			Console.WriteLine();
			Console.WriteLine("請按任意鍵結束...");
			Console.ReadLine();
		}

		private static void CheckDirectory() {
			string sPathRoot = Path.GetPathRoot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows));
			string sEAPath = sPathRoot + @"程式交易\";
			string sDatabasePath = sEAPath + @"Databases\";
			string sLogPath = sEAPath + @"Logs\";
			string sOrderPath = sEAPath + @"Orders\";

			IsDirectoryExist(sEAPath);
			IsDirectoryExist(sDatabasePath);
			IsDirectoryExist(sLogPath);
			IsDirectoryExist(sOrderPath);
		}

		private static void IsDirectoryExist(string 資料夾) {
			if (!System.IO.Directory.Exists(資料夾)) {
				System.IO.Directory.CreateDirectory(資料夾);
			}
		}
	}
}