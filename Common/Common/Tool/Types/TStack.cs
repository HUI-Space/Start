using System;
using System.Collections.Generic;

namespace Start
{
    public class TStack : TGenericType
    {
        public override Type Type => typeof(Stack<>); 
        
        
        public override string ToString()
        {
            return $"Stack<{GenericType}>";
        }
    }
}