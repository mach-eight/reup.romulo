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

        private PaginationResult<Tag> firstPage = new PaginationResult<Tag>()
        {
            count = 8,
            next = "url-for-page/2",
            previous = null,
            results = new Tag[] {
                new Tag
                {
                    id = "0",
                    name = "tag0",
                    description = "tag0 description"
                },
                new Tag
                {
                    id = "1",
                    name = "tag1",
                    description = "tag1 description"
                },
                new Tag
                {
                    id = "2",
                    name = "tag2",
                    description = "tag2 description"
                },
            }
        };

        private PaginationResult<Tag> secondPage = new PaginationResult<Tag>()
        {
            count = 8,
            next = "url-for-page/3",
            previous = "url-for-page/1",
            results = new Tag[] {
                new Tag
                {
                    id = "3",
                    name = "tag3",
                    description = "tag3 description"
                },
                new Tag
                {
                    id = "4",
                    name = "tag4",
                    description = "tag4 description"
                },
                new Tag
                {
                    id = "5",
                    name = "tag5",
                    description = "tag5 description"
                },
            }
        };

        private PaginationResult<Tag> thirdPage = new PaginationResult<Tag>()
        {
            count = 8,
            next = null,
            previous = "url-for-page/2",
            results = new Tag[] {
                new Tag
                {
                    id = "6",
                    name = "tag6",
                    description = "tag6 description"
                },
                new Tag
                {
                    id = "7",
                    name = "tag7",
                    description = "tag7 description"
                },
            }
        };

        public Task<PaginationResult<Tag>> GetTags()
        {
            timesFetched++;
            lastPageRequested = 1;
            return Task.FromResult(firstPage);
        }

        public Task<PaginationResult<Tag>> GetTags(int page = 1)
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
