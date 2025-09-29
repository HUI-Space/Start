using System;

namespace Start
{
    public class TBool : TType
    {
        public Type Type => typeof(bool);
        
        public string GetJsonFormat(string value)
        {
            if (value.Equals("true") || value.Equals("false"))
            {
                return value;
            }
            Logger.Error($"{value} is not a valid bool");
            return default;
        }
        
        public override string ToString()
        {
            return "bool";
        }
    }
}