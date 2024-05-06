using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;

namespace ReupVirtualTwin.webRequesters
{
    public static class UnityWebRequestExtensions
    {
        /**
         * This method is an extension method for UnityWebRequest that returns a Task<UnityWebRequest>
         * that completes when the UnityWebRequest is done. This allow us to use async/await syntax
         */
        public static Task<WebRequestResult> SendWebRequestTask(this UnityWebRequest webRequest)
        {
            var completionSource = new TaskCompletionSource<WebRequestResult>();
            webRequest.SendWebRequest().completed += operation =>
            {
                if(
                    webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError
                ) {
                    string error = $"Error: {webRequest.error} for url: {webRequest.url}";
                    Debug.LogError(error);
                    completionSource.TrySetResult(WebRequestResult.Failure(webRequest, error));
                    return;
                }
                completionSource.TrySetResult(WebRequestResult.Success(webRequest));
            };
            return completionSource.Task;
        }
    }
        public class WebRequestResult
        {
            public UnityWebRequest Request { get; private set; }
            public bool IsSuccess { get; private set; }
            public string ErrorMessage { get; private set; }

            public static WebRequestResult Success(UnityWebRequest request)
            {
                return new WebRequestResult
                {
                    Request = request,
                    IsSuccess = true,
                    ErrorMessage = null
                };
            }
            public static WebRequestResult Failure(UnityWebRequest request, string error)
            {
                return new WebRequestResult
                {
                    Request = request,
                    IsSuccess = false,
                    ErrorMessage = error
                };
            }
        }
}
