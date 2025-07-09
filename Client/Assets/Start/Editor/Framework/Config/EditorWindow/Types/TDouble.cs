using UnityEngine;

namespace Start.Editor
{
    public class TDouble :TType
    {
        public string GetJsonFormat(string value)
        {
            if (!double.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid double");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "double";
        }
    }
}