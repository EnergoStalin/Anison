using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Anison.Logging;

namespace Anison;

public sealed class Player
{
	
	private MediaFoundationReader? _mediaStream;
	private WaveOutEvent _playback = new WaveOutEvent();
	private ILogger? _log;
	public bool IsPlaying
		=>	_playback?.PlaybackState == PlaybackState.Playing;
	public float? Volume {
		get => _playback?.Volume;
		set => _playback.Volume = (float)value!;
	}
	public string? URL => _mediaStream?.ToString();
	public Player(string url, ILogger? log = default)
	{
		_log = log;
		Init(url);
	}
	/// <summary>
	/// Initialize playback reinitialize if needed
	/// Use this method for reinitalize it's safe
	/// </summary>
	public void Init(string? url = default)
	{
		if(url == default && URL != default)
			url = URL;

		if(url == default)
			throw new ArgumentNullException("None of URL or url provided");

		if (_mediaStream != default)
		{
			if(_mediaStream.ToString() == url)
				return;

			Close();
		}

		_mediaStream = new MediaFoundationReader(url);

		try
		{
			_playback = new WaveOutEvent() { Volume = 1 };
		}
		catch (Exception ex)
		{
			Close();
			_log?.WriteError($"WMP init failed {ex.Message}");

			throw;
		}
	}
	public void Play()
	{
		if (_playback == default)
			throw new NullReferenceException("Init player before playing");

		_playback.Stop();
		_mediaStream?.Seek(0, System.IO.SeekOrigin.End);
		_playback.Play();
	}
	public void Pause() {

	}
	/// <summary>
	/// Reconnect stream with <see cref="Player.URL">
	/// </summary>
	public void Reconnect()
	{
		Close();
		Init();
		Play();
	}
	public void Close()
	{
		_playback?.Dispose();
		_mediaStream?.Close();
	}
}
