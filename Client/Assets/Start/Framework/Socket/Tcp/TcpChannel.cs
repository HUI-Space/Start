using System;
using System.Net;

namespace Start
{
    public class TcpChannel : ChannelBase
    {
        public override EChannelType EChannelType => EChannelType.Tcp;
        
        private readonly Buffer _receiveBuffer = new Buffer();
        public TcpChannel(uint id, IPEndPoint endPoint, IChannelHelper channelHelper)
            : base(new TcpClient(id,endPoint), channelHelper)
        {
            
        }
        
        protected override void Received(byte[] bytes, long offset, long count)
        {
            if (ChannelHelper is ITcpChannelHelper tcpHelper)
            {
                _receiveBuffer.Append(bytes, offset, count);
                //处理粘包
                while (true)
                {
                    if (_receiveBuffer.Size < tcpHelper.PacketLength)
                    {
                        return;
                    }
                    var packetLength = tcpHelper.GetPacketLength(_receiveBuffer.Data);
                    if (packetLength > _receiveBuffer.Size)
                    {
                        return;
                    }
                    var result = new byte[packetLength];
                    Array.Copy(_receiveBuffer.Data, 0, result, 0, packetLength);
                    _receiveBuffer.Remove(0, packetLength);
                    PacketQueue.Enqueue(result);
                }
            }
        }
    }
}