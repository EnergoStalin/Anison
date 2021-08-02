using Anison.Properties;
using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Anison
{
    partial class ContextMenu : ApplicationContext
    {
        public ContextMenu()
        {
            InitializeComponent();
            AnisonLogger.SongChanged += OnSongChanged;

            StartLogger();
        }
        void StartLogger()
        {
            (new Thread(AnisonLogger.Log) { IsBackground = true }).Start();
        }
        void OnSongChanged(object sender, EventArgs args)
        {
            onAir.Text = ((AnisonLogger.Song)sender).ToString();
        }
        private void ToggleLogDebugInfo(object sender, EventArgs e)
        {
            Settings.Default.LogDebugInfo = ((ToolStripMenuItem)sender).Checked;
        }
        private void ToggleRunAtStartup(object sender, EventArgs e)
        {
            Settings.Default.RunAtStartup = !Settings.Default.RunAtStartup;
            Startup.SetRunAtStartup(Settings.Default.RunAtStartup);
        }
        private void ShowLog(object sender, EventArgs e)
        {
            Process.Start(AnisonLogger.LogPath);
        }
        private void ChangeVolume(object sender, EventArgs e)
        {
            AnisonWMP.Volume = int.Parse(Helpers.UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender).Text);
        }
        private void ToggleLog(object sender, EventArgs e)
        {
            Settings.Default.WriteLog = !Settings.Default.WriteLog;
        }
        private void GoToAnisonSite(object sender, EventArgs e)
        {
            Process.Start("https://anison.fm");
        }
        private void SearchSongInYoutube(object sender, EventArgs e)
        {
            Searcher.SearchInYoutube(AnisonLogger.CurrentSong.Title);
        }
        private void SearchSongInGoogle(object sender, EventArgs e)
        {
            Searcher.SearchInGoogle(AnisonLogger.CurrentSong.Title);
        }
        private void ToggleDropChop(object sender, EventArgs e)
        {
            AnisonLogger.DropChop = !AnisonLogger.DropChop;
        }
        private void SearchSongInOsu(object sender, EventArgs e)
        {
            Searcher.SearchInOsu(AnisonLogger.CurrentSong.Title);
        }
        private void PlayRadio(object sender, EventArgs e)
        {
            AnisonWMP.Play();
        }
        private void StopRadio(object sender, EventArgs e)
        {
            AnisonWMP.Stop();
        }
        private void ChangeBitrate(object sender, EventArgs e)
        {
            Settings.Default.PrefferedBitrate = Helpers.UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender).Text;
            AnisonWMP.Restart();
        }
        public void Exit(object sender, EventArgs args)
        {
            TrayIcon.Visible = false;
            AnisonWMP.Close();

            Settings.Default.Save();
            Application.Exit();
        }
    }
}
