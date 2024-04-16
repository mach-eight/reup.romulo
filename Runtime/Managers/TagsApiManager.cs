using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class TagsApiManager : MonoBehaviour, ITagsApiManager
    {
        public ITagsWebRequesterController webRequester { get => _webRequester; set => _webRequester = value; }

        private ITagsWebRequesterController _webRequester;
        private List<ObjectTag> tags = new List<ObjectTag>();
        private bool thereAreTagsToFetch = true;
        private bool waitingForTagsResponse = false;
        private int currentPage = 0;


        public async Task<List<ObjectTag>> GetTags()
        {
            if (currentPage == 0 && !waitingForTagsResponse)
            {
                return await LoadMoreTags();
            }
            return tags;
        }

        public async Task<List<ObjectTag>> LoadMoreTags()
        {
            if (!thereAreTagsToFetch || waitingForTagsResponse)
            {
                return tags;
            }
            waitingForTagsResponse = true;
            PaginationResult<ObjectTag> fetchedTagsResult = await _webRequester.GetTags(++currentPage);
            CheckIfThereIsStillTagsToFetch(fetchedTagsResult);
            AddNewTags(fetchedTagsResult.results);
            waitingForTagsResponse = false;
            return tags;
        }
        
        public void CleanTags()
        {
            tags = new List<ObjectTag>();
            currentPage = 0;
            thereAreTagsToFetch = true;
            waitingForTagsResponse = false;
        }

        private void CheckIfThereIsStillTagsToFetch(PaginationResult<ObjectTag> fetchedTagsResult) { 
            if (string.IsNullOrEmpty(fetchedTagsResult.next))
            {
                thereAreTagsToFetch = false;
            }
        }

        private List<ObjectTag> AddNewTags(ObjectTag[] newTags)
        {
            tags = new List<ObjectTag>(tags);
            tags.AddRange(newTags);
            return tags;
        }

    }
}
