using System;

namespace Anison.Model;

public class Song : EventArgs
{
	public string Title;
	public TimeSpan Duration;
	public string? PosterUrl { get; set; }
	public Song(string title, TimeSpan duration)
	{
		Title = title;
		Duration = duration;
	}
	public Song(string title = "")
	{
		Title = title;
		Duration = TimeSpan.Zero;
	}
	
	public override string ToString()
	{
		return string.Format("{0} {1}", Title, Duration.ToString(@"\(mm\:ss\)"));
	}
}