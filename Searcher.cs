using System.Diagnostics;
using System.Web;

namespace Anison
{
    static class Searcher
    {
        private static string GoogleTemplate = "http://google.com/search?q={0}&ie=UTF-8";
        private static string OsuTemplate = "https://osu.ppy.sh/beatmapsets?m={0}&q={1}";
        private static string YoutubeTemplate = "https://www.youtube.com/results?search_query={0}";
        public static void SearchInGoogle(string term)
        {
            Process.Start(string.Format(GoogleTemplate, HttpUtility.UrlEncode(term)));
        }
        public static void SearchInOsu(string term)
        {
            Process.Start(string.Format(OsuTemplate, 0, HttpUtility.UrlEncode(term)));
        }
        public static void SearchInYoutube(string term)
        {
            Process.Start(string.Format(YoutubeTemplate, HttpUtility.UrlEncode(term)));
        }
    }
}
