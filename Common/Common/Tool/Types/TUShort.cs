using System;

namespace Start
{
    public class TUShort : TType
    {
        public Type Type => typeof(ushort);
        
        public string GetJsonFormat(string value)
        {
            if (!ushort.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid ushort");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "ushort";
        }
    }
}