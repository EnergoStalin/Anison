using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Anison.Model;
using Anison.Logging;

namespace Anison
{
	/// <summary>
	/// Represents radio state
	/// </summary>
	class State
	{
		public delegate void SongChangedEvent(object? sender, Song? song);
		public event SongChangedEvent? SongChanged;

		private static readonly string RequestURL = "https://anison.fm/status.php";
		public Song? CurrentSong { get; private set; }

		private readonly Logger _log;
		private readonly TimeSpan _errorDelay = new TimeSpan(0, 0, 20);
		private readonly HttpClient _client = new HttpClient();

		public State(Logger log)
		{
			_client.DefaultRequestHeaders.Add("Referer", "https://anison.fm/");
			_client.DefaultRequestHeaders.Add("Accept", "text/javascript");
			_log = log;

			this.LoggerThread();
		}

		/// <summary>
		/// Perform http request to anison api endpoint
		/// </summary>
		/// <returns></returns>
		private async Task<Song> _getNowPlayingAsync()
		{
			var song = new Song();
			var status = await _client.GetStringAsync(RequestURL);

			var json = JObject.Parse(status);

			var anime = (string)json["on_air"]!["anime"]!;
			var track = (string)json["on_air"]!["track"]!;
			var posterId = (string)json["on_air"]!["link"]!;
			var duration = (int)json["duration"]!;

			song.Title = $"{anime} - {track}";
			song.Duration = TimeSpan.FromSeconds(duration);
			song.PosterUrl = $"https://anison.fm/resources/poster/50/{posterId}.jpg";


			return song;
		}
		private async void LoggerThread()
		{
			while (true)
			{
				try
				{
					CurrentSong = await _getNowPlayingAsync();
					await Task.Delay(CurrentSong.Duration);
				}
				catch (AggregateException)
				{
					_log.WriteError($"[AnisonState] No connection Retry after {_errorDelay.TotalSeconds}sec.");
					await Task.Delay(_errorDelay);
				}
			}
		}
	}
}
