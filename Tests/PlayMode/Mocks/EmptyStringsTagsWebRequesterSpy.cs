using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace Tests.PlayMode.Mocks
{
    public class EmptyStringsTagsWebRequesterSpy : ITagsApiConsumer
    {
        public int numberOfTimesFetched = 0;

        private PaginationResult<ObjectTag> returnPage = new PaginationResult<ObjectTag>()
        {
            count = 0,
            next = "",
            previous = "",
            results = new ObjectTag[0],
        };
        public Task<PaginationResult<ObjectTag>> GetTags()
        {
            numberOfTimesFetched++;
            return Task.FromResult(returnPage);
        }

        public Task<PaginationResult<ObjectTag>> GetTags(int page)
        {
            numberOfTimesFetched++;
            return Task.FromResult(returnPage);
        }

    }
}
