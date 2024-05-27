using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ITagsApiManager
    {
        public ITagsApiConsumer tagsApiConsumer { get; set; }
        public Task<List<Tag>> GetTags();
        public Task<List<Tag>> LoadMoreTags();
        public void CleanTags();
        public string searchTagText { get; set; }
    }
}
