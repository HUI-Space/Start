namespace Start.Editor
{
    public class TList : TArray
    {
        public TList(TType type) : base(type)
        {
        }
        
        public override string ToString()
        {
            return $"List<{_type}>";
        }
    }
}