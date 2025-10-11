using System;
using System.Text;

namespace Start
{
    public class TDoubleArray : TGenericType
    {
        public override Type Type => typeof(Array[,]);
        
        public override string GetJsonFormat(string value)
        {
            string[] rows = value.Split('|');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < rows.Length; i++)
            {
                stringBuilder.Append("[");
                string[] values = rows[i].Split('_');
                for (int j = 0; j < values.Length - 1; j++)
                {
                    stringBuilder.Append($"{GenericType.GetJsonFormat(values[j])},");
                }
                stringBuilder.Append($"{GenericType.GetJsonFormat(values[values.Length - 1])}");
                stringBuilder.Append("]");
                if (i != rows.Length - 1)
                {
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        
        public override string ToString()
        {
            return $"{GenericType}[,]";
        }
    }
}