using System;

namespace Anison.Logging;

public interface ILogger
{
    void WriteInfo(string msg);
    void WriteError(string msg, Exception? ex = default);
}