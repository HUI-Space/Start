using System;

namespace Start
{
    /// <summary>
    /// 数据集基础抽象类，提供数据集版本管理和网络消息注册功能
    /// </summary>
    public abstract class DataSetBase
    {
        /// <summary>
        /// 获取当前数据集的版本号
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// 注册数据集特定的逻辑
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// 取消注册数据集特定的逻辑
        /// </summary>
        public abstract void UnRegister();

        /// <summary>
        /// 当数据集版本改变时触发的事件
        /// </summary>
        private event Action OnVersionChange;

        /// <summary>
        /// 添加版本改变事件的监听器
        /// </summary>
        /// <param name="action">版本改变时要执行的动作</param>
        public void AddListener(Action action)
        {
            OnVersionChange += action;
        }

        /// <summary>
        /// 移除版本改变事件的监听器
        /// </summary>
        /// <param name="action">要移除的版本改变时执行的动作</param>
        public void RemoveListener(Action action)
        {
            OnVersionChange -= action;
        }

        /// <summary>
        /// 重置数据集版本号为0
        /// </summary>
        public void Reset()
        {
            Version = 0;
        }

        /// <summary>
        /// 注册网络消息处理回调
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="callback">接收到消息时的回调函数</param>
        /// <typeparam name="T">消息的类型</typeparam>
        protected void RegisterNetMessage<T>(uint channelId, uint messageId, Action<uint, T> callback) where T : class
        {
            SocketManager.Instance.Register(channelId, messageId, callback, 1);
            SocketManager.Instance.Register<object>(channelId, messageId, OnCallback, 1);
        }

        /// <summary>
        /// 取消注册网络消息处理回调
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="callback">要取消的接收到消息时的回调函数</param>
        /// <typeparam name="T">消息的类型</typeparam>
        protected void UnRegisterNetMessage<T>(uint channelId, uint messageId, Action<uint, T> callback) where T : class
        {
            SocketManager.Instance.UnRegister(channelId, messageId, callback);
            SocketManager.Instance.UnRegister<object>(channelId, messageId, OnCallback);
        }

        /// <summary>
        /// 网络消息回调处理函数
        /// </summary>
        /// <param name="code">消息代码</param>
        /// <param name="msg">接收到的消息</param>
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
                    Logger.Error(e.Message);
                }
            }
        }
    }
}