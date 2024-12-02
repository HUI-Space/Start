namespace Start.Framework
{
    public abstract class DataEntityCollectionBase : IDataEntityCollection
    {
        public abstract void Initialize();

        public abstract void Register();

        public abstract void UnRegister();

        public abstract void Reset();

        public abstract void DeInitialize();

        protected void RegisterNetMessage<T>(uint channelId) where T : class
        {
            SocketManager.Instance.Register<T>(channelId, CheckAllKey, 1);
        }

        protected void UnRegisterNetMessage<T>(uint channelId) where T : class
        {
            SocketManager.Instance.UnRegister<T>(channelId, CheckAllKey);
        }

        protected virtual void CheckAllKey(uint code, object msg)
        {
        }
    }
}