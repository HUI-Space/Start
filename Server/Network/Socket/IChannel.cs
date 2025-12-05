using System.Net;


namespace Start.Server
{
    public interface IChannel : IReusable
    {
        int SessionId { get; }
        
        void Receive(byte[] data);
        
        void Send(Span<byte> data);
    }
}

