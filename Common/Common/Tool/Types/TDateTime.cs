using System;

namespace Start
{
    public class TDateTime : TType
    {
        public Type Type => typeof(DateTime);

        public string GetJsonFormat(string value)
        {
            if (!DateTime.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid DateTime");
                return default;
            }
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "DateTime";
        }
    }
}