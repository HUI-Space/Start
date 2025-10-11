using System;
using System.Collections.Generic;
using System.Text;

namespace Start
{
    public class TDictionary : TType
    {
        public TType KeyGenericType { get; private set; }
        public TType ValueGenericType { get; private set; }
        
        public void SetKeyTType(TType key) 
        {
            KeyGenericType = key;
        }
        
        public void SetValueTType(TType key) 
        {
            ValueGenericType = key;
        }
        
        public void SetTType(TType key , TType value) 
        {
            KeyGenericType = key;
            ValueGenericType = value;
        }

        public virtual Type Type => typeof(Dictionary<,>);

        public virtual string GetJsonFormat(string value)
        {
            string[] values = value.Split('|');
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            for (int i = 0; i < values.Length; i++)
            {
                string[] item = values[i].Split('_');
                stringBuilder.AppendLine(i == values.Length - 1
                    ? $"        \"{item[0]}\" : {ValueGenericType.GetJsonFormat(item[1])}"
                    : $"        \"{item[0]}\" : {ValueGenericType.GetJsonFormat(item[1])},");
            }
            stringBuilder.AppendLine("      }");
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"Dictionary<{KeyGenericType},{ValueGenericType}>";
        }
    }
}