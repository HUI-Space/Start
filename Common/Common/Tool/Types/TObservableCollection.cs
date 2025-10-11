using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Start
{
    public class TObservableCollection : TGenericType
    {
        public override Type Type => typeof(ObservableCollection<>); 
        
        public override string ToString()
        {
            return $"ObservableCollection<{GenericType}>";
        }
    }
}