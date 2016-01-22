using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.Collections.Generic;
using log4net;
using Zeghs.Events;

namespace Zeghs.IO {
	internal sealed class ZRequest {
		private const int MAX_WRITE_BUFFER_SIZE = 8192;
		
		private static readonly ILog logger = LogManager.GetLogger(typeof(ZRequest));
                private static readonly string Content_Type = "application/x-www-form-urlencoded";

		private HttpWebRequest __cRequest = null;

                /// <summary>
                ///    [取得/設定]目前編碼模式(預設:UTF-8)
                /// </summary>
		internal string Charset {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]CookieContainer集合
                /// </summary>
		internal CookieContainer CookieContainer {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]ContentType
                /// </summary>
		internal string ContentType {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]是否要持續連線
                /// </summary>
		internal bool KeepAlive {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]提交方式("GET", "POST")
                /// </summary>
		internal string Method {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]Post參數
                /// </summary>
		internal string Parameters {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]參考位址
                /// </summary>
		internal string Referer {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]HttpWebResponse類別
                /// </summary>
		internal HttpWebResponse Response {
                        get {
				HttpWebResponse cResponse = null;
				
				try {
					cResponse = (HttpWebResponse) __cRequest.GetResponse();
				} catch (Exception __errExcep) {
					logger.ErrorFormat("ZRequest.Response: url={0}\r\nparameters={1}\r\nmessage={2}\r\n{3}", this.Url, this.Parameters, __errExcep.Message, __errExcep.StackTrace);
				}
				return cResponse;
                        }
                }

                /// <summary>
                ///    [取得/設定]請求逾時的時間(單位：1/1000s  預設：20000)
                /// </summary>
		internal int RequestTimeout {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]Request Url字串
                /// </summary>
		internal string Url {
                        get;
                        set;
                }

                internal ZRequest() {
			this.Charset = "utf-8";
			this.KeepAlive = false;
			this.RequestTimeout = 150000;  //預設Timeout時間 150s
                }

		internal int Request() {
			try {
				__cRequest = (HttpWebRequest) WebRequest.Create(this.Url);
			} catch (Exception __errExcep1) {
				logger.ErrorFormat("ZRequest.Request: url={0}\r\nparameters={1}\r\nmessage={2}\r\n{3}", this.Url, this.Parameters, __errExcep1.Message, __errExcep1.StackTrace);
				return 1;
			}

			//設定HTTP傳輸協定參數資料
			__cRequest.Method = this.Method;
			__cRequest.Referer = this.Referer;
			__cRequest.KeepAlive = this.KeepAlive;

			__cRequest.Accept = "*/*";
			__cRequest.UserAgent = "Mozilla/4.0";
			__cRequest.Timeout = this.RequestTimeout;
			__cRequest.ReadWriteTimeout = this.RequestTimeout;
			__cRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			__cRequest.CookieContainer = CookieContainer;

			if (this.Method.Equals("POST")) {
				__cRequest.ContentType = ((this.ContentType == null) ? Content_Type : this.ContentType);

				if (this.Parameters != null) {
					byte[] bArray = Encoding.GetEncoding(Charset).GetBytes(this.Parameters);
					__cRequest.ContentLength = bArray.Length;

					try {
						Stream cStream = __cRequest.GetRequestStream();
						cStream.Write(bArray, 0, bArray.Length);
						cStream.Flush();
						cStream.Close();
						cStream.Dispose();
					} catch (Exception __errExcep2) {
						logger.ErrorFormat("Zrequest.Request: url={0}\r\nparameters={1}\r\nmessage={2}\r\n{3}", this.Url, this.Parameters, __errExcep2.Message, __errExcep2.StackTrace);
						return 2;
					}
				}
			}
			return 0;
		}
        }
}