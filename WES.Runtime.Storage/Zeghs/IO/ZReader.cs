using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using log4net;

namespace Zeghs.IO {
        /// <summary>
        ///    處理Response串流資料的類別(有針對GZip壓縮做特別處理)
        /// </summary>
        internal sealed class ZReader {
		private const int ERROR_HTTP_RESPONSE_FAIL = -1024;

                private static readonly ILog logger = LogManager.GetLogger(typeof(ZReader));

		private int __iCount = 0;
		private int __iDataSize = 0;
		private int __iResult = ERROR_HTTP_RESPONSE_FAIL;
		private long __lPosition = -1;
		private DateTime __cBeginDate;
		private DateTime __cEndDate;
		private HttpWebResponse __cResponse = null;  //HttpWebResponse類別

		/// <summary>
		///   [取得] 資料起始日期
		/// </summary>
		internal DateTime BeginDate {
			get {
				return __cBeginDate;
			}
		}

		/// <summary>
		///    [取得/設定]目前編碼模式(預設:utf-8)
		/// </summary>
		internal string Charset {
			get;
			set;
		}

		/// <summary>
		///   [取得] 下載的資料區塊個數
		/// </summary>
		internal int Count {
			get {
				return __iCount;
			}
		}

		/// <summary>
		///   [取得] 資料終止日期
		/// </summary>
		internal DateTime EndDate {
			get {
				return __cEndDate;
			}
		}

		/// <summary>
		///   [取得] 請求的資料位置
		/// </summary>
		internal long Position {
			get {
				return __lPosition;
			}
		}

		/// <summary>
		///   [取得] 回傳代碼(0=成功)
		/// </summary>
		internal int Result {
			get {
				return __iResult;
			}
		}

		internal ZReader(HttpWebResponse Response) {
			this.Charset = "utf-8";

			if (Response != null) {
				if (Response.StatusCode == HttpStatusCode.OK) {
					__cResponse = Response;

					Cookie cCookie = null;
					cCookie = __cResponse.Cookies["result"];
					__iResult = int.Parse(cCookie.Value);

					if (__iResult == 0) {
						cCookie = __cResponse.Cookies["position"];
						__lPosition = long.Parse(cCookie.Value);

						cCookie = __cResponse.Cookies["count"];
						__iCount = int.Parse(cCookie.Value);

						cCookie = __cResponse.Cookies["dataSize"];
						__iDataSize = int.Parse(cCookie.Value);

						cCookie = __cResponse.Cookies["beginDate"];
						__cBeginDate = DateTime.ParseExact(cCookie.Value, "yyyyMMdd", null);

						cCookie = __cResponse.Cookies["endDate"];
						__cEndDate = DateTime.ParseExact(cCookie.Value, "yyyyMMdd", null);
					}
				}
			}
                }

                /// <summary>
                ///    讀取Response資料
                /// </summary>
                /// <returns>返回值：ZBuffer類別(null=讀取失敗)</returns>
		internal ZBuffer Read() {
			if (__cResponse == null) {
				return null;
			}

                        try {
                                int iLength = 0;
                                Stream cStream = __cResponse.GetResponseStream();
                                if (__cResponse.ContentEncoding.Equals("gzip")) {
					if (logger.IsDebugEnabled) logger.Debug("ZReader.Read: 封包資料有使用gzip壓縮編碼，建立GZip解碼物件...");
                                        cStream = new GZipStream(cStream, CompressionMode.Decompress);
                                }

                                //建立緩衝區
				byte[] cTemps = new byte[8192];
				ZBuffer cBuffer = new ZBuffer(__iDataSize + 2);
                                
                                do {
                                        iLength = cStream.Read(cTemps, 0, 8192);
					if (iLength > 0) {
						cBuffer.Add(cTemps, 0, iLength);
					}
                                } while (iLength != 0);

				cStream.Close();  //關閉資料流
				cStream.Dispose(); //釋放資源

				__cResponse = null;  //釋放資源

				if (logger.IsDebugEnabled) logger.DebugFormat("ZReader.Read: dataLength={0}", cBuffer.Length);
                                return cBuffer;
                        } catch (System.Exception __errExcep) {
                                logger.ErrorFormat("{0}\r\n{1}", __errExcep.StackTrace, __errExcep.Message);
                                return null;
                        }
                }
		
		/// <summary>
		///    讀取Response資料
		/// </summary>
		/// <returns>返回值：網頁內容</returns>
		internal string ReadText() {
			string sText = string.Empty;
			
			ZBuffer cBuffer = this.Read();
			if (cBuffer != null) {
				sText = Encoding.GetEncoding(this.Charset).GetString(cBuffer.Data,
										     0,
										     cBuffer.Length);
			}
			return sText;
		}
	}
}