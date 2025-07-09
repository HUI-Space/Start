namespace Start.Editor
{
    public class TString : TType
    {
        public string GetJsonFormat(string value)
        {
            return $"\"{value}\"";
        }
        
        public override string ToString()
        {
            return "string";
        }
    }
}