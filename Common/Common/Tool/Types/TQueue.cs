using System;
using System.Collections.Generic;

namespace Start
{
    public class TQueue : TGenericType
    {
        public override Type Type => typeof(Queue<>); 
        
        
        public override string ToString()
        {
            return $"Queue<{GenericType}>";
        }
    }
}