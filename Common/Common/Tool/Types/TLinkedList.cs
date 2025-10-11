using System;
using System.Collections.Generic;

namespace Start
{
    public class TLinkedList : TGenericType
    {
        public override Type Type => typeof(LinkedList<>);
        
        public override string ToString()
        {
            return $"LinkedList<{GenericType}>";
        }
    }
}