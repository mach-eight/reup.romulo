using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ITagsController
    {
        public List<Tag> GetTagsFromObject(GameObject obj);
        public List<Tag> AddTagToObject(GameObject obj, Tag tag);
        public List<Tag> RemoveTagFromObject(GameObject obj, Tag tag);
        public bool DoesObjectHaveTag(GameObject obj, string tagId);
        public string[] GetTagNamesFromObject(GameObject obj);
    }
}
