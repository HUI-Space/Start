using System;

namespace Start
{
    public class TSByte : TType
    {
        public Type Type => typeof(sbyte);
        public string GetJsonFormat(string value)
        {
            if (!sbyte.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid sbyte");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "sbyte";
        }
    }
}