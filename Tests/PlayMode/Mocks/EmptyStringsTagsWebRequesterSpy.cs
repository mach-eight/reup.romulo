using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace Tests.PlayMode.Mocks
{
    public class EmptyStringsTagsWebRequesterSpy : ITagsApiConsumer
    {
        public int numberOfTimesFetched = 0;

        private PaginationResult<Tag> returnPage = new PaginationResult<Tag>()
        {
            count = 0,
            next = "",
            previous = "",
            results = new Tag[0],
        };
        public Task<PaginationResult<Tag>> GetTags()
        {
            numberOfTimesFetched++;
            return Task.FromResult(returnPage);
        }

        public Task<PaginationResult<Tag>> GetTags(int page)
        {
            numberOfTimesFetched++;
            return Task.FromResult(returnPage);
        }

    }
}

