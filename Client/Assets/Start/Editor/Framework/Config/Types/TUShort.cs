using UnityEngine;

namespace Start.Editor
{
    public class TUShort : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!ushort.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid ushort");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "ushort";
        }
    }
}