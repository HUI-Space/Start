using System;

namespace Start
{
    public class TArray : TGenericType
    {
        public override Type Type => typeof(Array);
        
        public override string ToString()
        {
            return $"{GenericType}[]";
        }
    }
}