using System.Net;
using System.Web;
using System.Text;

namespace Zeghs.Utils {
        /// <summary>
        ///     參數類別(將要加入Http Header的字串陣列轉換至HttpWebRequest類別內的Header)
        /// </summary>
        public sealed class Parameter {
                /// <summary>
                ///    加入檔頭參數
                /// </summary>
                /// <param name="Request">HttpWebRequest類別</param>
                /// <param name="items">string陣列</param>
                public static void AddHeader(HttpWebRequest Request, string[] items) {
                        if (items != null) {
                                string[] sArray = null;
                                
                                int iCount = items.Length;
                                for (int i = 0; i < iCount; i++) {
                                        sArray = items[i].Split(':');
                                        Request.Headers.Add(sArray[0], sArray[1]);
                                }
                        }
                }
	}
}