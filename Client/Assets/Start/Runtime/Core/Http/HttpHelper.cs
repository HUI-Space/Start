using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Start
{
    public class HttpHelper : IHttpHelper
    {
        public Task<HttpResponse> Get(string url, int timeout)
        {
            HttpResponse webResponse = HttpResponse.Create(url);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            return SendRequest(unityWebRequest, webResponse , timeout);
        }

        public Task<HttpResponse> Post(string url, byte[] postData, int timeout)
        {
            HttpResponse webResponse = HttpResponse.Create(url,postData);
            UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, Encoding.UTF8.GetString(postData));
            return SendRequest(unityWebRequest, webResponse , timeout);
        }

        private async Task<HttpResponse> SendRequest(UnityWebRequest unityWebRequest, HttpResponse httpResponse ,int timeout)
        {
            if (timeout > 0)
            {
                unityWebRequest.timeout = timeout;
            }
            await unityWebRequest.SendWebRequest();
            unityWebRequest.Abort();
            
            bool isError = false;
#if UNITY_2020_2_OR_NEWER
            isError = unityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
#else
            isError = unityWebRequest.isError;
#endif
            
            if (isError)
            {
                httpResponse.SetError(unityWebRequest.error,unityWebRequest.responseCode);
            }
            else
            {
                httpResponse.SetResult(unityWebRequest.downloadHandler.data,unityWebRequest.responseCode);
            }
            
            unityWebRequest.Dispose();
            
            return httpResponse;
        }
    }
}