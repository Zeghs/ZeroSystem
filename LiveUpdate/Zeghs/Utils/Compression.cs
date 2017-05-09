using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Zeghs.Events;

namespace Zeghs.Utils {
	public sealed class Compression {
		public event ExtractProgressHandler onExtractProgress = null;

		public void Dispose() {
			onExtractProgress = null;
		}

		public void Extract(string zipFile, string password, string outFolder) {
			ZipFile cZipFile = null;

			try {
				FileStream cStream = File.OpenRead(zipFile);
				cZipFile = new ZipFile(cStream);

				if (!String.IsNullOrEmpty(password)) {
					cZipFile.Password = password;
				}

				int iCurrent = 0;
				int iTotals = (int) cZipFile.Count;
				foreach (ZipEntry zipEntry in cZipFile) {
					if (!zipEntry.IsFile) {
						continue;
					}

					string entryFileName = zipEntry.Name;
					string fullZipToPath = Path.Combine(outFolder, entryFileName);
					string directoryName = Path.GetDirectoryName(fullZipToPath);
					if (directoryName.Length > 0) {
						Directory.CreateDirectory(directoryName);
					}

					byte[] arrBuffer = new byte[4096];
					Stream cZipStream = cZipFile.GetInputStream(zipEntry);

					using (FileStream streamWriter = File.Create(fullZipToPath)) {
						StreamUtils.Copy(cZipStream, streamWriter, arrBuffer);
					}

					++iCurrent;

					if (this.onExtractProgress != null) {
						this.onExtractProgress(this, new ProgressEvent(entryFileName, iCurrent, iTotals));
					}
				}
			} finally {
				if (cZipFile != null) {
					cZipFile.IsStreamOwner = true;
					cZipFile.Close();
				}
			}
		}
	}
}