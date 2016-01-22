using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Zeghs.Data;
using Zeghs.Events;

namespace Zeghs.Services {
	/// <summary>
	///   即時報價服務抽象類別(所有的即時報價資訊源都需要實作這個抽象類別)
	/// </summary>
	public abstract class AbstractQuoteService : IDisposable {
		/// <summary>
		///   歷史Tick回補完畢後所觸發的事件
		/// </summary>
		public event EventHandler<QuoteComplementCompletedEvent> onComplementCompleted = null;

		/// <summary>
		///   即時報價服務與遠端伺服器斷線後所觸發的事件
		/// </summary>
		public event EventHandler<QuoteDisconnectEvent> onDisconnected = null;

		/// <summary>
		///   即時報價服務登入完成後所觸發的事件
		/// </summary>
		public event EventHandler onLoginCompleted = null;

		/// <summary>
		///   即時報價服務接收到即時公告所觸發的事件
		/// </summary>
		public event EventHandler<QuoteNoticeEvent> onNotice = null;

		/// <summary>
		///   即時報價服務接收到即時成交Tick所觸發的事件
		/// </summary>
		public event EventHandler<QuoteEvent> onQuote = null;

		/// <summary>
		///   即時報價服務接收到清盤重置指令後所觸發的事件
		/// </summary>
		public event EventHandler<QuoteResetEvent> onReset = null;

		/// <summary>
		///   即時伺服器傳送系統時間所觸發的事件
		/// </summary>
		public event EventHandler<QuoteDateTimeEvent> onQuoteDateTime = null;

		/// <summary>
		///   訂閱完成後所觸發的事件
		/// </summary>
		public event EventHandler<QuoteComplementCompletedEvent> onSubscribeCompleted = null;

		private bool __bDisposed = false;

