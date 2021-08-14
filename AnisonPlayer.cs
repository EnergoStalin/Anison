using Anison.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Anison
{
    static class AnisonPlayer
    {
        
        private static MediaFoundationReader reader = null;
        private static WaveOutEvent playback = new WaveOutEvent();
        public static float Volume
        {
            get
            {
                return playback.Volume;
            }
            set
            {
                playback.Volume = value;
                Settings.Default.SavedVolume = value;
            }
        }
        /// <summary>
        /// Initing url reader and reset playback with new soruce
        /// </summary>
        public static string URL
        {
            get
            {
                return reader?.ToString();
            }
            set
            {   
                playback?.Dispose();
                reader?.Close();
                reader = new MediaFoundationReader(value);
                playback?.Init(reader);
            }
        }
        static AnisonPlayer()
        {
            Init();
        }
        public static bool Init()
        {
            if (reader != null) return true;
            try
            {
                playback = new WaveOutEvent() { Volume = Settings.Default.SavedVolume > 1f ? Settings.Default.SavedVolume / 100 : Settings.Default.SavedVolume };
                URL = string.Format(Settings.Default.AnisonURLTemplate, Settings.Default.PrefferedBitrate);
                return true;
            }
            catch (Exception ex)
            {
                playback?.Dispose();
                playback = null;
                AnisonLogger.WLog($"WMP init failed {ex.Message}", AnisonLogger.LogLvl.DEBUG);
                return false;
            }
        }
        public static void Play()
        {
            if (!Init()) return;

            playback.Stop();
            reader.Seek(0, System.IO.SeekOrigin.End);
            playback.Play();
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
            playback?.Dispose();
        }
        public static void Close()
        {
            playback?.Dispose();
            reader?.Close();
        }
    }
}
