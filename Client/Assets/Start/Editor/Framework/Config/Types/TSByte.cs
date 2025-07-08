using UnityEngine;

namespace Start.Editor
{
    public class TSByte : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!sbyte.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid sbyte");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "sbyte";
        }
    }
}