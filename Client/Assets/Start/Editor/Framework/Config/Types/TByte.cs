using UnityEngine;

namespace Start.Editor
{
    public class TByte : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!byte.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid byte");
                return default;
            }
            return value;
        }

        public override string ToString()
        {
            return "byte";
        }
    }
}