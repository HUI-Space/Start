using System;

namespace Start.Framework
{
    public interface IKcpHelper
    {
        event Action<byte[]> OnSendKcp;
        event Action<byte[]> OnReceiveKcp;
        void Update(float elapseSeconds, float realElapseSeconds);
        void SendKcp(byte[] buffer);
        void ReceiveKcp(byte[] data);
    }
}