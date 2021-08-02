using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Anison
{
    static class Startup
    {
        private static WshShell Shell = new WshShell();
        public static void CreateLnk(string path, string target, string desc = null)
        {
            IWshShortcut shortcut = (IWshShortcut)Shell.CreateShortcut(path);
            shortcut.Description = desc;
            shortcut.TargetPath = target;
            shortcut.Save();
        }
        public static string GetLnkTarget(string path)
        {
            return ((IWshShortcut)Shell.CreateShortcut(path)).TargetPath;
        }
        public static void SetRunAtStartup(bool choice)
        {
            var appName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), Path.GetFileNameWithoutExtension(appName) + ".lnk");
            if (choice)
            {
                CreateLnk(shortcutPath, Path.Combine(Environment.CurrentDirectory, appName), "Anison.fm logger startup.");
            }
            else
            {
                System.IO.File.Delete(shortcutPath);
            }
        }
    }
}
