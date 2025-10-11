namespace Start
{
    public interface ICopy<T> where T : ICopy<T>
    {
        void CopyTo(T t);
    }
 
    public class Copy : ICopy<Copy>
    {
        public int Id;
        
        public void CopyTo(Copy t)
        {
            t.Id = Id;
        }
    }

    public class AA
    {
        public int Id;
    }
}