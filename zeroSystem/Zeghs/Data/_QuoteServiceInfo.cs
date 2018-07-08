using System;
using Zeghs.Informations;

namespace Zeghs.Data {
	internal sealed class _QuoteServiceInfo {
		private static string Format(string format, double value) {
			string sRet = string.Empty;
			char chUnit = (value >= 1048576) ? 'M' : (value >= 1024) ? 'K' : ' ';
			value = (chUnit == 'M') ? value / 1048576 : (chUnit == 'K') ? value / 1024 : value;
			
			return string.Format(format, value.ToString("N2"), chUnit);
		}

		private long __lPacketCount = 0;
		private long __lPrevPacketCount = 0;
		private QuoteServiceInformation __cQuoteServiceInfo = null;

		public string Company {
			get {
				return __cQuoteServiceInfo.Company;
			}
		}

		public string DataSource {
			get {
				return __cQuoteServiceInfo.DataSource;
			}
		}

		public string Description {
			get {
				return __cQuoteServiceInfo.Description;
			}
		}

		public bool Enabled {
			get {
				return __cQuoteServiceInfo.Enabled;
			}
		}

		public string FileVersion {
			get {
				return __cQuoteServiceInfo.FileVersion;
			}
		}

		public string Name {
			get {
				return __cQuoteServiceInfo.Name;
			}
		}

		public string PacketCount {
			get {
				return Format("{0}{1}", __lPacketCount);
			}
		}

		public string PacketCountPerSeconds {
			get {
				return Format("{0}{1}/s", __lPacketCount - __lPrevPacketCount);
			}
		}

		public string ProductVersion {
			get {
				return __cQuoteServiceInfo.ProductVersion;
			}
		}

		internal _QuoteServiceInfo(QuoteServiceInformation quoteServiceInfo) {
			__cQuoteServiceInfo = quoteServiceInfo;
		}

		internal QuoteServiceInformation GetInformation() {
			return __cQuoteServiceInfo;
		}

		internal void SetPacketCount(long packetCount) {
			__lPrevPacketCount = __lPacketCount;
			__lPacketCount = packetCount;
		}
	}
}