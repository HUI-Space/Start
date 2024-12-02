﻿using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class HttpManager: ManagerBase<HttpManager>
    {
        public override int Priority => 13;
        
        private IHttpHelper _httpHelper;

        public static void SetHelper(IHttpHelper httpHelper)
        {
            Instance._httpHelper = httpHelper;
        }
        
        /// <summary>
        /// 获取Url
        /// HttpResponse 使用完成及时Reference.Release(HttpResponse)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public Task<HttpResponse> Get(string url, int timeout = 0)
        {
            return _httpHelper.Get(url, timeout);
        }

        /// <summary>
        /// 获取Url
        /// HttpResponse 使用完成及时Reference.Release(HttpResponse)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public Task<HttpResponse> Post(string url, byte[] postData, int timeout = 10)
        {
            return _httpHelper.Post(url, postData, timeout);
        }
    }
}