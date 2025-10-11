using System;

namespace Start
{
    public class TFP : TType
    {
        public Type Type => typeof(FP);
        
        public string GetJsonFormat(string value)
        {
            return value;
        }
        
        public override string ToString()
        {
            return "FP";
        }
    }
}