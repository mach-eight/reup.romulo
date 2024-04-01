using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IObjectTags
    {
        public List<string> GetTags();
        public List<string> AddTag(string tag);
        public List<string> AddTags(string[] tagsList);
        public List<string> RemoveTag(string tag);
    }
}
