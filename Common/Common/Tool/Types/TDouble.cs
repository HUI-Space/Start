using System;

namespace Start
{
    public class TDouble :TType
    {
        public Type Type => typeof(Double);
        public string GetJsonFormat(string value)
        {
            if (!double.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid double");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "double";
        }
    }
}