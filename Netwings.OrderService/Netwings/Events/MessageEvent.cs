using System;

namespace Netwings.Event {
	public sealed class MessageEvent : EventArgs {
		public string Message {
			get;
			set;
		}

		public byte[] Buffer {
			get;
			set;
		}

		public int Length {
			get;
			set;
		}
	}
}