using Anison.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
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
        public static event EventHandler SongUpdated;
        public class Song : EventArgs
        {
            public string Title;
            public TimeSpan Duration;
            public Image poster { get; set; }
            public override string ToString()
            {
                return string.Format("{0} {1}", Title, Duration.ToString(@"\(mm\:ss\)"));
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

        private static string RequestURL = "https://anison.fm/status.php";
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
            client.DefaultRequestHeaders.Add("Referer", "https://anison.fm/");

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
            //Syncronyosly fetch api data
            client.DefaultRequestHeaders.Add("Accept", "text/javascript");
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
            var json = JObject.Parse(readTask.Result);
            client.DefaultRequestHeaders.Remove("Accept");

            CurrentSong.Title = string.Join(" - ", new string[] { (string)json["on_air"]["anime"], (string)json["on_air"]["track"] });
            CurrentSong.Duration = TimeSpan.FromSeconds((int)json["duration"]);

            //Retriving poster image
            string posterLink = (string)json["on_air"]["link"];
            getTask = client.GetAsync($"https://anison.fm/resources/poster/50/{posterLink}.jpg");
            if (getTask.Exception != null) throw getTask.Exception;
            getTask.Wait();
            if (getTask.Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                CurrentSong.Title = "Request Failed";
                CurrentSong.Duration = ErrorDelay;
                return CurrentSong;
            }
            var StreamReadTask = getTask.Result.Content.ReadAsStreamAsync();
            StreamReadTask.Wait();
            CurrentSong.poster = new Bitmap(StreamReadTask.Result);

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
                    var updateDelay = TimeSpan.FromSeconds(1);
                    while(CurrentSong.Duration.TotalSeconds >= 0)
                    {
                        Thread.Sleep(updateDelay);
                        CurrentSong.Duration = CurrentSong.Duration.Add(-updateDelay);
                        SongUpdated.Invoke(CurrentSong, null);
                    }
                }
            }
        }
        private static void WriteLog(Song song)
        {
            WLog(song.ToString());
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
