using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using log4net;
using Zeghs.Utils;

namespace Zeghs.Web {
        /// <summary>
        ///    處理Response串流資料的類別(有針對GZip壓縮做特別處理)
        /// </summary>
        public sealed class ZReader {
                private static readonly ILog logger = LogManager.GetLogger(typeof(ZReader));

		private string __sContentType = null;
		private HttpWebResponse __cResponse = null;  //HttpWebResponse類別

		/// <summary>
		///    [取得/設定]目前編碼模式(預設:utf-8)
		/// </summary>
		public string Charset {
			get;
			set;
		}

		/// <summary>
		///    [取得/設定] 內容格式
		/// </summary>
		public string ContentType {
			get {
				return __sContentType;
			}
		}

		public ZReader(HttpWebResponse Response) {
			this.Charset = "utf-8";

			if (Response != null) {
				if (Response.StatusCode == HttpStatusCode.OK) {
					__cResponse = Response;
					__sContentType = __cResponse.ContentType;
				}
			}
                }

                /// <summary>
                ///    讀取Response資料
                /// </summary>
                /// <returns>返回值：ZBuffer類別(null=讀取失敗)</returns>
                public ZBuffer Read() {
			if (__cResponse == null) {
				return null;
			}

                        try {
                                int iLength = 0;
                                Stream cStream = __cResponse.GetResponseStream();
                                if (__cResponse.ContentEncoding.Equals("gzip")) {
                                        if (logger.IsDebugEnabled) logger.Debug("封包資料有使用gzip壓縮編碼，建立GZip解碼物件...");
                                        cStream = new GZipStream(cStream, CompressionMode.Decompress);
                                }

                                //建立緩衝區
				byte[] cTemps = new byte[8192];
				MemoryStream cMemoryStream = new MemoryStream(32 * 1024);
                                
                                do {
                                        iLength = cStream.Read(cTemps, 0, 8192);
					cMemoryStream.Write(cTemps, 0, iLength);
                                } while (iLength != 0);

				cStream.Close();  //關閉資料流
				cStream.Dispose(); //釋放資源

				int iSize = (int)cMemoryStream.Length;
				ZBuffer cBuffer = new ZBuffer(iSize);
				cBuffer.Length = iSize;

				cMemoryStream.Position = 0;
				cMemoryStream.Read(cBuffer.Data, 0, iSize);
				cMemoryStream.Close();
				cMemoryStream.Dispose();

				__cResponse = null;  //釋放資源
				
                                if (logger.IsDebugEnabled) logger.DebugFormat("[Response]資料資訊  Length:{0}", cBuffer.Length);
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
		public string ReadText() {
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