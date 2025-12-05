using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Start;


// 测试示例
class Program
{
    public class A
    {
        
    }
    static void Main()
    {

        List<A> a = new List<A>();
        a.Capacity = 10;
        Console.WriteLine(a[1]);
        for (int i = 0; i < a.Count; i++)
        {
            Console.WriteLine("aa");
        }
        // 测试基础泛型容器
        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(List<string>)));
        // 输出：List<string>

        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(Dictionary<int, string>)));
        // 输出：Dictionary<int, string>

        // 测试嵌套泛型
        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(HashSet<List<int>>)));
        // 输出：HashSet<List<int>>

        // 测试复杂嵌套
        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(ObservableCollection<string>)));
        // 输出：Dictionary<string, HashSet<DateTime>>

        // 测试可空类型和数组（额外支持）
        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(int?)));
        // 输出：int?
        Console.WriteLine(GenericTypeNameHelper.IsGenericContainer(typeof(List<int>[])));
        // 输出：List<int>[]
    }
}