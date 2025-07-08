using System;


namespace Start
{
    public class CallTask : IReference, IComparable<CallTask>
    {
        /// <summary>
        /// 序列号
        /// </summary>
        public uint SerialNumber { get; private set; }

        /// <summary>
        /// 消息Id
        /// </summary>
        public uint MessageId { get; private set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public uint ErrCode { get; private set; }

        /// <summary>
        /// 发送给服务器的协议
        /// </summary>
        public byte[] Request { get; private set; }

        /// <summary>
        /// 服务器发送给客户端消息
        /// </summary>
        public byte[] Response { get; private set; }

        /// <summary>
        /// 任务
        /// </summary>
        public RecycleTask Task { get; private set; }

        public int CompareTo(CallTask other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return other.SerialNumber.CompareTo(SerialNumber);
        }

        public void Clear()
        {
            SerialNumber = default;
            Request = default;
            Response = default;
            ErrCode = default;
            MessageId = default;
            Task = default;
        }

        public static CallTask Create(uint serialNumber, uint messageId, byte[] request)
        {
            var callTask = ReferencePool.Acquire<CallTask>();
            callTask.SerialNumber = serialNumber;
            callTask.MessageId = messageId;
            callTask.Request = request;
            callTask.Task = RecycleTask.Create();
            return callTask;
        }

        public void SetSerialNumber(uint serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void SetResponse(uint errCode, byte[] response)
        {
            ErrCode = errCode;
            Response = response;
            Task.SetResult();
        }
    }
}