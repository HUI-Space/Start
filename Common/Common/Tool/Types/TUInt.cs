using System;

namespace Start
{
    public class TUInt : TType
    {
        public Type Type => typeof(uint);
        public string GetJsonFormat(string value)
        {
            if (!uint.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid uint");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "uint";
        }
    }
}