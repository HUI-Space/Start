using UnityEngine;

namespace Start.Editor
{
    public class TULong : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!ulong.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid ulong");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "ulong";
        }
    }
}