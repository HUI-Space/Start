using UnityEngine;

namespace Start.Editor
{
    public class TUInt : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!uint.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid uint");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "uint";
        }
    }
}