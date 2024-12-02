namespace Start.Framework
{
    public partial class MessageManager
    {
        private class Message : IReference
        {
            public int MessageType { get; private set; }
            public int MessageId { get; private set; }
            public IGenericData Data { get; private set; }

            public static Message Create(int MessageType, int messageId, IGenericData data)
            {
                Message message = ReferencePool.Acquire<Message>();
                message.MessageType = MessageType;
                message.MessageId = messageId;
                message.Data = data;
                return message;
            }

            public void Clear()
            {
                MessageId = default;
                Data = default;
            }
        }
    }
}