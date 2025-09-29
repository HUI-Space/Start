using System;
using System.Text;

namespace Start
{
    public class TDictionary : TType
    {
        private TType _key;
        private TType _value;
        
        public TDictionary(TType key , TType value) 
        {
            _key = key;
            _value = value;
        }

        public Type Type { get; }

        public string GetJsonFormat(string value)
        {
            string[] values = value.Split('|');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            for (int i = 0; i < values.Length; i++)
            {
                string[] item = values[i].Split('_');
                stringBuilder.AppendLine(i == values.Length - 1
                    ? $"        \"{item[0]}\" : {_value.GetJsonFormat(item[1])}"
                    : $"        \"{item[0]}\" : {_value.GetJsonFormat(item[1])},");
            }
            stringBuilder.AppendLine("      }");
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"Dictionary<{_key},{_value}>";
        }
    }
}