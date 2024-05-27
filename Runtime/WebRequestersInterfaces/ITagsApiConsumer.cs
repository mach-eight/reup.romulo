using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface ITagsApiConsumer
    {
        public Task<PaginationResult<Tag>> GetTags();
        public Task<PaginationResult<Tag>> GetTags(int page);
    }
}
