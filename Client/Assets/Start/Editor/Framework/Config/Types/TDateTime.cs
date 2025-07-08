using System;
using UnityEngine;

namespace Start.Editor
{
    public class TDateTime : TType
    {
        public string GetJsonFormat(string value)
        {
            if (!DateTime.TryParse(value, out var result))
            {
                Debug.LogError($"{value} is not a valid DateTime");
                return default;
            }
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "DateTime";
        }
    }
}