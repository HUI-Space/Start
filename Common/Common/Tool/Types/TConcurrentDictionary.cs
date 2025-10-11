using System;
using System.Collections.Concurrent;

namespace Start
{
    public class TConcurrentDictionary : TDictionary
    {
        public override Type Type => typeof(ConcurrentDictionary<,>);
        
        public override string ToString()
        {
            return $"ConcurrentDictionary<{KeyGenericType},{ValueGenericType}>";
        }
    }
}