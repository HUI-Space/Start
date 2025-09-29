using System;

namespace Start
{
    public interface TType
    {
        /// <summary>
        ///  类型
        /// </summary>
        Type Type { get; }
        
        /// <summary>
        ///  获取Json格式
        /// </summary>
        string GetJsonFormat(string value);
    }
}