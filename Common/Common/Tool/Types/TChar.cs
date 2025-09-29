using System;

namespace Start
{
    public class TChar : TType
    {
        public Type Type => typeof(Char);
        public string GetJsonFormat(string value)
        {
            if (!char.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid char");
                return default;
            }
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "char";
        }
    }
}