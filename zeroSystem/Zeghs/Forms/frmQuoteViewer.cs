﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Zeghs.Data;
using Zeghs.Events;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmQuoteViewer : frmChildBase {
		private bool __bBusy = false;
		private Queue<QuoteEvent> __cQueue = null;
		private System.Timers.Timer __cTimer = null;
		private HashSet<string> __cDataSources = null;

		private object __oLock = new object();

		internal frmQuoteViewer() {
			__cQueue = new Queue<QuoteEvent>(1024);
			__cDataSources = new HashSet<string>();

			__cTimer = new System.Timers.Timer(200);
			__cTimer.AutoReset = false;
			__cTimer.Elapsed += Timer_onElapsed;

			InitializeComponent();
			InitializeSourceGrid();

			this.ShowTitle = true;
		}

		private void QuoteUpdate(string exchangeName, string dataSource, IQuote quote) {
			string sSymbolName = quote.SymbolName;
			int iIndex = source.IndexOf(sSymbolName);

			_QuoteInfo cQuoteInfo = null;
			double dPrice = quote.RealTick.Price;
			if (iIndex == -1) {
				string sSymbolId = quote.SymbolId;
				if (sSymbolId.Length == 0) {
					return;
				} else {
					cQuoteInfo = new _QuoteInfo(exchangeName, dataSource, sSymbolId, dPrice);
					cQuoteInfo.SymbolName = quote.SymbolName;

					source.Add(cQuoteInfo);
				}
			} else {
				cQuoteInfo = source.GetItemAt(iIndex);
			}

			cQuoteInfo.SetPrice(dPrice);
			cQuoteInfo.SetVolume(quote.RealTick.Volume);
			cQuoteInfo.SetTime(quote.RealTick.Time.TimeOfDay);
		}

		private void RefreshGrid() {
			if (!this.IsDisposed) {
				if (this.dataGrid.InvokeRequired) {
					this.dataGrid.Invoke((MethodInvoker) delegate {
						source.Refresh();
					});
				} else {
					source.Refresh();
				}
			}
		}

		private void RemoveQuoteServices() {
			lock (__cDataSources) {
				foreach (string sDataSource in __cDataSources) {
					AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
					cService.onQuote -= QuoteService_onQuote;
					cService.onSubscribeCompleted -= QuoteService_onSubscribeCompleted;
				}
				__cDataSources.Clear();
			}
		}

		private void frmQuoteViewer_Load(object sender, EventArgs e) {
			QuoteManager.Manager.onQuoteServiceSwitchChanged += QuoteManager_onQuoteServiceSwitchChanged;
		}

		private void frmQuoteViewer_Resize(object sender, EventArgs e) {
			this.dataGrid.Top = 21;
			this.dataGrid.Height = this.ClientSize.Height - this.dataGrid.Top - 1;
		}

		private void QuoteManager_onQuoteServiceSwitchChanged(object sender, QuoteServiceSwitchChangedEvent e) {
			string sDataSource = e.DataSource;
			if (e.IsRunning) {
				bool bNotHave = false;
				lock (__cDataSources) {
					if (!__cDataSources.Contains(sDataSource)) {
						__cDataSources.Add(sDataSource);
						bNotHave = true;
					}
				}

				if (bNotHave) {
					AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(sDataSource);
					cService.onQuote += QuoteService_onQuote;
					cService.onSubscribeCompleted += QuoteService_onSubscribeCompleted;
				}
			} else {
				lock (__cDataSources) {
					__cDataSources.Remove(sDataSource);
				}
			}
		}

		private void QuoteService_onQuote(object sender, QuoteEvent e) {
			lock (__cQueue) {
				__cQueue.Enqueue(e);
			}
			__cTimer.Start();
		}

		private void QuoteService_onSubscribeCompleted(object sender, QuoteComplementCompletedEvent e) {
			AbstractQuoteService cService = QuoteManager.Manager.GetQuoteService(e.DataSource);
			IQuote cQuote = cService.Storage.GetQuote(e.SymbolId);
			if (cQuote != null) {
				QuoteUpdate(e.ExchangeName, e.DataSource, cQuote);  //更新報價
				RefreshGrid();
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
				if (__cQueue.Count > 0) {
					QuoteEvent cQuoteEvent = null;
					while (__cQueue.Count > 0) {
						lock (__cQueue) {
							cQuoteEvent = __cQueue.Dequeue();
						}

						QuoteUpdate(cQuoteEvent.ExchangeName, cQuoteEvent.DataSource, cQuoteEvent.Quote);  //更新報價
					}

					RefreshGrid();
				}

				lock (__oLock) {
					__bBusy = false;
				}
			}
		}
	}
}