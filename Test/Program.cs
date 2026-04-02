using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Start;


// 测试示例
class Program
{

    static void Main()
    {
        Console.WriteLine("testS");
        Test();
        Console.WriteLine("testE");
    }
    
    private static async void Test()
    { 
        StructTask task = new StructTask(true);
        task.SetResult();
        Console.WriteLine("test");
        await task;
        Console.WriteLine("test2");
        await task;
        Console.WriteLine("test3");
        //Task.w
    }
}