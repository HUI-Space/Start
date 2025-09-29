using System;

namespace Start
{
    public class TInt : TType
    {
        public Type Type => typeof(int);
        public string GetJsonFormat(string value)
        {
            if (!int.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid int");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "int";
        }
    }
}