using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Anison
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static string? _rootPath;
		public static string RootPath => GetRoot();
		private static string GetRoot()
		{
			if(_rootPath != default) return _rootPath;

			if (
				Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower() == Environment.CurrentDirectory.ToLower() ||
				Environment.GetFolderPath(Environment.SpecialFolder.Startup).ToLower() == Environment.CurrentDirectory.ToLower()
			)
			{
				var processName = Path.GetFileName(Process.GetCurrentProcess().ProcessName);
				var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
				var path = new FileInfo($"{Path.Combine(startupFolder, processName)}.lnk");

				_rootPath = Path.GetDirectoryName(path.LinkTarget!)!;
			}
			else _rootPath = Environment.CurrentDirectory;

			return _rootPath;
		}

		protected override void OnActivated(EventArgs e)
		{
			
		}
	}
}
