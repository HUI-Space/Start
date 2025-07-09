using UnityEngine;

namespace Start.Editor
{
    public class TBool : TType
    {
        public string GetJsonFormat(string value)
        {
            if (value.Equals("true") || value.Equals("false"))
            {
                return value;
            }
            Debug.LogError($"{value} is not a valid bool");
            return default;
        }
        
        public override string ToString()
        {
            return "bool";
        }
    }
}