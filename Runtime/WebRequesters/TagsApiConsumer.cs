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

        public Task<PaginationResult<Tag>> GetTags()
        {
            return FetchTags();
        }

        public Task<PaginationResult<Tag>> GetTags(int page)
        {
            return FetchTags($"page={page}");
        }

        private async Task<PaginationResult<Tag>> FetchTags(string queryParams="")
        {
            string url = $"{baseUrl}tags/?{queryParams}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                WebRequestResult result = await webRequest.SendWebRequestTask();
                if (result.IsSuccess)
                {
                    return JsonUtility.FromJson<PaginationResult<Tag>>(webRequest.downloadHandler.text);
                }
                throw new System.Exception($"Error: {webRequest.error} for url: {url}");
            }
        }
    }
}
