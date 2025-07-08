// See https://aka.ms/new-console-template for more information


using Start;

public class Program
{

    public static bool End = false;
    
    /// <summary>
    /// 服务器代码就是一坨，后续有需要再修改
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("服务器启动");
        await Manager.Initialize();
        while (true)
        {
            Thread.Sleep(1);
            try
            {
                Manager.Update(0,0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (End)
            {
                break;
            }
        }
        await Manager.DeInitialize();
        Console.WriteLine("服务器结束");
    }
}