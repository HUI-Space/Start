using UnityEngine;

namespace Start.Editor
{
    public class TLong : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!long.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid long");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "long";
        }
    }
}