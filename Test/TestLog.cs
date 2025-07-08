



using Start;

public class TestLog 
{
    public void Test()
    {
        Logger.Info("TestLog");
    }
}
    
public class TestLogHelper : ILogHelper
{
    public void Log(ELogType logType, string message, params object[] args)
    {
        Console.WriteLine($"{logType}:{message}");
    }
}

