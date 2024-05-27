using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.models
{
    public class ObjectTags : MonoBehaviour, IObjectTags 
    {
        static public string tagsUrl = "https://api-prod-reup.macheight.com/api/v1/";
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
            tags.Remove(tag);
            return tags;
        }

        public List<Tag> AddTags(Tag[] tagsList)
        {
            tags.AddRange(tagsList);
            return tags;
        }
    }
}
