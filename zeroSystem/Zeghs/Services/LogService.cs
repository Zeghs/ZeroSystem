using System;
using System.Timers;
using System.Collections.Generic;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Zeghs.Data;

namespace Zeghs.Services {
	internal sealed class LogService : IDisposable {
		private const int MAX_LOGDATA_COUNT = 1024;

		private sealed class LimitMemoryAppender : AppenderSkeleton {
			internal event EventHandler onLogEvent = null;

			private LimitedBoundList<LogData> __cLogs = null;

			internal LimitMemoryAppender(LimitedBoundList<LogData> logs) {
				__cLogs = logs;
				__cLogs.SetReduce(MAX_LOGDATA_COUNT, 16);
			}

			protected override void Append(LoggingEvent loggingEvent) {
				LogData cLogData = new LogData();
				cLogData.Level = loggingEvent.Level.DisplayName;
				cLogData.Message = loggingEvent.RenderedMessage;
				cLogData.LogTime = loggingEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");

				__cLogs.Add(cLogData);

				if (onLogEvent != null) {
					onLogEvent(this, EventArgs.Empty);
				}
			}

			protected override void OnClose() {
				base.OnClose();

				onLogEvent = null;
				__cLogs = null;
			}
		}

		internal event EventHandler onLogEvent = null;

		private bool __bBusy = false;
		private bool __bDisposed = false;
		private Timer __cTimer = null;
		private LimitedBoundList<LogData> __cLogs = null;
		private LimitMemoryAppender __cLimitMemoryAppender = null;

		private object __oLock = new object();

		internal LimitedBoundList<LogData> Logs {
			get {
				return __cLogs;
			}
		}

		internal LogService() {
			__cLogs = new LimitedBoundList<LogData>(MAX_LOGDATA_COUNT);

			__cTimer = new Timer();
			__cTimer.Elapsed += Timer_onElapsed;
			__cTimer.AutoReset = false;
			__cTimer.Interval = 500;

			Hierarchy cRepository = LogManager.GetRepository() as Hierarchy;
			cRepository.ConfigurationChanged += Repository_ConfigurationChanged;

			CreateLogger();  //建立MemoryLogger
		}

		/// <summary>
		///   釋放腳本資源
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void CreateLogger() {
			Hierarchy cRepository = LogManager.GetRepository() as Hierarchy;
			IAppender cAppender = cRepository.Root.GetAppender("memoryOut");

			if (cAppender == null) {
				__cLimitMemoryAppender = new LimitMemoryAppender(__cLogs);
				__cLimitMemoryAppender.Name = "memoryOut";
				__cLimitMemoryAppender.onLogEvent += LimitMemoryAppender_onLogEvent;

				cRepository.Root.AddAppender(__cLimitMemoryAppender);
			}
		}

		private void Dispose(bool disposing) {
			if (!__bDisposed) {
				__bDisposed = true;
				if (disposing) {
					onLogEvent = null;

					Hierarchy cRepository = LogManager.GetRepository() as Hierarchy;
					cRepository.ConfigurationChanged -= Repository_ConfigurationChanged;
					cRepository.Shutdown();

					__cTimer.Dispose();
					__cLogs.Clear();
				}
			}
		}

		private void LimitMemoryAppender_onLogEvent(object sender, EventArgs e) {
			__cTimer.Start();
		}

		private void Repository_ConfigurationChanged(object sender, EventArgs e) {
			CreateLogger();
		}

		private void Timer_onElapsed(object sender, ElapsedEventArgs e) {
			if (onLogEvent != null) {
				bool bBusy = false;
				lock (__oLock) {
					bBusy = __bBusy;
					if (!bBusy) {
						__bBusy = true;
					}
				}

				if (!bBusy) {
					onLogEvent(this, EventArgs.Empty);

					lock (__oLock) {
						__bBusy = false;
					}
				}
			}
		}
	}
}