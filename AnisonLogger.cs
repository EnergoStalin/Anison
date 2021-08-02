using Anison.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Anison
{
    static class AnisonLogger
    {
        public enum LogLvl
        {
            INFO,
            DEBUG
        }
        public static event EventHandler SongChanged;
        public class Song : EventArgs
        {
            public string Title;
            public TimeSpan Duration;
            public override string ToString()
            {
                return Title;
            }
            public Song(string title, TimeSpan duration)
            {
                Title = title;
                Duration = duration;
            }
            public Song(string title = null)
            {
                Title = "";
                Duration = TimeSpan.Zero;
            }
        }

        private static string RequestURL = "http://anison.fm/status.php?widget=false";
        public static string LogPath { get; set; }
        public static readonly string LogName = "Music.sav";
        public static Song CurrentSong { get; private set; }
        public static bool DropChop {
            get { return _DropChop; }
            set
            {
                Settings.Default.ExcludeChop = value;
                _DropChop = value;
            }
        }

        private static bool _DropChop = true;
        private static TimeSpan ErrorDelay = new TimeSpan(0, 0, 20);
        private static readonly HttpClient client = new HttpClient();

        static AnisonLogger()
        {
            LogPath = Path.Combine(Program.RootPath, LogName);
            DropChop = Settings.Default.ExcludeChop;
            CurrentSong = new Song();
        }
        /// <summary>
        /// Perform http request to anison api endpoint
        /// </summary>
        /// <returns></returns>
        private static Song NowPlaying()
        {
            var getTask = client.GetAsync(RequestURL);
            if (getTask.Exception != null) throw getTask.Exception;
            getTask.Wait();
            if (getTask.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                CurrentSong.Title = "Request Failed";
                CurrentSong.Duration = ErrorDelay;
                return CurrentSong;
            }
            var readTask = getTask.Result.Content.ReadAsStringAsync();
            if (readTask.Exception != null) throw readTask.Exception;
            readTask.Wait();

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(readTask.Result);
            string title = Regex.Replace(json["on_air"], "(?:<[^>]+>)|(?:&#\\d+)", "").Replace("В эфире: ", "").Replace(";", "-");

            CurrentSong.Title = title;
            CurrentSong.Duration = TimeSpan.FromSeconds(int.Parse(json["duration"]));
            return CurrentSong;
        }
        public static void Log()
        {
            while (true)
            {
                try
                {
                    NowPlaying();
                    if (Settings.Default.WriteLog && !(DropChop && CurrentSong.Title.Contains("Anison.FM")))
                        WriteLog(CurrentSong);
                }
                catch (AggregateException)
                {
                    CurrentSong.Title = "No connection";
                    CurrentSong.Duration = ErrorDelay;
                    if(Settings.Default.LogDebugInfo)
                        WLog($"No connection Retry after {ErrorDelay.TotalSeconds}sec.", LogLvl.DEBUG);
                }
                finally
                {
                    SongChanged.Invoke(CurrentSong, null);
                    Thread.Sleep(CurrentSong.Duration);
                }
            }
        }
        private static void WriteLog(Song song)
        {
            WLog((new StringBuilder()).Append(song.Title).Append(song.Duration.ToString("' ('mm':'ss')'")).ToString());
        }
        public static void WLog(string msg, LogLvl lvl = LogLvl.INFO)
        {
            if (lvl == LogLvl.DEBUG && !Settings.Default.LogDebugInfo) return;
            using (StreamWriter sw = File.AppendText(LogPath))
            {
                sw.WriteLine((new StringBuilder()).Append(DateTime.Now.ToString("[dd.MM.yyyy] [HH:mm:ss] ")).Append(msg).ToString());
            }
        }
    }
}
