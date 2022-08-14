using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;


namespace Anison;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string RootPath => GetRoot();
    private static string? _rootPath;
    private static string GetRoot()
    {
        if (_rootPath != default) return _rootPath;

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

    private Player? _player;
    private State? _state;

    protected override void OnStartup(StartupEventArgs e)
    {
        var logger = new Logging.Logger("./1.log");
        _player = new Player("https://pool.anison.fm/AniSonFM(320)", new Logging.Logger("./player.log"));
        _state = new State(new Logging.Logger("./songs.log"));

        base.OnStartup(e);
    }

	protected override void OnExit(ExitEventArgs e)
	{
        _state?.Dispose();
		base.OnExit(e);
	}
}
