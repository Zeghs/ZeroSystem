using System;

namespace Zeghs.Events {
	/// <summary>
	///   報價服務斷線事件(報價服務斷線時，會觸發此事件通知)
	/// </summary>
	public sealed class QuoteDisconnectEvent : EventArgs {
		private int __iRemotePort = 0;      //遠端Port
		private string __sRemoteIP = null;  //遠端IP位址
		private string __sDataSource = null;

		/// <summary>
		///   [取得] 報價元件名稱
		/// </summary>
		public string DataSource {
			get {
				return __sDataSource;
			}
		}

		/// <summary>
		///   [取得]遠端主機位址
		/// </summary>
		public string RemoteIP {
			get {
				return __sRemoteIP;
			}
		}

		/// <summary>
		///    [取得]遠端主機Port
		/// </summary>
		public int RemotePort {
			get {
				return __iRemotePort;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="dataSource">報價元件名稱</param>
		/// <param name="remoteIP">遠端伺服器IP</param>
		/// <param name="remotePort">遠端伺服器連接Port</param>
		public QuoteDisconnectEvent(string dataSource, string remoteIP, int remotePort) {
			__sDataSource = dataSource;
			__sRemoteIP = remoteIP;
			__iRemotePort = remotePort;
		}
	}
}