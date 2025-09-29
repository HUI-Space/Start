using System;

namespace Start
{
    public class TByte : TType
    {
        public Type Type => typeof(byte);
        public string GetJsonFormat(string value)
        {
            if (!byte.TryParse(value, out var result))
            {
                Logger.Error($"{value} is not a valid byte");
                return default;
            }
            return value;
        }

        public override string ToString()
        {
            return "byte";
        }
    }
}