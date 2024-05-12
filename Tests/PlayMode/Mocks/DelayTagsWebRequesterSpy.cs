using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace Tests.PlayMode.Mocks
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
            results = new Tag[1] { new Tag(){name = "tag0", id = "0", description="description0"}, },
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
    }
}

