using System;

namespace Start
{
    public interface IKcpHelper
    {
        event Action<byte[]> OnSendKcp;
        event Action<byte[]> OnReceiveKcp;
        void Update(float elapseSeconds, float realElapseSeconds);
        void SendKcp(byte[] buffer);
        void ReceiveKcp(byte[] data, long offset, long count);
    }
}