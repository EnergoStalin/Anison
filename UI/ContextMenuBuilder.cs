using Anison.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anison
{
    partial class ContextMenu
    {
        private NotifyIcon TrayIcon;
        ContextMenuStrip Menu;
        ToolStripMenuItem onAir = new ToolStripMenuItem();
        private void InitializeComponent()
        {
            Menu = new ContextMenuStrip();

            onAir.DropDownItems.Add("Anison.fm", null, GoToAnisonSite);
            onAir.DropDownItems.Add("Osu!Search", null, SearchSongInOsu);
            onAir.DropDownItems.Add("Google Search", null, SearchSongInGoogle);
            onAir.DropDownItems.Add("Search Youtube", null, SearchSongInYoutube);

            ToolStripMenuItem player = new ToolStripMenuItem("Player");
            ToolStripMenuItem volume = new ToolStripMenuItem("Volume");
            ToolStripMenuItem bitrate = new ToolStripMenuItem("Bitrate");
            ToolStripMenuItem log = new ToolStripMenuItem("Log");
            ToolStripMenuItem settings = new ToolStripMenuItem("Settings");
            ToolStripMenuItem wlog = new ToolStripMenuItem("Write Log")
            {
                Checked = Settings.Default.WriteLog,
                CheckOnClick = true
            };
            wlog.CheckedChanged += ToggleLog;
            log.DropDownItems.Add("Show Log", null, ShowLog);
            log.DropDownItems.Add(new ToolStripMenuItem("Exclude Chop", null, ToggleDropChop) { CheckOnClick = true, Checked = Settings.Default.ExcludeChop });
            log.DropDownItems.Add(new ToolStripMenuItem("Debug info", null, ToggleLogDebugInfo) { CheckOnClick = true, Checked = Settings.Default.LogDebugInfo });
            log.DropDownItems.Add(wlog);
            //Create bitrate properties
            foreach(var rate in Settings.Default.AvalibleBitrates.Split(','))
            {
                bitrate.DropDownItems.Add(new ToolStripMenuItem(rate, null, ChangeBitrate) { Checked = rate == Settings.Default.PrefferedBitrate });
            }
            player.DropDownItems.Add("Play", null, PlayRadio);
            player.DropDownItems.Add(bitrate);
            player.DropDownItems.Add("Stop", null, StopRadio);
            //Create volume properties
            for (float i = 0f; i < 1f;)
            {
                if (i < 0.3f) i += 0.05f; else i += 0.1f;
                i = (float)Math.Round(i, 2);

                volume.DropDownItems.Add(new ToolStripMenuItem(i.ToString(), null, ChangeVolume) { Checked = Settings.Default.SavedVolume == i });
            }
            settings.DropDownItems.Add(new ToolStripMenuItem("Run At Startup", null, ToggleRunAtStartup) { CheckOnClick = true, Checked = Settings.Default.RunAtStartup });

            Menu.Items.Add(onAir);
            Menu.Items.Add(player);
            Menu.Items.Add(volume);
            Menu.Items.Add(log);
            Menu.Items.Add(settings);
            Menu.Items.Add("Exit", null, Exit);

            TrayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                ContextMenuStrip = Menu,
                Visible = true
            };
        }
    }
}
