namespace Anison.Tests;
using Anison;
using Anison.Logging;
using System.Threading;

internal class Logger : ILogger {
    public Exception? EX;
    public void WriteInfo(string msg) {}
	public void WriteError(string msg, Exception? ex = default) { EX = ex; }
}

public class UnitTest1
{
    [Fact]
    public void StateTest()
    {
        var errorTrace = new Logger();
        var state = new State(errorTrace);
        var wait = new ManualResetEvent(false);
        state.SongChanged += (song) => {
            if(song != default)
                wait.Set();
        };

        if(wait.WaitOne(3000))
        {
            if(errorTrace.EX != default)
                throw errorTrace.EX;

            return;
        }
        else throw new Exception("Timeout reached");
    }
}