using System;

namespace Start
{
    public class TString : TType
    {
        public Type Type => typeof(string);
        public string GetJsonFormat(string value)
        {
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "string";
        }
    }
}