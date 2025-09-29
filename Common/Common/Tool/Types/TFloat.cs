using System;

namespace Start
{
    public class TFloat : TType
    {
        public Type Type => typeof(float);
        
        public string GetJsonFormat(string value)
        {
            if (!float.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid float");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "float";
        }
    }
}