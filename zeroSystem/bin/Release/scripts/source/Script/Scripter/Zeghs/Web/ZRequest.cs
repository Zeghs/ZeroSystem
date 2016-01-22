using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.Collections.Generic;
using log4net;
using Zeghs.Utils;
using Zeghs.Events;

namespace Zeghs.Web {
        public sealed class ZRequest {
		private static readonly ILog logger = LogManager.GetLogger(typeof(ZRequest));
                private static readonly string Content_Type = "application/x-www-form-urlencoded";

		private Exception __cException = null;
		private HttpWebRequest __cRequest = null;
		private MemoryStream __cMemoryStream = null;

                /// <summary>
                ///    [取得/設定]目前編碼模式(預設:UTF-8)
                /// </summary>
                public string Charset {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]CookieContainer集合
                /// </summary>
                public CookieContainer CookieContainer {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]ContentType
                /// </summary>
                public string ContentType {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]是否使用代理伺服器
                /// </summary>
                public bool IsProxy {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]標頭設定
                /// </summary>
                public string[] Headers {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]是否要持續連線
                /// </summary>
                public bool KeepAlive {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]提交方式("GET", "POST")
                /// </summary>
                public string Method {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]Post參數
                /// </summary>
                public string Parameters {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]參考位址
                /// </summary>
                public string Referer {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得]HttpWebResponse類別
                /// </summary>
                public HttpWebResponse Response {
                        get {
				HttpWebResponse cResponse = null;
				
				try {
					cResponse = (HttpWebResponse) __cRequest.GetResponse();
				} catch (Exception __errExcep) {
					logger.ErrorFormat("Url:{0}\r\nParameters:{1}\r\nMessage:{2}\r\n{3}", this.Url, this.Parameters, __errExcep.Message, __errExcep.StackTrace);
				}
				return cResponse;
                        }
                }

                /// <summary>
                ///    [取得/設定]請求逾時的時間(單位：1/1000s  預設：20000)
                /// </summary>
                public int RequestTimeout {
                        get;
                        set;
                }

                /// <summary>
                ///    [取得/設定]Request Url字串
                /// </summary>
                public string Url {
                        get;
                        set;
                }

                public ZRequest() {
			this.Charset = "utf-8";
			this.KeepAlive = false;
			this.RequestTimeout = 150000;  //預設Timeout時間 150s
                }

		public void Close() {
			if (__cRequest != null) {
				__cRequest.Abort();
				__cRequest = null;
			}

			if (__cMemoryStream != null) {
				__cMemoryStream.Close();
				__cMemoryStream.Dispose();
			}
		}
                
                public int Request() {
                        try {
				__cRequest = WebRequest.Create(this.Url) as HttpWebRequest;
                        } catch (Exception __errExcep1) {
				logger.ErrorFormat("Url:{0}\r\nParameters:{1}\r\nMessage:{2}\r\n{3}", this.Url, this.Parameters, __errExcep1.Message, __errExcep1.StackTrace);
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
                        
                        Parameter.AddHeader(__cRequest, this.Headers);

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
						logger.ErrorFormat("Url:{0}\r\nParameters:{1}\r\nMessage:{2}\r\n{3}", this.Url, this.Parameters, __errExcep2.Message, __errExcep2.StackTrace);
						return 2;
					}
				}
			}
                        return 0;
                }
        }
}