using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ITagsApiManager
    {
        public ITagsApiConsumer webRequester { get; set; }
        public Task<List<ObjectTag>> GetTags();
        public Task<List<ObjectTag>> LoadMoreTags();
        public void CleanTags();
    }
}
