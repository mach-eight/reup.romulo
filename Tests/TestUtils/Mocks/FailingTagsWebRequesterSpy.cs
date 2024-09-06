using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.webRequestersInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ReupVirtualTwinTests.utils.mocks
{
    public class FailingTagsWebRequesterSpy : ITagsApiConsumer
    {
        public Task<PaginationResult<Tag>> GetTags()
        {
            throw new System.Exception("Error from FailingTagsWebRequesterSpy");
        }

        public Task<PaginationResult<Tag>> GetTags(int page)
        {
            throw new System.Exception("Error from FailingTagsWebRequesterSpy");
        }
    }
}
