using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.webRequesters
{
    public class TagsApiConsumer : ITagsApiConsumer
    {
        private string baseUrl;
        public TagsApiConsumer(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Task<PaginationResult<ObjectTag>> GetTags()
        {
            return FetchTags();
        }

        public Task<PaginationResult<ObjectTag>> GetTags(int page)
        {
            return FetchTags($"page={page}");
        }

        private async Task<PaginationResult<ObjectTag>> FetchTags(string queryParams="")
        {
            string url = $"{baseUrl}tags/?{queryParams}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                await webRequest.SendWebRequestTask();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    throw new System.Exception($"Error: {webRequest.error} for url: {url}");
                }
                return JsonUtility.FromJson<PaginationResult<ObjectTag>>(webRequest.downloadHandler.text);
            }
        }
    }
}