		/// <summary>
		///   [取得/設定] 報價元件名稱
		/// </summary>
		public string DataSource {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 交易所簡稱
		/// </summary>
		public string ExchangeName {
			get;
			set;
		}

		/// <summary>
		///   [取得] 是否登入成功
		/// </summary>
		public bool IsLogin {
			get;
			protected set;
		}

		/// <summary>
		///   [取得/設定] 使用者密碼
		/// </summary>
		public string Password {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 遠端伺服器IP位址
		/// </summary>
		public string RemoteIP {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 遠端伺服器Port
		/// </summary>
		public int RemotePort {
			get;
			set;
		}

		/// <summary>
		///   [取得] 報價資訊儲存媒體
		/// </summary>
		[JsonIgnore]
		public abstract IQuoteStorage Storage {
			get;
		}

		/// <summary>
		///   [取得] 最後交易日期
		/// </summary>
		[JsonIgnore]
		public abstract DateTime TradeDate {
			get;
		}

		/// <summary>
		///   [取得/設定] 更新時間
		/// </summary>
		public DateTime UpdateTime {
			get;
			set;
		}
		
		/// <summary>
		///   [取得/設定] 使用者帳號
		/// </summary>
		public string UserId {
			get;
			set;
		}

		/// <summary>
		///   新增報價資訊訂閱
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public abstract void AddSubscribe(string symbolId);

		/// <summary>
		///   新增報價資訊訂閱(可多商品訂閱)
		/// </summary>
		/// <param name="symbolList">商品代號列表</param>
		public abstract void AddSubscribe(List<string> symbolList);

		/// <summary>
		///   回補即時報價今天的所有歷史Tick
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public abstract void Complement(string symbolId);

		/// <summary>
		///   讀取即時報價服務的設定
		/// </summary>
		public abstract void Load();

		/// <summary>
		///   登入遠端伺服器
		/// </summary>
		/// <returns>返回值:true=登入程序已成功(等候onLogin事件通知), false=登入程序失敗</returns>
		public abstract bool Login();
		
		/// <summary>
		///   登出遠端伺服器(會將遠端所有連線都終止)
		/// </summary>
		public abstract void Logout();
		
		/// <summary>
		///   取消報價資訊訂閱
		/// </summary>
		/// <param name="symbolId">商品代號</param>
		public abstract void RemoveSubscribe(string symbolId);

		/// <summary>
		///   儲存即時報價服務的設定
		/// </summary>
		public abstract void Save();

		/// <summary>
		///   更新商品代號資訊
		/// </summary>
		public abstract void SymbolUpdate();

		/// <summary>
		///   釋放報價服務的所有資源
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   釋放報價服務的所有資源(繼承後可複寫方法)
		/// </summary>
		/// <param name="disposing">是否處理受託管的資源</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.__bDisposed) {
				__bDisposed = true;
				
				if (disposing) {
					onQuote = null;
					onReset = null;
					onNotice = null;
					onDisconnected = null;
					onQuoteDateTime = null;
					onLoginCompleted = null;
					onSubscribeCompleted = null;
					onComplementCompleted = null;
				}
			}
		}

		/// <summary>
		///   發送回補完畢的事件通知
		/// </summary>
		/// <param name="e">QuoteComplementEvent類別</param>
		protected void OnComplementCompleted(QuoteComplementCompletedEvent e) {
			if (onComplementCompleted != null) {
				Task.Factory.StartNew(() => {
					onComplementCompleted(this, e);
				});
			}
		}

		/// <summary>
		///   發送與遠端伺服器中斷連線的通知
		/// </summary>
		/// <param name="e">QuoteDisconnectEvent類別</param>
		protected void OnDisconnect(QuoteDisconnectEvent e) {
			if (onDisconnected != null) {
				Task.Factory.StartNew(() => {
					onDisconnected(this, e);
				});
			}
		}

		/// <summary>
		///   發送登入完成的通知
		/// </summary>
		protected void OnLoginCompleted() {
			if (onLoginCompleted != null) {
				Task.Factory.StartNew(() => {
					onLoginCompleted(this, EventArgs.Empty);
				});
			}
		}

		/// <summary>
		///   發送即時公告資訊的通知
		/// </summary>
		/// <param name="e">QuoteNoticeEvent類別</param>
		protected void OnNotice(QuoteNoticeEvent e) {
			if (onNotice != null) {
				Task.Factory.StartNew(() => {
					onNotice(this, e);
				});
			}
		}

		/// <summary>
		///   發送即時成交Tick的通知
		/// </summary>
		/// <param name="e">QuoteEvent類別</param>
		protected void OnQuote(QuoteEvent e) {
			if (onQuote != null) {
				Task.Factory.StartNew(() => {
					onQuote(this, e);
				});
			}
		}

		/// <summary>
		///   發送清盤重置的通知
		/// </summary>
		/// <param name="e">QuoteResetEvent類別</param>
		protected void OnReset(QuoteResetEvent e) {
			if (onReset != null) {
				Task.Factory.StartNew(() => {
					onReset(this, e);
				});
			}
		}

		/// <summary>
		///   發送由報價伺服器傳送來的系統時間通知
		/// </summary>
		/// <param name="dataSource">即時報價來源名稱</param>
		/// <param name="quoteDateTime">報價日期時間資訊</param>
		protected void OnQuoteDateTime(string dataSource, DateTime quoteDateTime) {
			if (onQuoteDateTime != null) {
				Task.Factory.StartNew(() => {
					onQuoteDateTime(this, new QuoteDateTimeEvent(dataSource, quoteDateTime));
				});
			}
		}

		/// <summary>
		///   發送訂閱完畢的事件通知
		/// </summary>
		/// <param name="e">QuoteComplementEvent類別</param>
		protected void OnSubscribeCompleted(QuoteComplementCompletedEvent e) {
			if (onSubscribeCompleted != null) {
				Task.Factory.StartNew(() => {
					onSubscribeCompleted(this, e);
				});
			}
		}
	}
}