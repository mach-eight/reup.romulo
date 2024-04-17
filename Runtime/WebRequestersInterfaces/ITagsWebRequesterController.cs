using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ITagsWebRequesterController
    {
        public Task<PaginationResult<ObjectTag>> GetTags();
        public Task<PaginationResult<ObjectTag>> GetTags(int page);
    }
}
