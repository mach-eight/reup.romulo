using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface ITagsApiConsumer
    {
        public Task<PaginationResult<ObjectTag>> GetTags();
        public Task<PaginationResult<ObjectTag>> GetTags(int page);
    }
}
