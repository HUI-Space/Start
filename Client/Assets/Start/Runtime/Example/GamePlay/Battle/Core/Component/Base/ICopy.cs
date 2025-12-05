namespace Start
{
    public class Copy : IComponent<Copy>
    {
        public int Id;
        
        public void CopyTo(Copy t)
        {
            t.Id = Id;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}