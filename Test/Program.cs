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
        string typeString = "Dictionary<Dictionary<int,List<string>>,List<int>>";
        typeString = "Dictionary<Dictionary<int,List<string>>,Dictionary<Dictionary<int,List<string>>,List<int>>>";
        if (TypeFactory.TryParseOuterDictionaryTypes(typeString, out string keyType, out string valueType))
        {
            Console.WriteLine($"外层字典的Key类型: {keyType}");   // 输出: Dictionary<int,List<string>>
            Console.WriteLine($"外层字典的Value类型: {valueType}"); // 输出: List<int>
        }
        else
        {
            Console.WriteLine("解析失败");
        }

        TypeFactory.TryGetType(typeString, out TType type);
        Console.WriteLine(type);
    }
}
