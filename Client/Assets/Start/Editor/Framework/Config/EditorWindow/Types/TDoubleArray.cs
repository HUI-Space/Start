using System.Text;

namespace Start.Editor
{
    public class TDoubleArray : TType
    {
        protected TType _type;
        
        public TDoubleArray(TType type)
        {
            _type = type;
        }
        public string GetJsonFormat(string value)
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
                    stringBuilder.Append($"{_type.GetJsonFormat(values[j])},");
                }
                stringBuilder.Append($"{_type.GetJsonFormat(values[values.Length - 1])}");
                stringBuilder.Append("]");
                if (i != rows.Length - 1)
                {
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}