using System;
using System.Collections.Generic;
using System.Text;

namespace Start
{
    public class TList : TGenericType
    {
        public override Type Type => typeof(List<>); 
        
        
        public override string ToString()
        {
            return $"List<{GenericType}>";
        }
    }
}