using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ITagsApiManager
    {
        public ITagsWebRequesterController webRequester { get; set; }
        public Task<List<ObjectTag>> GetTags();
        public Task<List<ObjectTag>> LoadMoreTags();
        public void CleanTags();
    }
}
