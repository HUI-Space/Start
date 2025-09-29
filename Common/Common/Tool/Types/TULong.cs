using System;

namespace Start
{
    public class TULong : TType
    {
        public Type Type => typeof(ulong);
        public string GetJsonFormat(string value)
        {
            if (!ulong.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid ulong");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "ulong";
        }
    }
}