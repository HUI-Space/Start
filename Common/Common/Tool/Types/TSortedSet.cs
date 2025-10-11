using System;
using System.Collections.Generic;

namespace Start
{
    public class TSortedSet : TGenericType
    {
        public override Type Type => typeof(SortedSet<>); 
        
        public override string ToString()
        {
            return $"SortedSet<{GenericType}>";
        }
    }
}