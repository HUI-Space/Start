namespace Start.Framework
{
    public interface IDataEntityCollection
    {
        void Initialize();
        void Register();
        void UnRegister();
        void Reset();
        void DeInitialize();
    }
}