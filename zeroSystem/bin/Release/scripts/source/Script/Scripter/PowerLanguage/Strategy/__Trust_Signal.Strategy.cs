using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerLanguage.Function;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Scripts;

namespace PowerLanguage.Strategy {
	[ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.0", Company = "量化策略交易員", Copyright = "Copyright © 2015 量化策略交易員. 保留一切權利。", Comment = "Trust")]
	public sealed class __Trust_Signal : SignalObject {
		private static TimeSpan __cClearTime = new TimeSpan(8, 0, 0);
		private static TimeSpan __cStartTime = new TimeSpan(8, 45, 0);
		private static TimeSpan __cStopTime = new TimeSpan(13, 45, 0);
        private static TimeSpan __cSaveTime = new TimeSpan(13, 45, 1);

		private struct __Trust {
			internal DateTime time;
			internal double askV;
			internal double bidV;
			internal double diffV;
		}

		private bool __bFlag = false;
		private bool __bSaveFile = true;
		private bool isSaveBusy = false;
		private __Trust __cTrust;
		private List<__Trust> __cList = null;
		private object __oLock = new object();

		public __Trust_Signal(object _ctx)
			: base(_ctx) {
		}

		protected override void Create() {
			__cTrust = new __Trust();
			__cTrust.time = DateTime.MinValue;

			__cList = new List<__Trust>(512);
		}

		protected override void Destroy() {
			Save();
		}

		protected override void CalcBar() {

		}

		protected override void OnQuoteDateTime(QuoteDateTimeEvent e) {
			bool bFlag = false;
			lock (__oLock) {
				bFlag = __bFlag;
				__bFlag = true;
			}

			if (!bFlag) {
				DateTime cTime = e.QuoteDateTime;
				TimeSpan cTimeSpan = cTime.TimeOfDay;
				if (cTimeSpan >= __cStartTime && cTimeSpan <= __cStopTime) {
					if (cTime.Second == 0) {
						__cList.Add(__cTrust);

						__cTrust = new __Trust();
						__cTrust.time = e.QuoteDateTime;

						CalcTrust();

						System.Console.WriteLine(e.QuoteDateTime);
					} else {
						CalcTrust();
					}
				} else {
					if (__bSaveFile && cTimeSpan > __cStopTime) { //收盤存檔
						__bSaveFile = false;
						__cList.Add(__cTrust);

						Save();
						System.Console.WriteLine("Save trust to file...");
					}

					if (__cList.Count > 0 && (cTimeSpan > __cClearTime && cTimeSpan < __cStartTime)) {
						__bSaveFile = true;
						__cList.Clear();
						System.Console.WriteLine("Clear trust structure...");
					}
				}

				lock (__oLock) {
					__bFlag = false;
				}
			}
		}

		private void CalcTrust() {
			if (__cTrust.time > DateTime.MinValue) {
				DOMPrice[] cASKs = Bars.DOM.Ask;
				DOMPrice[] cBIDs = Bars.DOM.Bid;

				int iLength = cASKs.Length;
				for (int i = 0; i < iLength; i++) {
					__cTrust.askV += cASKs[i].Size;
					__cTrust.bidV += cBIDs[i].Size;
				}
				__cTrust.diffV = __cTrust.bidV - __cTrust.askV;
			}
		}
		
		private void Save() {
			if (isSaveBusy) {
				return;
			}
			isSaveBusy = true;
			StringBuilder cBuilder = new StringBuilder(1024 * 32);

			int iCount = __cList.Count;
			for (int i = 1; i < iCount; i++) {
				__Trust cTrust = __cList[i];
				cBuilder.Append(cTrust.time.ToString("yyyy/MM/dd HH:mm:ss")).Append(",").Append(cTrust.bidV).Append(",").Append(cTrust.askV).Append(",").Append(cTrust.diffV).AppendLine();
			}

			//File.WriteAllText("abc.txt", cBuilder.ToString(), Encoding.UTF8);
			File.AppendAllText("abc.txt", cBuilder.ToString(), Encoding.UTF8);
			isSaveBusy = false;
		}
	}
}