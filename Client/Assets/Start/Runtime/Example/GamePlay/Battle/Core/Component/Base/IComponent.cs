namespace Start
{
    public interface IComponent<T> :IReusable where T : IComponent<T>
    {
        void CopyTo(T component);
    }
}