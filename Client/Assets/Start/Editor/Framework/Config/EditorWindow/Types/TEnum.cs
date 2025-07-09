using UnityEngine;

namespace Start.Editor
{
    public class TEnum : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!int.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid Enum");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return $"Enum";
        }
    }
}