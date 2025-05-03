using DiscordRPC.Logging;

namespace Todd;

public class ToddLogger(string path) : FileLogger(path)
{
    public void LogBoth(string msg)
    {
        Console.WriteLine(msg);
        Info(msg);
    }
    
    public void Info(string msg) => base.Info($"{DateTime.Now} - {msg}");
    
    public void Error(string msg) => base.Error($"{DateTime.Now} - {msg}");
}