using UnityEngine;

namespace Start.Editor
{
    public class TChar : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!char.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid char");
                return default;
            }
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "char";
        }
    }
}