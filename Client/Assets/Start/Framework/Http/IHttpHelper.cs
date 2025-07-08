using System.Threading.Tasks;

namespace Start
{
    public interface IHttpHelper 
    {
        /// <summary>
        /// 通过 Web 请求代理辅助器发送 Web 请求。
        /// </summary>
        /// <param name="url">Web 请求地址</param>
        /// <param name="timeout">延时</param>
        /// <returns></returns>
        Task<HttpResponse> Get(string url, int timeout);
        
        /// <summary>
        /// 通过 Web 请求代理辅助器发送 Web 请求。
        /// </summary>
        /// <param name="url">Web 请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="timeout">延时</param>
        /// <returns></returns>
        Task<HttpResponse> Post(string url,byte[] postData,int timeout);
    }
}