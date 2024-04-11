using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace Tests.PlayMode.Mocks
{
    public class DelayTagsWebRequesterSpy : ITagsWebRequesterController
    {
        public int numberOfTimesFetched = 0;
        public int lastPageFetched;
        private PaginationResult<ObjectTag> returnPage = new PaginationResult<ObjectTag>()
        {
            count = 100000,
            next = "some-next-page-url",
            previous = "some-previous-page-url",
            results = new ObjectTag[1] { new ObjectTag(){name = "tag0", id = 0, description="description0"}, },
        };
        private int delay;

        public DelayTagsWebRequesterSpy(int delay)
        {
            this.delay = delay;
        }

        public async Task<PaginationResult<ObjectTag>> GetTags()
        {
            lastPageFetched = 1;
            numberOfTimesFetched++;
            await Task.Delay(delay);
            return returnPage;
        }

        public async Task<PaginationResult<ObjectTag>> GetTags(int page)
        {
            lastPageFetched = page;
            numberOfTimesFetched++;
            await Task.Delay(delay);
            return returnPage;
        }
    }
}
