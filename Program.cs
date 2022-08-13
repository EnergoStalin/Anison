using Anison.Properties;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;

namespace Anison
{
	static class Program
	{
		private static string _RootPath = null;
		public static string RootPath
		{
			get
			{
				if (_RootPath == null)
					_RootPath = GetRoot();
				return _RootPath;
			}
			private set { }
		}
		private static string GetRoot()
		{
			if (
				Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower() == Environment.CurrentDirectory.ToLower() ||
				Environment.GetFolderPath(Environment.SpecialFolder.Startup).ToLower() == Environment.CurrentDirectory.ToLower()
			)
			{
				return Path.GetDirectoryName(Startup.GetLnkTarget(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), Path.GetFileName(Process.GetCurrentProcess().ProcessName) + ".lnk")));
			}
			else return Environment.CurrentDirectory;
		}
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += OnApplicationExit;

			Application.Run(new ContextMenu());
		}
		static void OnApplicationExit(object sender, EventArgs args)
		{
			Settings.Default.Save();
		}
	}
}
