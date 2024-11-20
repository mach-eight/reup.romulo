using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.mocks
{
    public class DelayTagsWebRequesterSpy : ITagsApiConsumer
    {
        public int numberOfTimesFetched = 0;
        public int lastPageFetched;
        private PaginationResult<Tag> returnPage = new PaginationResult<Tag>()
        {
            count = 100000,
            next = "some-next-page-url",
            previous = "some-previous-page-url",
            results = TagFactory.CreateBulk(1).ToArray(),
        };
        private int delay;

        public DelayTagsWebRequesterSpy(int delay)
        {
            this.delay = delay;
        }

        public async Task<PaginationResult<Tag>> GetTags()
        {
            lastPageFetched = 1;
            numberOfTimesFetched++;
            await Task.Delay(delay);
            return returnPage;
        }

        public async Task<PaginationResult<Tag>> GetTags(int page)
        {
            lastPageFetched = page;
            numberOfTimesFetched++;
            await Task.Delay(delay);
            return returnPage;
        }

        public async Task<PaginationResult<Tag>> GetTags(int page, int pageSize)
        {
            lastPageFetched = page;
            numberOfTimesFetched++;
            await Task.Delay(delay);
            return returnPage;
        }
    }
}

