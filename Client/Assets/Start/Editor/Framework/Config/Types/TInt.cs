using UnityEngine;

namespace Start.Editor
{
    public class TInt : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!int.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid int");
                return default;
            }
            return value;
        }
        
        public override string ToString()
        {
            return "int";
        }
    }
}