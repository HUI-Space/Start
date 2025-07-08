using UnityEngine;

namespace Start.Editor
{
    public class TShort : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!short.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid short");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "short";
        }
    }
}