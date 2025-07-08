using System.Text;

namespace Start.Editor
{
    public class TArray : TType
    {
        protected TType _type;
        
        public TArray(TType type)
        {
            _type = type;
        }
        
        public string GetJsonFormat(string value)
        {
            string[] values = value.Split('_');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < values.Length - 1; i++)
            {
                stringBuilder.Append($"{_type.GetJsonFormat(values[i])},");
            }
            stringBuilder.Append($"{_type.GetJsonFormat(values[values.Length - 1])}");
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"{_type}[]";
        }
    }
}