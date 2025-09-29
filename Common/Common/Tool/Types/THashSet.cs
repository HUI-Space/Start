namespace Start
{
    public class THashSet : TArray
    {
        public THashSet(TType type) : base(type)
        {
        }
        
        public override string ToString()
        {
            return $"HashSet<{_type}>";
        }
    }
}