using System;
using System.Collections.Generic;

namespace Start
{
    public class TSortedDictionary : TDictionary
    {
        public override Type Type => typeof(SortedDictionary<,>);
        
        public override string ToString()
        {
            return $"SortedDictionary<{KeyGenericType},{ValueGenericType}>";
        }
    }
}