

namespace Start
{
    public interface IComponent<T> : IReference where T : IComponent<T>
    {
        void CopyTo(T component);
    }
}