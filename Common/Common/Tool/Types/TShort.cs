using System;

namespace Start
{
    public class TShort : TType
    {
        public Type Type => typeof(short);
        public string GetJsonFormat(string value)
        {
            if (!short.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid short");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "short";
        }
    }
}