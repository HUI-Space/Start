namespace Start
{
    /// <summary>
    /// Http 响应
    /// </summary>
    public class HttpResponse : IReusable
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; private set; }
        
        /// <summary>
        /// 请求链接
        /// </summary>
        public string Url { get; private set; }
        
        /// <summary>
        /// 要发送的数据流
        /// </summary>
        public byte[] PostData{ get; private set; }
        
        /// <summary>
        /// 响应状态码
        /// </summary>
        public long ResponseCode { get; private set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 消息结果
        /// </summary>
        public byte[] Result { get; private set; }

        public static HttpResponse Create(string url)
        {
            HttpResponse httpResponse = RecyclableObjectPool.Acquire<HttpResponse>();
            httpResponse.Url = url;
            return httpResponse;
        }
        
        public static HttpResponse Create(string url , byte[] postData)
        {
            HttpResponse httpResponse = RecyclableObjectPool.Acquire<HttpResponse>();
            httpResponse.Url = url;
            httpResponse.PostData = postData;
            return httpResponse;
        }

        public void SetError(string error , long responseCode)
        {
            Error = error;
            ResponseCode = responseCode;
            IsSuccess = false;
            IsDone = true;
        }

        public void SetResult(byte[] result , long responseCode)
        {
            Result = result;
            ResponseCode = responseCode;
            IsSuccess = true;
            IsDone = true;
        }
        
        public void Reset()
        {
            Url = default;
            Error = default;
            IsDone = default;
            Result = default;
            PostData = default;
            IsSuccess = default;
            ResponseCode = default;
        }
    }
}