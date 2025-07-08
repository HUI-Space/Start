// See https://aka.ms/new-console-template for more information

using Start;

public class Program
{
    /// <summary>
    /// 服务器代码就是一坨，后续有需要再修改
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        await Manager.Initialize();
    }
}
