using System;

namespace Start
{
    public class TEnum : TType
    {
        public Type Type => typeof(Enum);
        
        public string GetJsonFormat(string value)
        {
            if (!int.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid Enum");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return $"Enum";
        }
    }
}