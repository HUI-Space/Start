namespace Start
{
    public interface IComponent<T> where T : IComponent<T>
    {
        void CopyTo(T component);
    }
}