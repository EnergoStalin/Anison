using Anison.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMPLib;

namespace Anison
{
    static class AnisonWMP
    {
        private static WindowsMediaPlayer wmps = null;
        public static int Volume
        {
            get
            {
                return wmps.settings.volume;
            }
            set
            {
                wmps.settings.volume = value;
                Settings.Default.SavedVolume = value;
            }
        }
        public static string URL { get { return wmps?.URL; } set { if(wmps != null) wmps.URL = value; } }
        static AnisonWMP()
        {
            Init();
        }
        public static void Init()
        {
            if (wmps != null) return;
            try
            {
                wmps = new WindowsMediaPlayer();
                wmps.settings.volume = Settings.Default.SavedVolume;
                URL = string.Format(Settings.Default.AnisonURLTemplate, Settings.Default.PrefferedBitrate);
            }
            catch (Exception ex)
            {
                wmps = null;
                AnisonLogger.WLog($"WMP init failed {ex.Message}", AnisonLogger.LogLvl.DEBUG);
            }
        }
        public static void Play()
        {
            Init();
            Stop();
            wmps?.controls.play();
        }
        /// <summary>
        /// Restart player with new url created from <see cref="Settings.Default.AnisonURLTemaplate"/> and <see cref="Settings.Default.PrefferedBitrate"/>
        /// </summary>
        public static void Restart()
        {
            Stop();
            URL = string.Format(Settings.Default.AnisonURLTemplate, Settings.Default.PrefferedBitrate);
            Play();
        }
        public static void Stop()
        {
            Init();
            wmps?.controls.stop();
        }
        public static void Close()
        {
            wmps?.close();
        }
    }
}
