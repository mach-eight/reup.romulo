using UnityEngine.Networking;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllers
{
    public static class UnityWebRequestExtensions
    {
        /**
         * This method is an extension method for UnityWebRequest that returns a Task<UnityWebRequest>
         * that completes when the UnityWebRequest is done. This allow us to use async/await syntax
         */
        public static Task<UnityWebRequest> SendWebRequestTask(this UnityWebRequest webRequest)
        {
            var completionSource = new TaskCompletionSource<UnityWebRequest>();
            webRequest.SendWebRequest().completed += operation =>
            {
                completionSource.TrySetResult(webRequest);
            };
            return completionSource.Task;
        }
    }
}
