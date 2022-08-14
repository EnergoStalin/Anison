using System;
using System.IO;

namespace Anison.Logging;

public class Logger : ILogger {
	private FileInfo info;
	public Logger(string path) {
		info = new FileInfo(path);
		info.Create();
	}
	public void WriteInfo(string msg) {
		using var sw = info.AppendText();
		sw.Write(DateTime.Now.ToString("[dd.MM.yyyy] [HH:mm:ss] "));
		sw.WriteLine(msg);
	}
	public void WriteError(string msg, Exception? ex = default) {
		WriteInfo(msg);
		
		if(ex != default)
			WriteInfo(ex.ToString());
	}
}