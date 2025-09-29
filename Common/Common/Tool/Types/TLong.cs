using System;

namespace Start
{
    public class TLong : TType
    {
        
        public Type Type => typeof(long);
        
        public string GetJsonFormat(string value)
        {
            if (!long.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid long");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "long";
        }
    }
}