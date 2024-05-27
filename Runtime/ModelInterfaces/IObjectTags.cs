using ReupVirtualTwin.dataModels;
using System.Collections.Generic;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IObjectTags
    {
        public List<Tag> GetTags();
        public List<Tag> AddTag(Tag tag);
        public List<Tag> AddTags(Tag[] tagsList);
        public List<Tag> RemoveTag(Tag tag);
    }
}
