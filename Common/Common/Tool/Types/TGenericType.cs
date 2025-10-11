using System;
using System.Text;

namespace Start
{
    public abstract class TGenericType : TType
    {
        public abstract Type Type { get; }

        public TType GenericType { get; private set; }

        public void SetTType(TType type)
        {
            GenericType = type;
        }
        
        public virtual string GetJsonFormat(string value)
        {
            string[] values = value.Split('_');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < values.Length - 1; i++)
            {
                stringBuilder.Append($"{GenericType.GetJsonFormat(values[i])},");
            }
            stringBuilder.Append($"{GenericType.GetJsonFormat(values[values.Length - 1])}");
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}