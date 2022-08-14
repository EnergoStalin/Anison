using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using Anison.Model;
using Anison.Logging;

namespace Anison;

/// <summary>
/// Represents radio state
/// </summary>
public sealed class State : IDisposable
{
    public delegate void SongChangedEvent(Song? song);
    public event SongChangedEvent? SongChanged;
    public Song? CurrentSong { get; private set; }

    private static readonly string RequestURL = "https://anison.fm/status.php";
    private readonly ILogger? _log;
    private readonly TimeSpan _errorDelay = new(0, 0, 20);
    private readonly HttpClient _client = new();
    private readonly CancellationTokenSource _cancelUpdates = new();

    public State(ILogger? log = default)
    {
        _client.DefaultRequestHeaders.Add("Referer", "https://anison.fm/");
        _client.DefaultRequestHeaders.Add("Accept", "text/javascript");
        _log = log;

        this.UpdateThread();
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
    private async void UpdateThread()
    {
        while (!_cancelUpdates.Token.IsCancellationRequested)
        {
            var wait = _errorDelay;
            try
            {
                CurrentSong = await _getNowPlayingAsync();
                wait = CurrentSong.Duration;
            }
            catch (AggregateException ex)
            {
                _log?.WriteError($"[State] No connection Retry after {_errorDelay.TotalSeconds}sec.", ex);
                CurrentSong = default;
            }

            // A little bit tricky way to count long running handlers
            var watch = Stopwatch.StartNew();
            SongChanged?.Invoke(CurrentSong);
            watch.Stop();
            if (watch.Elapsed >= wait)
                continue;

            wait -= watch.Elapsed;

            // Cringe
            try { await Task.Delay(wait, _cancelUpdates.Token); } catch {}
        }
    }

    public void Dispose()
    {
        _cancelUpdates.Cancel();
    }
}
