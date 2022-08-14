namespace Anison.Tests;
using Anison;
using Anison.Logging;
using System;
using System.Threading;

internal class Logger : ILogger
{
    public Exception? EX;
    public void WriteInfo(string msg) { }
    public void WriteError(string msg, Exception? ex = default) { EX = ex; }
}

public class StateTest
{
    [Fact]
    public void SongChanged()
    {
        var errorTrace = new Logger();
        using var state = new State(errorTrace);
        var wait = new ManualResetEvent(false);
        state.SongChanged += (song) =>
        {
            if (song != default)
                wait.Set();
        };

        if (wait.WaitOne())
        {
            if(errorTrace.EX != default)
                throw errorTrace.EX;
                
            return;
        }
        else throw new Exception("Timeout reached");
    }
}