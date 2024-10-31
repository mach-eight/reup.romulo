using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.models
{
    public class ObjectTags : MonoBehaviour, IObjectTags
    {
        public List<Tag> tags = new List<Tag>();

        public List<Tag> GetTags()
        {
            return tags;
        }

        public List<Tag> AddTag(Tag tag)
        {
            tags.Add(tag);
            return tags;
        }

        public List<Tag> RemoveTag(Tag tag)
        {
            tags.RemoveAll(t => t.id == tag.id);
            return tags;
        }

        public List<Tag> AddTags(Tag[] tagsList)
        {
            tags.AddRange(tagsList);
            return tags;
        }

        public void RemoveAllTags()
        {
            tags.Clear();
        }
    }
}
