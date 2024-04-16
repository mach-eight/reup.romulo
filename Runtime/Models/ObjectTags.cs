using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class ObjectTags : MonoBehaviour, IObjectTags 
    {
        static public string tagsUrl = "https://api-prod-reup.macheight.com/api/v1/";
        public List<string> tags = new List<string>();

        public List<string> GetTags()
        {
            return tags;
        }

        public List<string> AddTag(string tag)
        {
            tags.Add(tag);
            return tags;
        }

        public List<string> RemoveTag(string tag)
        {
            tags.Remove(tag);
            return tags;
        }

        public List<string> AddTags(string[] tagsList)
        {
            tags.AddRange(tagsList);
            return tags;
        }
    }
}
