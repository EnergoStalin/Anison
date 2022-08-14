using System;
using System.IO;

namespace Anison.Logging {
	public class Logger {
		private FileInfo info;
		public Logger(string path) {
			info = new FileInfo(path);
		}
		public void WriteInfo(string msg) {
			using var sw = info.AppendText();
			sw.WriteLine(msg);
		}
		public void WriteError(string msg, Exception? ex = default) {
			using var sw = info.AppendText();
			sw.WriteLine(msg);
			
			if(ex != default)
				sw.WriteLine(ex);
		}
	}
}