using System.Diagnostics;

namespace Zeghs.Utils {
	internal sealed class LiveUpdateUtil {
		private static string __sLiveUpdateUrl = "http://www.zeghs.com/updates/{0}/update.zip";

		internal static string GetUpdateUrl(FileVersionInfo version) {
			return string.Format(__sLiveUpdateUrl, version.ProductVersion);
		}
	}
}