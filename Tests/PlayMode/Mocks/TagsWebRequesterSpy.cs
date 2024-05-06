using ReupVirtualTwin.webRequestersInterfaces;
using System.Threading.Tasks;
using ReupVirtualTwin.dataModels;

namespace Tests.PlayMode.Mocks
{
    class TagsWebRequesterSpy : ITagsApiConsumer
    {
        public int lastPageRequested;
        public int lastPageSizeRequested;
        public int timesFetched = 0;

        private PaginationResult<ObjectTag> firstPage = new PaginationResult<ObjectTag>()
        {
            count = 8,
            next = "url-for-page/2",
            previous = null,
            results = new ObjectTag[] {
                new ObjectTag
                {
                    id = 0,
                    name = "tag0",
                    description = "tag0 description"
                },
                new ObjectTag
                {
                    id = 1,
                    name = "tag1",
                    description = "tag1 description"
                },
                new ObjectTag
                {
                    id = 2,
                    name = "tag2",
                    description = "tag2 description"
                },
            }
        };

        private PaginationResult<ObjectTag> secondPage = new PaginationResult<ObjectTag>()
        {
            count = 8,
            next = "url-for-page/3",
            previous = "url-for-page/1",
            results = new ObjectTag[] {
                new ObjectTag
                {
                    id = 3,
                    name = "tag3",
                    description = "tag3 description"
                },
                new ObjectTag
                {
                    id = 4,
                    name = "tag4",
                    description = "tag4 description"
                },
                new ObjectTag
                {
                    id = 5,
                    name = "tag5",
                    description = "tag5 description"
                },
            }
        };

        private PaginationResult<ObjectTag> thirdPage = new PaginationResult<ObjectTag>()
        {
            count = 8,
            next = null,
            previous = "url-for-page/2",
            results = new ObjectTag[] {
                new ObjectTag
                {
                    id = 6,
                    name = "tag6",
                    description = "tag6 description"
                },
                new ObjectTag
                {
                    id = 7,
                    name = "tag7",
                    description = "tag7 description"
                },
            }
        };

        public Task<PaginationResult<ObjectTag>> GetTags()
        {
            timesFetched++;
            lastPageRequested = 1;
            return Task.FromResult(firstPage);
        }

        public Task<PaginationResult<ObjectTag>> GetTags(int page = 1)
        {
            timesFetched++;
            lastPageRequested = page;
            if (page == 1)
            {
                return Task.FromResult(firstPage);
            }
            if (page == 2)
            {
                return Task.FromResult(secondPage);
            }
            return Task.FromResult(thirdPage);
        }
    }

}
