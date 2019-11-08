using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmTrustViewer : Form {
		private bool __bBusy = false;
		private int __iDecimalPoint = 0;
		private string __sSymbolId = null;
		private string __sDataSource = null;
		private IQuoteStorage __cStorage = null;
		private System.Timers.Timer __cTimer = null;

		private object __oLock = new object();

		internal frmTrustViewer(string dataSource, string symbolId, int decimalPoint) {
			__sDataSource = dataSource;
			__sSymbolId = symbolId;
			__iDecimalPoint = decimalPoint;

			InitializeComponent();
			InitializeSourceGrid();
		}

		private void RefreshGrid() {
			if (!this.IsDisposed) {
				if (this.dataGrid.InvokeRequired) {
					this.dataGrid.BeginInvoke((MethodInvoker) delegate {
						source.Refresh();
					});
				} else {
					source.Refresh();
				}
			}
		}

		private void TrustUpdate() {
			IQuote cQuote = __cStorage.GetQuote(__sSymbolId);
			
			DOMPrice[] cAsks = cQuote.DOM.Ask;
			DOMPrice[] cBids = cQuote.DOM.Bid;

			if (cAsks != null && cBids != null) {
				int iLength = cAsks.Length;
				for (int i = 0; i < iLength; i++) {
					_DOMInfo cDOM = null;
					if (i < source.Count) {
						cDOM = source.GetItemAt(i);
					} else {
						cDOM = new _DOMInfo(__iDecimalPoint);
						source.Add(cDOM);
					}

					cDOM.SetDOM(cAsks[i], cBids[i]);
				}
			}
		}

		private void frmTrustViewer_Load(object sender, System.EventArgs e) {
			this.Text = string.Format("[{0}] {1}", __sSymbolId, this.Text);
		}

		private void frmTrustViewer_Shown(object sender, System.EventArgs e) {
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(__sDataSource);
			if (cService != null) {
				__cStorage = cService.Storage;

				IQuote cQuote = cService.Storage.GetQuote(__sSymbolId);
				if (cQuote != null) {
					__cTimer = new System.Timers.Timer(100);
					__cTimer.AutoReset = false;
					__cTimer.Elapsed += Timer_onElapsed;

					cService.onQuote += QuoteService_onQuote;

					TrustUpdate();
					RefreshGrid();
				}
			}

			this.Focus();
		}

		private void QuoteService_onQuote(object sender, QuoteEvent e) {
			if (__sSymbolId.Equals(e.Quote.SymbolId)) {
				__cTimer.Start();
			}
		}
		
		private void Timer_onElapsed(object sender, System.Timers.ElapsedEventArgs e) {
			bool bBusy = false;
			lock (__oLock) {
				bBusy = __bBusy;
				if (!bBusy) {
					__bBusy = true;
				}
			}

			if (!bBusy) {
				TrustUpdate();  //更新委託價量
				RefreshGrid();

				lock (__oLock) {
					__bBusy = false;
				}
			}
		}
	}
}