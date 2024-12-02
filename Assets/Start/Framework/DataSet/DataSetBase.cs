using System;

namespace Start.Framework
{
    public abstract class DataSetBase
    {
        public int Version { get; private set; }
        
        public abstract void Register();

        public abstract void UnRegister();

        private event Action OnVersionChange;

        public void AddListener(Action action)
        {
            OnVersionChange += action;
        }

        public void RemoveListener(Action action)
        {
            OnVersionChange -= action;
        }

        public void Reset()
        {
            Version = 0;
        }

        protected void RegisterNetMessage<T>(uint channelId,  Action<uint, T> callback) where T : class
        {
            SocketManager.Instance.Register(channelId, callback,1);
            SocketManager.Instance.Register<object>(channelId, OnCallback,1);
        }

        protected void UnRegisterNetMessage<T>(uint channelId, Action<uint, T> callback) where T : class
        {
            SocketManager.Instance.UnRegister(channelId, callback);
            SocketManager.Instance.UnRegister<object>(channelId, OnCallback);
        }
        
        private void OnCallback(uint code, object msg)
        {
            if (code == 0)
            {
                Version++;
                try
                {
                    OnVersionChange?.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }
    }
}