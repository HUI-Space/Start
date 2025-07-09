using UnityEngine;

namespace Start.Editor
{
    public class TFloat : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!float.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid float");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "float";
        }
    }
}