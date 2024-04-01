using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IObjectTags
    {
        public List<EditionTag> GetTags();
        public List<EditionTag> AddTag(EditionTag tag);
        public List<EditionTag> AddTags(EditionTag[] tagsList);
        public List<EditionTag> RemoveTag(EditionTag tag);
    }
}
