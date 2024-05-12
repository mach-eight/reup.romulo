using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class TagsApiManager : MonoBehaviour, ITagsApiManager
    {
        public ITagsApiConsumer tagsApiConsumer { get => _tagsApiConsumer; set => _tagsApiConsumer = value; }

        public string searchTagText { get => _searchText; set => _searchText = value; }

        public bool waitingForTagsResponse = false;

        private string _searchText = "";
        private ITagsApiConsumer _tagsApiConsumer;
        private List<Tag> tags = new List<Tag>();
        private bool thereAreTagsToFetch = true;
        private int currentPage = 0;

        public async Task<List<Tag>> GetTags()
        {
            if (currentPage == 0 && !waitingForTagsResponse)
            {
                return await LoadMoreTags();
            }
            return tags;
        }

        public async Task<List<Tag>> LoadMoreTags()
        {
            if (!thereAreTagsToFetch || waitingForTagsResponse)
            {
                return tags;
            }
            waitingForTagsResponse = true;
            PaginationResult<Tag> fetchedTagsResult = await _tagsApiConsumer.GetTags(++currentPage);
            CheckIfThereIsStillTagsToFetch(fetchedTagsResult);
            AddNewTags(fetchedTagsResult.results);
            waitingForTagsResponse = false;
            return tags;
        }
        
        public void CleanTags()
        {
            if (waitingForTagsResponse) return;
            tags = new List<Tag>();
            currentPage = 0;
            thereAreTagsToFetch = true;
            waitingForTagsResponse = false;
        }

        private void CheckIfThereIsStillTagsToFetch(PaginationResult<Tag> fetchedTagsResult) {
            if (string.IsNullOrEmpty(fetchedTagsResult.next))
            {
                thereAreTagsToFetch = false;
            }
        }

        private List<Tag> AddNewTags(Tag[] newTags)
        {
            tags = new List<Tag>(tags);
            tags.AddRange(newTags);
            return tags;
        }

    }
}
