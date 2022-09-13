using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mitake;
using Zeghs.Managers;
using Zeghs.Data;
using System.Threading;


using Mitake.Sockets.Data;
using Mitake.Stock.Decode;
using Mitake.Stock.Data;
using Mitake.Stock.Util;

using PowerLanguage;
using System.IO;

namespace Zeghs {
	static class Program {
		static QuoteService cService = null;

		[STAThread]
		static void Main() {
			//*
			cService = new QuoteService();
			//cService.onQuote += new EventHandler<Events.QuoteEvent>(cService_onQuote);
			cService.onLoginCompleted += new EventHandler(cService_onLoginCompleted);
			cService.onSubscribeCompleted += new EventHandler<Events.QuoteComplementCompletedEvent>(cService_onSubscribeCompleted);
			cService.onComplementCompleted += new EventHandler<Events.QuoteComplementCompletedEvent>(cService_onComplementCompleted);
			cService.Load();

			cService.Login();

			MitakeSymbolManager.DataSource = "Mitake";
			MitakeSymbolManager.ExchangeName = "TWSE";
			cService.SymbolUpdate();
			//*/

			/*
			MitakeSymbolManager.DataSource = "Mitake";
			MitakeSymbolManager.ExchangeName = "TWSE";

			System.Console.Write("Input Date(yyyy/MM/dd):");
			DateTime cDate = DateTime.Parse(System.Console.ReadLine());
	
			Time.SetToday(cDate);

			string sFile = string.Format("STOCK{0}.bak", cDate.ToString("yyyyMMdd"));

			System.Console.WriteLine(sFile);

			cService = new QuoteService();
			cService.Load();
			cService.Dispose();

			try {
				StockDecoder.StockProc += new Mitake.Events.StockHandler(StockDecoder_StockProc);
				if (System.IO.File.Exists(sFile)) {
					int iSize = 0;
					SocketToken cToken = new SocketToken();
					System.IO.FileStream cStream = new System.IO.FileStream(sFile, System.IO.FileMode.Open);
					while ((iSize = cStream.Read(cToken.ReceiveBuffer.Data, 0, 8192)) > 0) {
						cToken.ReceiveBuffer.Length = iSize;
						StockDecoder.Decode(null, cToken, false);
					}
					cStream.Close();
				}
			} catch(Exception __errExcep) {
				System.Console.WriteLine("{0}\r\n{1}", __errExcep.Message, __errExcep.StackTrace);
			}

			System.Console.WriteLine("Completed...");
			//*/

			System.Console.ReadLine();
		}

		private static object __oLock = new object();
		private static void cService_onQuote(object sender, Events.QuoteEvent e) {
			//ITick cTick = e.Quote.GetTick(0);
			//System.Console.WriteLine("{0} {1,8:0.00} {2,10} {3, 10}", cTick.Time.ToString("HHmmss"), cTick.Price, cTick.Single, cTick.Volume);
			lock (__oLock) {
				System.Console.CursorLeft = 0;
				System.Console.CursorTop = 0;
				IDOMData cData = e.Quote.DOM;
				for (int i = 0; i < 5; i++) {
					System.Console.CursorTop = i;
					System.Console.WriteLine("{0,8} {1,8} {2,8} {3,8}", cData.Bid[i].Size, cData.Bid[i].Price, cData.Ask[i].Price, cData.Ask[i].Size);
				}
			}
		}

		static DateTime dddd = new DateTime(2015,3,12,13,44,59);
		static void StockDecoder_StockProc(object sender, Mitake.Events.StockEvent e) {
			if (e.Type == 0x38) {
				Decode_S38.Decode(e.Serial, e.Source);
			}

			if (e.Header == 0x53 && e.Serial == 598 && (e.Type == 0x31 || e.Type == 0x3e || e.Type == 0xb1 || e.Type == 0xbe)) {
				MitakeQuote cQuote = MitakeStorage.Storage.GetQuote(e.Serial);
				if (cQuote != null) {
					if (cQuote.即時資訊.Time >= dddd) {
						//Thread.Sleep(2000);
					}
					System.Console.WriteLine(cQuote.即時資訊.Serial + " " + cQuote.即時資訊.Time.ToString("yyyyMMdd HH:mm:ss") + " " + cQuote.即時資訊.Bid.Price + " " + cQuote.即時資訊.Ask.Price + " " + cQuote.即時資訊.Price + " " + cQuote.即時資訊.Single + " " + cQuote.即時資訊.Volume);
				}
				Decode_S31.Decode(cQuote, e.Source);
			}
		}

		static void cService_onSubscribeCompleted(object sender, Events.QuoteComplementCompletedEvent e) {
			//cService.onSubscribeCompleted -= cService_onSubscribeCompleted;
			System.Console.WriteLine("s " + e.SymbolId);
			IQuote cQuote = cService.Storage.GetQuote(e.SymbolId);
			System.Console.WriteLine(cQuote);
			System.Console.Clear();
		}

		static void cService_onComplementCompleted(object sender, Events.QuoteComplementCompletedEvent e) {
			//cService.onComplementCompleted -= cService_onComplementCompleted;
			System.Console.WriteLine("c " + e.SymbolId);
			IQuote cQuote = cService.Storage.GetQuote(e.SymbolId);
			System.Console.WriteLine(cQuote);
			System.Console.WriteLine("symbol:{0} name:{1}", cQuote.SymbolId, cQuote.SymbolName);

			double dVolume = 0;
			int iCount = cQuote.TickCount;
			for (int i = iCount - 1; i >= 0; i--) {
				ITick cTick =  cQuote.GetTick(i);
				System.Console.WriteLine("{7,7} {0} {1,8:0.00} {2,8:0.00} {3,8:0.00} {4,10} {5, 10} {6, 10}", cTick.Time.ToString("HHmmss"), cTick.Bid.Price, cTick.Ask.Price, cTick.Price, cTick.Single, cTick.Volume, cTick.Volume - dVolume, (cTick as MitakeQuoteTick).Serial);
				File.AppendAllText("aaa.txt", string.Format("{7,7} {0} {1,8:0.00} {2,8:0.00} {3,8:0.00} {4,10} {5, 10} {6, 10}\r\n", cTick.Time.ToString("HHmmss"), cTick.Bid.Price, cTick.Ask.Price, cTick.Price, cTick.Single, cTick.Volume, cTick.Volume - dVolume, (cTick as MitakeQuoteTick).Serial), Encoding.UTF8);
				if (cTick.Volume > dVolume) {
					dVolume = cTick.Volume;
				}
			}
		}

		static void cService_onLoginCompleted(object sender, EventArgs e) {
			cService.onLoginCompleted -= cService_onLoginCompleted;
			//cService.SymbolUpdate();

			System.Console.WriteLine("訂閱......");
			cService.Complement("TXF0.tw");
			//cService.AddSubscribe("MXWN0.tw");
			//cService.Complement("MXFN0.tw");
			//cService.Complement("MXWN0.tw");
			//cService.AddSubscribe("MXW0.tw");
			//cService.Complement("TXFN0.tw");
			//cService.Complement("TXW0C9250.tw");
			//cService.AddSubscribe("OTC.tw");
			//cService.Complement("EXF0.tw");
			//cService.AddSubscribe("TXF0.tw");
			//cService.Complement("TXF0.tw");
			//cService.AddSubscribe("2330.tw");
			//cService.Complement("2371.tw");
		}
	}
}
