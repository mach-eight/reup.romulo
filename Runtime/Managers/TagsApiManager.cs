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

        [SerializeField]
        private bool waitingForTagsResponse = false;
        [SerializeField]
        private int currentPage = 0;
        [SerializeField]
        private ITagsApiConsumer _tagsApiConsumer;
        [SerializeField]
        private List<Tag> tags = new List<Tag>();
        [SerializeField]
        private bool areThereTagsToFetch = true;

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
            try
            {
                if (!areThereTagsToFetch || waitingForTagsResponse)
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
            catch
            {
                waitingForTagsResponse = false;
                currentPage--;
                return tags;
            }
        }

        public void CleanTags()
        {
            if (waitingForTagsResponse)
            {
                Debug.LogWarning("Can't clean tags while waiting for tags request response, please wait a couple of seconds");
                return;
            }
            Reset();
        }

        private void CheckIfThereIsStillTagsToFetch(PaginationResult<Tag> fetchedTagsResult)
        {
            if (string.IsNullOrEmpty(fetchedTagsResult.next))
            {
                areThereTagsToFetch = false;
            }
        }

        private List<Tag> AddNewTags(Tag[] newTags)
        {
            tags = new List<Tag>(tags);
            tags.AddRange(newTags);
            return tags;
        }

        public int GetCurrentPage()
        {
            return currentPage;
        }

        public bool GetWaitingForTagResponse()
        {
            return waitingForTagsResponse;
        }

        public void Reset()
        {
            waitingForTagsResponse = false;
            tags = new List<Tag>();
            currentPage = 0;
            areThereTagsToFetch = true;
        }

        public bool GetAreThereTagsToFetch()
        {
            return areThereTagsToFetch;
        }
    }
}
