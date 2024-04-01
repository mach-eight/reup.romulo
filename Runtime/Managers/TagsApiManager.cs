using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections;
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
        private int currentPage = 0;


        public async Task<List<ObjectTag>> GetTags()
        {
            Debug.Log("current page " + currentPage);
            if (currentPage == 0)
            {
                return await LoadMoreTags();
            }
            return tags;
        }

        public async Task<List<ObjectTag>> LoadMoreTags()
        {
            Debug.Log("asking to load more tags");
            Debug.Log("currentPage: " + currentPage);
            if (!thereAreTagsToFetch)
            {
                return tags;
            }
            Debug.Log("looks like there are still tags to fetch");
            PaginationResult<ObjectTag> fetchedTagsResult = await _webRequester.GetTags(++currentPage);
            Debug.Log("currentPage after fetching: " + currentPage);
            Debug.Log("fetchedTagsResult");
            Debug.Log(fetchedTagsResult.next);
            Debug.Log(fetchedTagsResult.previous);
            Debug.Log(fetchedTagsResult.count);
            //thereAreTagsToFetch = false;
            if(fetchedTagsResult.next == null || fetchedTagsResult.next == "")
            {
                Debug.Log("Tags to fetch are done!");
                thereAreTagsToFetch = false;
            }
            else
            {
                Debug.Log("There are more tags to fetch keep going babe!");
                Debug.Log("next: " + fetchedTagsResult.next);
            }
            tags = new List<ObjectTag>(tags);
            tags.AddRange(fetchedTagsResult.results);
            return tags;
        }
        
        public void CleanTags()
        {
            tags = new List<ObjectTag>();
            currentPage = 0;
            thereAreTagsToFetch = true;
        }
    }
}
