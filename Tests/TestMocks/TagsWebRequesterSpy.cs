using ReupVirtualTwin.webRequestersInterfaces;
using System.Threading.Tasks;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;
using System.Collections.Generic;

namespace ReupVirtualTwinTests.mocks
{
    public class TagsWebRequesterSpy : ITagsApiConsumer
    {
        public int lastPageRequested;
        public int lastPageSizeRequested;
        public int timesFetched = 0;
        public PaginationResult<Tag> firstPage;
        public PaginationResult<Tag> secondPage;
        public PaginationResult<Tag> thirdPage;

        public TagsWebRequesterSpy()
        {
            firstPage = new PaginationResult<Tag>()
            {
                count = 8,
                next = "url-for-page/2",
                previous = null,
                results = TagFactory.CreateBulk(3).ToArray(),
            };
            secondPage = new PaginationResult<Tag>()
            {
                count = 8,
                next = "url-for-page/3",
                previous = "url-for-page/1",
                results = TagFactory.CreateBulk(3).ToArray(),

            };
            thirdPage = new PaginationResult<Tag>()
            {
                count = 8,
                next = null,
                previous = "url-for-page/2",
                results = TagFactory.CreateBulk(2).ToArray(),
            };
        }

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

        public Task<PaginationResult<Tag>> GetTags(int page = 1, int pageSize = 3)
        {
            timesFetched++;
            lastPageRequested = page;
            lastPageSizeRequested = pageSize;
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
